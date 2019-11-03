using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;

using Microsoft.Extensions.Logging;

namespace Bytewizer.Backblaze.Enumerables
{
    /// <summary>
    /// Iterates sequentially through the response item elements.
    /// </summary>
    /// <typeparam name="T">The response item class related to a request.</typeparam>
    public abstract class BaseIterator<T> : IEnumerable<T> where T : IItem
    {
        /// <summary>
        /// Connected client to Backblaze B2 Cloud Storage.
        /// </summary>
        protected readonly IApiClient _client;

        /// <summary>
        /// Application logging
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// An absolute cache expiration time to live (TTL) relative to now.
        /// </summary>
        protected readonly TimeSpan _cacheTTL;

        /// <summary>
        /// The cancellation token to cancel operation.
        /// </summary>
        protected readonly CancellationToken _cancellationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseIterator{T}"/> class.
        /// </summary>
        protected BaseIterator(IApiClient client, ILogger logger, TimeSpan cacheTTL, CancellationToken cancellationToken)
        {
            _client = client;
            _logger = logger;
            _cacheTTL = cacheTTL;
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Returns the next iterator until completed.
        /// </summary>
        protected abstract List<T> GetNextPage(out bool isCompleted);

        /// <summary>
        /// Returns an enumerator that iterates through the collection>.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            bool isCompleted;
            do
            {
                List<T> page = GetNextPage(out isCompleted);

                foreach (T item in page)
                    yield return item;

                if (!page.Any())
                    yield break;
            } while (!isCompleted);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
