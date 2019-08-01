using System.Net;
using System.Net.Http;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// An interface for <see cref="ApiResults"/>.
    /// </summary>
    public interface IApiResults<T>
    {
        /// <summary>
        /// The HTTP response message.
        /// </summary>
        /// <value>The http response message.</value>
        HttpResponseMessage HttpResponse { get; }

        /// <summary>
        /// The parsed HTTP body data.
        /// </summary>
        /// <value>The response data.</value>
        T Response { get; }

        /// <summary>
        /// The parsed API error data.
        /// </summary>
        /// <value>The error response data.</value>
        ErrorResponse Error { get; }

        /// <summary>
        /// The status code of the HTTP response.
        /// </summary>
        /// <value>The http response message status code.</value>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// A value that indicates if the HTTP response was successful.
        /// </summary>
        bool IsSuccessStatusCode { get; }
    }
}