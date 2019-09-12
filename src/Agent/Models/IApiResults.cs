using System.Net;
using System.Net.Http;

namespace Bytewizer.Backblaze.Models
{
    /// <summary>
    /// An interface for <see cref="ApiResults{T}"/>.
    /// </summary>
    public interface IApiResults<T>
    {
        /// <summary>
        /// The HTTP response message.
        /// </summary>
        HttpResponseMessage HttpResponse { get; }

        /// <summary>
        /// The parsed HTTP body data.
        /// </summary>
        T Response { get; }

        /// <summary>
        /// The parsed API error data.
        /// </summary>
        ErrorResponse Error { get; }

        /// <summary>
        /// The status code of the HTTP response.
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// A value that indicates if the HTTP response was successful.
        /// </summary>
        bool IsSuccessStatusCode { get; }

        /// <summary>
        /// Throws an exception if the <see cref="IsSuccessStatusCode"/> property for the HTTP response is <c>false</c>"/>.
        /// </summary>
        IApiResults<T> EnsureSuccessStatusCode();
    }
}