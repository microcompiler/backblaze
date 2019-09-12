using System;
using System.Net;
using System.Net.Http;
using System.Diagnostics;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    ///  Represents a response from the Backblaze B2 Cloud Storage service.
    /// </summary>
    static class ApiResults
    {
        internal static T Create<T>(HttpResponseMessage response, object content, ErrorResponse error = null)
        {
            return (T)Activator.CreateInstance(typeof(T), response, content, error);
        }
    }

    /// <summary>
    /// Represents a response from the Backblaze B2 Cloud Storage service.
    /// </summary>
    /// <typeparam name="T">Payload contained in the response</typeparam>
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    public sealed class ApiResults<T> : DisposableObject, IApiResults<T>
    {
        #region Lifetime

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResults{T}." /> class.
        /// </summary>
        /// <param name="response">Http response message.</param>
        public ApiResults(HttpResponseMessage response)
            : this(response, GetBodyAsObject(response))
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResults{T}." /> class.
        /// </summary>
        /// <param name="response">Http response message.</param>
        public ApiResults(HttpResponseMessage response, T content)
        {
            HttpResponse = response ?? throw new ArgumentNullException(nameof(response));
            Response = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResults{T}." /> class.
        /// </summary>
        /// <param name="response">Http response message.</param>
        public ApiResults(HttpResponseMessage response, ErrorResponse error)
        {
            HttpResponse = response ?? throw new ArgumentNullException(nameof(response));
            Error = error;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResults{T}." /> class.
        /// </summary>
        /// <param name="response">Http response message.</param>
        public ApiResults(HttpResponseMessage response, T content, ErrorResponse error)
        {
            HttpResponse = response ?? throw new ArgumentNullException(nameof(response));
            Response = content;
            Error = error;
        }

        /// <summary>
        /// <see cref="DisposableObject.Dispose(bool)"/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            HttpResponse.Dispose();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the HTTP response message.
        /// </summary>
        /// <value>The http response message.</value>
        public HttpResponseMessage HttpResponse { get; private set; }

        /// <summary>
        /// Gets the parsed HTTP body data.
        /// </summary>
        /// <value>The response data.</value>
        public T Response { get; private set; }

        /// <summary>
        /// Gets the parsed API error data.
        /// </summary>
        /// <value>The error response data.</value>
        public ErrorResponse Error { get; private set; }

        /// <summary>
        /// Gets the status code of the HTTP response.
        /// </summary>
        /// <value>The http response message status code.</value>
        public HttpStatusCode StatusCode { get { return HttpResponse.StatusCode; } }

        /// <summary>
        /// A value that indicates if the HTTP response was successful.
        /// </summary>
        public bool IsSuccessStatusCode { get { return HttpResponse.IsSuccessStatusCode; } }

        /// <summary>
        /// Throws an exception if the <see cref="IsSuccessStatusCode"/> property for the HTTP response is <see cref="false"/>.
        /// </summary>
        public IApiResults<T> EnsureSuccessStatusCode()
        {
            if (IsSuccessStatusCode)
                return this;

            throw new ApiException(StatusCode, Error);
        }

        ///	<summary>
        ///	Debugger display for this object.
        ///	</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                string message = string.Empty;
                if (Error != null)
                    message = $", {nameof(Error.Message)}: {Error.Message}";

                if (Response != null)
                    message = $", ResponseType: {Response.GetType()}";

                return $"{{{nameof(StatusCode)}: {StatusCode.ToString()}{message}}}";
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response">Http response message.</param>
        private static T GetBodyAsObject(HttpResponseMessage response)
        {
            object body = response.Content;
            if (body is T) return (T)body;
            return default;
        }

        #endregion
    }
}
