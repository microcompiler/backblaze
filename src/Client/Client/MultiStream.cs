using System;
using System.IO;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Implements a <see cref="Stream"/> that operates on a specified range of a source <see cref="Stream"/>.
    /// </summary>
    public class MultiStream : Stream
    {
        private readonly Stream _stream;
        private readonly long _position;
        private readonly long _length;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiStream"/> class.
        /// </summary>
        /// <param name="stream">The source stream.</param>
        /// <param name="position">A zero-based start positon in source stream.</param>
        /// <param name="length">Length of stream.</param>
        public MultiStream(Stream stream, long position, long length)
        {
            if (!stream.CanRead || !stream.CanWrite || !stream.CanSeek)
                throw new ArgumentException(nameof(stream));

            _stream = stream;
            _position = position;
            _length = length;
            _stream.Seek(position, SeekOrigin.Begin);
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead => _stream.CanRead;

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek => _stream.CanSeek;

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite => _stream.CanWrite;

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length => _length;

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        public override long Position {
            get => _stream.Position - _position;
            set => _stream.Position = value + _position;
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            _stream.Flush();
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            long maxRead = Length - Position;
            return _stream.Read(buffer, offset, (count <= maxRead) ? count : (int)maxRead);
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <b>origin</b> parameter.</param>
        /// <param name="origin">A value of type <see cref="SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset + _position, origin);
        }

        /// <summary>
        /// Sets the length of the current stream. 
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }
    }
}