using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Represents a cors rule and related properties.
    /// </summary>
    public class CorsRule : IEquatable<CorsRule>
    {
        /// <summary>
        /// Represents the minimum age seconds. 
        /// </summary>
        public const long MinimumAgeSeconds = 0;

        /// <summary>
        /// Represents the maximum age seconds. 
        /// </summary>
        public const long MaximumAgeSeconds = 86400;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsRule" /> class.
        /// </summary>
        public CorsRule(string corsRuleName, List<string> allowedOrigins, List<string> allowedOperations, int maxAgeSeconds)
        {
            CorsRuleName = corsRuleName;
            AllowedOrigins = allowedOrigins;
            AllowedOperations = allowedOperations;
            MaxAgeSeconds = maxAgeSeconds;
        }

        /// <summary>
        /// Gets cors rule name for this bucket. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string CorsRuleName { get; private set; }

        /// <summary>
        /// Gets a list specifying which origins the rule covers. At least one value must be specified.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<string> AllowedOrigins { get; private set; }

        /// <summary>
        /// Get a list specifying which operations the rule allows. At least one value must be specified.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<string> AllowedOperations { get; private set; }

        /// <summary>
        /// Gets or sets a list of headers that are allowed in a pre-flight OPTIONS request. 
        /// If present requests 'Access-Control-Request-Headers' value is set.
        /// </summary>
        public List<string> AllowedHeaders { get; set; }

        /// <summary>
        /// If present this is a list of headers that may be exposed to an application inside the client (eg. exposed to Javascript in a browser). 
        /// </summary>
        public List<string> ExposeHeaders { get; set; }

        /// <summary>
        /// Gets the maximum number of seconds that a browser may cache the response to a preflight request. 
        /// The value must not be negative and it must not be more than <see cref="MaxAgeSeconds"/> seconds.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public int MaxAgeSeconds
        {
            get { return _maxAgeSeconds; }
            set
            {
                if (value > MinimumAgeSeconds || value < MaximumAgeSeconds)
                    _maxAgeSeconds = value;
                else
                    throw new ArgumentOutOfRangeException($"Argument must be a minimum of {MinimumAgeSeconds} and a maximum of {MaximumAgeSeconds} duration in seconds.", nameof(MaxAgeSeconds));
            }
        }
        private int _maxAgeSeconds;

        /// <summary>
        /// Returns the name of this <see cref="CorsRuleName" />.
        /// </summary>
        public override string ToString()
        {
            return CorsRuleName;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        public override bool Equals(object obj)
        {
            return Equals(obj as CorsRule);
        }

        /// <summary>
        /// Determines whether this instance is equal to another <see cref="CorsRule" />.
        /// </summary>
        /// <param name="other">The <see cref="CorsRule" /> to compare to this instance.</param>
        public bool Equals(CorsRule other)
        {
            return other != null &&
                   CorsRuleName == other.CorsRuleName &&
                   EqualityComparer<List<string>>.Default.Equals(AllowedOrigins, other.AllowedOrigins) &&
                   EqualityComparer<List<string>>.Default.Equals(AllowedOperations, other.AllowedOperations) &&
                   EqualityComparer<List<string>>.Default.Equals(AllowedHeaders, other.AllowedHeaders) &&
                   EqualityComparer<List<string>>.Default.Equals(ExposeHeaders, other.ExposeHeaders) &&
                   MaxAgeSeconds == other.MaxAgeSeconds;
        }

        /// <summary>
        /// Returns a hash code for this <see cref="CorsRule" />.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = -235503683;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CorsRuleName);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(AllowedOrigins);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(AllowedOperations);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(AllowedHeaders);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(ExposeHeaders);
            hashCode = hashCode * -1521134295 + MaxAgeSeconds.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Compares two <see cref="CorsRule" /> instances for equality.
        /// </summary>
        /// <param name="a">The first <see cref="CorsRule" /> to compare.</param>
        /// <param name="b">The second <see cref="CorsRule" /> to compare.</param>
        public static bool operator ==(CorsRule a, CorsRule b)
        {
            return EqualityComparer<CorsRule>.Default.Equals(a, b);
        }

        /// <summary>
        /// Compares two <see cref="CorsRule" /> instances for inequality.
        /// </summary>
        /// <param name="a">The first <see cref="CorsRule" /> to compare.</param>
        /// <param name="b">The second <see cref="CorsRule" /> to compare.</param>
        public static bool operator !=(CorsRule a, CorsRule b)
        {
            return !(a == b);
        }
    }
}
