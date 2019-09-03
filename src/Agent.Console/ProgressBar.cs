using System;
using System.Linq;
using System.Diagnostics;

using Bytewizer.Backblaze.Models;
using System.Text;

namespace Bytewizer.Backblaze.Agent
{
    /// <summary>
    /// Represents a console progress bar which uses <see cref="IProgress{T}"/> for status updates.
    /// </summary>
    public class ProgressBar : NaiveProgress<ICopyProgress>
    {
        /// <summary>
        /// Stopwatch to track download speed.
        /// </summary>
        private readonly Stopwatch lastUpdated = new Stopwatch();

        /// <summary>
        /// Reported bytes transfer so far.
        /// </summary>
        private long lastTransfered;

        /// <summary>
        /// Reported text so far.
        /// </summary>
        private string currentText = string.Empty;

        /// <summary>
        /// The frequency with which the progress bar updates.
        /// </summary>
        public int RefreshRate
        {
            get { return _refreshRate; }
            set
            {
                if (value <= 0)
                { _refreshRate = 1; }
                else
                { _refreshRate = value; }
            }
        }
        private int _refreshRate = 333;
        
        /// <summary>
        /// Number of progress bar blocks to display.
        /// </summary>
        public int BlockLength
        {
            get { return _blockLength; }
            set
            {
                if (value <= 0)
                { _blockLength = 20; }
                else
                { _blockLength = value; }
            }
        }
        private int _blockLength = 20;

        /// <summary>
        /// The character to display in the progress bar.
        /// </summary>
        public char BlockCharacter { get; set; } = '#';

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        public ProgressBar()
        { }

        /// <summary>
        /// Reports progress status to console.
        /// </summary>
        /// <param name="progress"></param>
        public void Report(ICopyProgress progress)
        {

            if (!lastUpdated.IsRunning)
            {
                lastUpdated.Start();
                return;
            }
            else
            {
                if (lastUpdated.ElapsedMilliseconds < RefreshRate && progress.PercentComplete != 1) { return; }
            }

            int transferRate = (int)((progress.BytesTransferred - lastTransfered) / Math.Max(RefreshRate, lastUpdated.ElapsedMilliseconds) * 1000);
            lastTransfered = progress.BytesTransferred;
            lastUpdated.Restart();

            string progressBar = $"[{string.Concat(Enumerable.Repeat(BlockCharacter, (int)Math.Floor(progress.PercentComplete * BlockLength))).PadRight(BlockLength)}]";
            string transfered = FormatFilesize(progress.BytesTransferred);
            string expected = FormatFilesize(progress.ExpectedBytes);
            string rate = $"{FormatFilesize(transferRate)}/s";
            string transferRatio = $"{ transfered } / { expected}";
            string percent = progress.PercentComplete.ToString("P").PadLeft(7);

            string progressLine = $"\r{percent} {progressBar} {transferRatio} @ {rate}";

            UpdateText(progressLine);

            if (progress.PercentComplete == 1)
            {
                lastTransfered = 0;
                lastUpdated.Stop();
            }
        }

        /// <summary>
        /// Display progress bar text to console clearing previous line.
        /// </summary>
        /// <param name="text">The text to display.</param>
        private void UpdateText(string text)
        {
            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            // Backtrack to the first differing character
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', currentText.Length - commonPrefixLength);

            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));

            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }

            Console.Write(outputBuilder);
            currentText = text;
        }

        /// <summary>
        /// Converts a file size into human-readable format.
        /// </summary>
        /// <param name="size">The file size to format.</param>
        private string FormatFilesize(long size)
        {
            double dSize = size;
            int index = 0;
            for (; dSize > 1024; index++)
                dSize /= 1024;
            return dSize.ToString("0.000 " + new[] { "B", "KB", "MB", "GB", "TB" }[index]);
        }
    }
}
