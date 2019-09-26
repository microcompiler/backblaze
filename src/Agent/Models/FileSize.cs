using System;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents a file size and related properties.
    /// </summary>
    public struct FileSize : IComparable, IComparable<FileSize>, IEquatable<FileSize>
    {

        /// <summary>
        /// Represents the number of bytes in one kilobyte. This field is constant.
        /// </summary>
        public const long KiloByte = 1024L;

        /// <summary>
        /// Represents the number of bytes in one megabyte. This field is constant.
        /// </summary>
        public const long MegaByte = 1024L * 1024;

        /// <summary>
        /// Represents the number of bytes in one gigabyte. This field is constant.
        /// </summary>
        public const long GigaByte = 1024L * 1024 * 1024;

        /// <summary>
        /// Represents the number of bytes in one terabyte. This field is constant.
        /// </summary>
        public const long TeraByte = 1024L * 1024 * 1024 * 1024;

        /// <summary>
        /// Gets the number of bytes that represent the value of this instance. 
        /// </summary>
        public long Bytes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSize" /> structure with the number of bytes.
        /// </summary>
        /// <param name="bytes">A <see cref="long" /> value specifying the number of bytes.</param>
        public FileSize(long bytes)
        {
            Bytes = bytes;
        }

        /// <summary>
        /// Returns a new <see cref="FileSize" /> value that is the sum of this instance and the specified <see cref="FileSize" /> value.
        /// </summary>
        /// <param name="value">The <see cref="FileSize" /> value to add.</param>
        /// <returns>
        /// A new <see cref="FileSize" /> value that represents the sum of this instance and the specified <see cref="FileSize" /> value.
        /// </returns>
        public FileSize Add(FileSize value)
        {
            return new FileSize(Bytes + value.Bytes);
        }

        /// <summary>
        /// Returns a new <see cref="FileSize" /> value that is the difference of this instance and the specified <see cref="FileSize" /> value.
        /// </summary>
        /// <param name="value">The <see cref="FileSize" /> value to subtract.</param>
        /// <returns>
        /// A new <see cref="FileSize" /> value that represents the difference of this instance and the specified <see cref="FileSize" /> value.
        /// </returns>
        public FileSize Subtract(FileSize value)
        {
            return new FileSize(Bytes - value.Bytes);
        }

        /// <summary>
        /// Returns a new <see cref="FileSize" /> value that is the result of this instance, multiplied by the specified <see cref="long" /> value.
        /// </summary>
        /// <param name="value">The multiplier.</param>
        /// <returns>
        /// A new <see cref="FileSize" /> value that represents the result of this instance, multiplied by the specified <see cref="long" /> value.
        /// </returns>
        public FileSize Multiply(long value)
        {
            return new FileSize(Bytes * value);
        }

        /// <summary>
        /// Returns a new <see cref="FileSize" /> value that is the result of this instance, divided by the specified <see cref="long" /> value.
        /// </summary>
        /// <param name="value">The divisor.</param>
        /// <returns>
        /// A new <see cref="FileSize" /> value that represents the result of this instance, divided by the specified <see cref="long" /> value.
        /// </returns>
        public FileSize Divide(long value)
        {
            return new FileSize(Bytes / value);
        }

        /// <summary>
        /// Gets the closest bytes siz unit in which the value of this instance can be represented.
        /// <para>Example: 1023 bytes is "bytes"; 1024 bytes is "KB"</para>
        /// </summary>
        public string ClosestUnit
        {
            get
            {
                if (Bytes < KiloByte) return "bytes";
                else if (Bytes < MegaByte) return "KB";
                else if (Bytes < GigaByte) return "MB";
                else if (Bytes < TeraByte) return "MB";
                else return "TB";
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// An equivalent <see cref="string" /> representing this instance.
        /// </returns>
        public string Format(string format)
        {
            return $"{Bytes.ToString(format)} {ClosestUnit}";
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="FileSize" /> and returns a comparison of their relative values.
        /// </summary>
        /// <param name="obj">An <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// </returns>
        public int CompareTo(object obj)
        {
            if (!(obj is FileSize))
                throw new ArgumentException($"{nameof(obj)} is not the same type as this instance.", nameof(obj));

            return CompareTo((FileSize)obj);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="FileSize" /> and returns a comparison of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="FileSize" /> to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// </returns>
        public int CompareTo(FileSize other)
        {
            return Bytes.CompareTo(other.Bytes);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Bytes.ToString();
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <c>true</c>, if the specified <see cref="object" /> is equal to this instance;
        /// otherwise, <c>false</c>.
        public override bool Equals(object obj)
        {
            return obj is FileSize byteSize && Equals(byteSize);
        }

        /// <summary>
        /// Determines whether this instance is equal to another <see cref="FileSize" />.
        /// </summary>
        /// <param name="other">The <see cref="FileSize" /> to compare to this instance.</param>
        /// <returns><c>true</c>, if this instance is equal to the <paramref name="other" /> parameter; otherwise, <c>false</c>.</returns>
        public bool Equals(FileSize other)
        {
            return Bytes == other.Bytes;
        }

        /// <summary>
        /// Returns a hash code for this <see cref="FileSize" />.
        /// </summary>
        /// <returns>
        /// The hash code for this <see cref="FileSize" /> instance.
        /// </returns>
        public override int GetHashCode()
        {
            return Bytes.GetHashCode();
        }


        /// <summary>
        /// Returns a <see cref="FileSize" /> that represents a specified number kilobytes.
        /// </summary>
        /// <param name="value">A <see cref="uint" /> value specifying the number of kilobytes.</param>
        /// <returns>
        /// A <see cref="FileSize" /> that represents <paramref name="value" />.
        /// </returns>
        public static FileSize FromKiloBytes(uint value)
        {
            return new FileSize(value * KiloByte);
        }

        /// <summary>
        /// Returns a <see cref="FileSize" /> that represents a specified number megabytes.
        /// </summary>
        /// <param name="value">A <see cref="uint" /> value specifying the number of megabytes.</param>
        /// <returns>
        /// A <see cref="FileSize" /> that represents <paramref name="value" />.
        /// </returns>
        public static FileSize FromMegaBytes(uint value)
        {
            return new FileSize(value * MegaByte);
        }

        /// <summary>
        /// Returns a <see cref="FileSize" /> that represents a specified number gigabytes.
        /// </summary>
        /// <param name="value">A <see cref="uint" /> value specifying the number of gigabytes.</param>
        /// <returns>
        /// A <see cref="FileSize" /> that represents <paramref name="value" />.
        /// </returns>
        public static FileSize FromGigaBytes(uint value)
        {
            return new FileSize(value * GigaByte);
        }

        /// <summary>
        /// Returns a <see cref="FileSize" /> that represents a specified number terabytes.
        /// </summary>
        /// <param name="value">A <see cref="uint" /> value specifying the number of terabytes.</param>
        /// <returns>
        /// A <see cref="FileSize" /> that represents <paramref name="value" />.
        /// </returns>
        public static FileSize FromTeraBytes(uint value)
        {
            return new FileSize(value * TeraByte);
        }


        /// <summary>
        /// Defines an implicit conversion of a <see cref="ulong" /> to a <see cref="FileSize" />.
        /// </summary>
        /// <param name="value">The <see cref="long" /> to convert.</param>
        public static implicit operator FileSize(long value)
        {
            return new FileSize(value);
        }

        /// <summary>
        /// Defines an explicit conversion of a <see cref="FileSize" /> to a <see cref="long" />.
        /// </summary>
        /// <param name="value">The <see cref="FileSize" /> to convert.</param>
        public static explicit operator long(FileSize value)
        {
            return value.Bytes;
        }

        /// <summary>
		/// Compares two <see cref="FileSize" /> values for equality.
		/// </summary>
		/// <param name="a">The first <see cref="FileSize" /> to compare.</param>
		/// <param name="b">The second <see cref="FileSize" /> to compare.</param>
		/// <returns>
		/// <c>true</c>, if both <see cref="FileSize" /> values are equal;
		/// otherwise, <c>false</c>.
		/// </returns>
		public static bool operator ==(FileSize a, FileSize b)
        {
            return a.Bytes == b.Bytes;
        }

        /// <summary>
        /// Compares two <see cref="FileSize" /> values for inequality.
        /// </summary>
        /// <param name="a">The first <see cref="FileSize" /> to compare.</param>
        /// <param name="b">The second <see cref="FileSize" /> to compare.</param>
        /// <returns>
        /// <c>true</c>, if both <see cref="FileSize" /> values are not equal;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(FileSize a, FileSize b)
        {
            return a.Bytes != b.Bytes;
        }

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="FileSize" /> value is less than another specified <see cref="FileSize" /> value.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>
        /// <c>true</c>, if <paramref name="a" /> is less than <paramref name="b" />;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(FileSize a, FileSize b)
        {
            return a.Bytes < b.Bytes;
        }

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="FileSize" /> value is less than or equal to another specified <see cref="FileSize" /> value.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>
        /// <c>true</c>, if <paramref name="a" /> is less than or equal to <paramref name="b" />;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <=(FileSize a, FileSize b)
        {
            return a.Bytes <= b.Bytes;
        }

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="FileSize" /> value is greater than another specified <see cref="FileSize" /> value.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>
        /// <c>true</c>, if <paramref name="a" /> is greater than <paramref name="b" />;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(FileSize a, FileSize b)
        {
            return a.Bytes > b.Bytes;
        }

        /// <summary>
        /// Returns a value indicating whether a specified <see cref="FileSize" /> value is greater than or equal to another specified <see cref="FileSize" /> value.
        /// </summary>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <returns>
        /// <c>true</c>, if <paramref name="a" /> is greater than or equal to <paramref name="b" />;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >=(FileSize a, FileSize b)
        {
            return a.Bytes >= b.Bytes;
        }

        /// <summary>
        /// Adds two specified <see cref="FileSize" /> values.
        /// </summary>
        /// <param name="a">The first value to add.</param>
        /// <param name="b">The second value to add.</param>
        /// <returns>
        /// The result of adding <paramref name="a" /> and <paramref name="b" />.
        /// </returns>
        public static FileSize operator +(FileSize a, FileSize b)
        {
            return a.Add(b);
        }

        /// <summary>
		/// Increments the <see cref="FileSize" /> operand by 1.
		/// </summary>
		/// <param name="value">The value to increment.</param>
		/// <returns>
		/// <paramref name="value" /> incremented by 1.
		/// </returns>
		public static FileSize operator ++(FileSize value)
        {
            return value.Add(1);
        }

        /// <summary>
        /// Subtracts two specified <see cref="FileSize" /> values.
        /// </summary>
        /// <param name="a">The minuend.</param>
        /// <param name="b">The subtrahend.</param>
        /// <returns>
        /// The result of adding <paramref name="b" /> from <paramref name="a" />.
        /// </returns>
        public static FileSize operator -(FileSize a, FileSize b)
        {
            return a.Subtract(b);
        }

        /// <summary>
        /// Decrements the <see cref="FileSize" /> operand by 1.
        /// </summary>
        /// <param name="value">The value to decrement.</param>
        /// <returns>
        /// <paramref name="value" /> decremented by 1.
        /// </returns>
        public static FileSize operator --(FileSize value)
        {
            return value.Subtract(1);
        }

        /// <summary>
        /// Multiplies a specified <see cref="FileSize" /> value and a specified <see cref="ulong" /> value.
        /// </summary>
        /// <param name="a">The first value to multiply.</param>
        /// <param name="b">The second value to multiply.</param>
        /// <returns>
        /// The result of multiplying <paramref name="a" /> by <paramref name="b" />.
        /// </returns>
        public static FileSize operator *(FileSize a, long b)
        {
            return a.Multiply(b);
        }
        /// <summary>
        /// Divides a specified <see cref="FileSize" /> value and a specified <see cref="ulong" /> value.
        /// </summary>
        /// <param name="a">The dividend.</param>
        /// <param name="b">The divisor.</param>
        /// <returns>
        /// The result of dividing <paramref name="a" /> by <paramref name="b" />.
        /// </returns>
        public static FileSize operator /(FileSize a, long b)
        {
            return a.Divide(b);
        }
    }
}
