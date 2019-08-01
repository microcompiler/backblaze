using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// Contains response information related to a CORS rule.
    /// </summary>
    public class CorsRule : IEquatable<CorsRule>
    {
        /// <summary>
        /// Cors rule name for this bucket. 
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string CorsRuleName { get; set; }

        /// <summary>
        /// A non-empty list specifying which origins the rule covers.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<string> AllowedOrigins { get; set; }

        /// <summary>
        /// A list specifying which operations the rule allows. At least one value must be specified.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<string> AllowedOperations { get; set; }

        /// <summary>
        /// If present this is a list of headers that are allowed in a pre-flight OPTIONS's request's Access-Control-Request-Headers header value. 
        /// </summary>
        public List<string> AllowedHeaders { get; set; }

        /// <summary>
        /// If present this is a list of headers that may be exposed to an application inside the client (eg. exposed to Javascript in a browser). 
        /// </summary>
        public List<string> ExposeHeaders { get; set; }

        /// <summary>
        /// This specifies the maximum number of seconds that a browser may cache the response to a preflight request. The value must not be negative and it must not be more than 86,400 seconds.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public uint MaxAgeSeconds { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as CorsRule);
        }

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

        public static bool operator ==(CorsRule rule1, CorsRule rule2)
        {
            return EqualityComparer<CorsRule>.Default.Equals(rule1, rule2);
        }

        public static bool operator !=(CorsRule rule1, CorsRule rule2)
        {
            return !(rule1 == rule2);
        }
    }
}
