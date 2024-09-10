using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Bytewizer.Backblaze.Models;
using Bytewizer.Backblaze.Utility;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Implements <see cref="HttpContent" /> which supports an event action for send operations in an <see cref="HttpClient"/>.
    /// </summary>
    public class ProgressStreamContent : HttpContent
    {
        private const int defaultBufferSize = 16384;

        private readonly Stream _content;
        private readonly int _bufferSize;
        private readonly long _expectedContentLength;
        private readonly bool _handleStreamDispose = false;
        private bool _contentConsumed;
        private readonly IProgress<ICopyProgress> _progressReport;

        /// <summary>
        /// Basic constructor which uses a default bufferSize and a zero expectedContentLength.
        /// </summary>
        /// <param name="content">The stream content to write.</param>
        /// <param name="progressReport">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="handleStreamDispose">When set true, the content stream is disposed when this object is disposed.</param>
        public ProgressStreamContent(Stream content, IProgress<ICopyProgress> progressReport, bool handleStreamDispose) 
            : this(content, defaultBufferSize, 0, progressReport, handleStreamDispose) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressStreamContent"/> class.
        /// </summary>
        /// <param name="content">The source stream to read from.</param>
        /// <param name="bufferSize">The size of the buffer to allocate in bytes. Sane values are typically 4096-81920. Setting a buffer of more than ~85k is likely to degrade performance.</param>
        /// <param name="expectedContentLength">Overrides the content stream length if the stream type does not provide one. Used for progress reporting.</param>
        /// <param name="progressReport">A progress action which fires every time the write buffer is cycled.</param>
        /// <param name="handleStreamDispose">When set true, the content stream is disposed when this object is disposed.</param>
        public ProgressStreamContent(Stream content, int bufferSize, long expectedContentLength, IProgress<ICopyProgress> progressReport, bool handleStreamDispose)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            _content = content ?? throw new ArgumentNullException("content");
            _handleStreamDispose = handleStreamDispose;
            _bufferSize = bufferSize;
            _expectedContentLength = expectedContentLength;
            _progressReport = progressReport;
        }

        /// <summary>
        /// Copies the source content stream into the given destination stream.
        /// </summary>
        /// <param name="stream">The destination stream to write to.</param>
        /// <param name="context">Transportation context.</param>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (stream == null) { throw new ArgumentNullException("stream"); }

            PrepareContent();

            return Task.Run(() =>
            {
                var totalTime = new System.Diagnostics.Stopwatch();
                totalTime.Start();

                var buffer = new byte[_bufferSize];
                long streamLength = _content.CanSeek ? _content.Length : 0;
                long size = _expectedContentLength > 0 ? _expectedContentLength : streamLength;
                long uploaded = 0;

                var speedCalculator = new SpeedCalculator();

                speedCalculator.AddSample(0);

                while (true)
                {
                    var length = _content.Read(buffer, 0, buffer.Length);
                    uploaded += length;
                    if (length <= 0) { break; }

                    stream.Write(buffer, 0, length);

                    speedCalculator.AddSample(uploaded);

                    _progressReport?.Report(new CopyProgress(totalTime.Elapsed, speedCalculator.CalculateBytesPerSecond(), uploaded, size));
                }
            });
        }

        /// <summary>
        /// Returns the http content length.
        /// </summary>
        protected override bool TryComputeLength(out long length)
        {
            if (_content.CanSeek)
            {
                length = _content.Length;
                return true;
            }
            length = 0;
            return false;
        }

        /// <summary>
        /// Disposes the stream handled by this object if handleStreamDispose is true.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _handleStreamDispose)
            {
                _content.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PrepareContent()
        {
            if (_contentConsumed)
            {
                if (_content.CanSeek)
                {
                    _content.Position = 0;
                }
                else
                {
                    throw new InvalidOperationException("Stream already read. Cannot seek on this stream type.");
                }
            }

            _contentConsumed = true;
        }
    }
}
