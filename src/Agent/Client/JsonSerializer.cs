using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// Serializes and deserializes objects into and from the Json format using the <see cref="JsonConvert"/>.
    /// </summary>
    public class JsonSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
        /// </summary>
        public JsonSerializer()
        {
            JsonSettings = new JsonSerializerSettings();
            JsonSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            JsonSettings.Converters.Add(new UnixTimeConverter());
            JsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            JsonSettings.NullValueHandling = NullValueHandling.Ignore;
            JsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            JsonSettings.Formatting = Formatting.Indented;
        }

        /// <summary>
        /// Gets the instance of JsonSerializerSettings.
        /// </summary>
        public JsonSerializerSettings JsonSettings { get; }

        /// <summary>
        /// Deserializes the Json string to an object.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="value">Json string to deserialize.</param>
        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, JsonSettings);
        }

        /// <summary>
        /// Serializes the object to a Json string. 
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="value">Object to deserialize.</param>
        public string SerializeObject<T>(T value)
        {
            return JsonConvert.SerializeObject(value, JsonSettings);
        }
    }
}
