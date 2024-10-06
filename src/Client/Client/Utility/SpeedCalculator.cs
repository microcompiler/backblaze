using System;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Utility
{
    /// <summary>
    /// Calculates transfer speed as a rolling average. Every time the position changes,
    /// the consumer notifies us and a timestamped sample is logged.
    /// </summary>
    public class SpeedCalculator
    {
        List<Sample> _samples = new List<Sample>();

        struct Sample
        {
            public long Position;
            public DateTime DateTimeUTC;
        }

        /// <summary>
        /// The length in seconds of the window across which speed is averaged.
        /// </summary>
        public const int WindowSeconds = 10;

        /// <summary>
        /// Adds a position sample to the set. It is automatically timestamped. Samples
        /// should be monotonically increasing. If, for whatever reason, they are not,
        /// previously-added samples that are later in the file are discarded so that
        /// the set remains strictly increasing.
        /// </summary>
        /// <param name="position">The updated position of the operation.</param>
        public void AddSample(long position)
        {
            var sample = new Sample();

            sample.Position = position;
            sample.DateTimeUTC = DateTime.UtcNow;

            // If we have walked backward for whatever reason, discard any samples past this
            // point so that we maintain the invariant of the sample set increasing position
            // monotonically.
            while ((_samples.Count > 0) && (_samples[_samples.Count - 1].Position > position))
                _samples.RemoveAt(_samples.Count - 1);

            _samples.Add(sample);
        }

        /// <summary>
        /// Calculates the current speed based on samples previously added by calls to
        /// <see cref="AddSample" />. The value of this function will change over time,
        /// even with no changes to the state of the <see cref="SpeedCalculator" />
        /// instance, because the value is relative to the current date/time, and the
        /// samples with which the calculation is being made are timestamped.
        /// </summary>
        /// <returns>The average number of bytes per second being processed.</returns>
        public long CalculateBytesPerSecond()
        {
            var cutoff = DateTime.UtcNow.AddSeconds(-WindowSeconds);

            // Discard any samples that are outside of the averaging window. We will never
            // need them again.
            while ((_samples.Count > 0) && (_samples[0].DateTimeUTC < cutoff))
                _samples.RemoveAt(0);

            if (_samples.Count < 2)
                return 0;

            var firstSample = _samples[0];
            var lastSample = _samples[_samples.Count - 1];

            long bytes = lastSample.Position - firstSample.Position;
            double seconds = (lastSample.DateTimeUTC - firstSample.DateTimeUTC).TotalSeconds;

            // If we don't have a meaningful span of time, clamp it. The number wouldn't
            // be terribly meaningful anyway.
            if (seconds < 0.01)
                seconds = 0.01;

            return (long)Math.Round(bytes / seconds);
        }
    }
}
