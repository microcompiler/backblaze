using System;
using System.Runtime.Serialization;

namespace Bytewizer.Backblaze
{
    /// <summary>
    /// The exception that is thrown when a configuration is not valid.
    /// </summary>
    [Serializable]
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Get or set the name of the configuration that causes this exception
        /// </summary>
        public string ConfigurationName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class
        /// </summary>
        public ConfigurationException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ConfigurationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="configurationName">The name of the configuration that caused the current exception.</param>
        public ConfigurationException(string message, string configurationName)
           : base(message)
        {
            ConfigurationName = configurationName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class 
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter
        /// is not a null reference, the current exception is raised in a catch block that handles the inner exception. </param>
        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class
        /// with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data. </param>
        /// <param name="context">The contextual information about the source or destination. </param>
        protected ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
