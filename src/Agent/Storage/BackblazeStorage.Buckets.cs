using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Authentication;

using Bytewizer.Backblaze.Client;
using Bytewizer.Backblaze.Models;
using System.Linq;

namespace Bytewizer.Backblaze.Storage
{
    /// <summary>
    /// Represents a default implementation of the <see cref="BackblazeStorage"/> which uses <see cref="ApiClient"/> for making HTTP requests.
    /// </summary>
    public partial class BackblazeStorage : IBackblazeBuckets
    {
        public IBackblazeBuckets Buckets { get { return this; } }

        #region ApiClient

        /// <summary>
        /// Creates a new bucket. 
        /// </summary>
        /// <param name="bucketName">The name to give the new bucket.
        /// <param name="bucketType">The bucket secuirty authorization type.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CreateBucketResponse>> IBackblazeBuckets.CreateAsync
            (string bucketName, BucketType bucketType)
        {
            var request = new CreateBucketRequest(AccountId, bucketName, bucketType);
            return await _client.CreateBucketAsync(request, cancellationToken);
        }

        /// <summary>
        /// Creates a new bucket. 
        /// </summary>
        /// <param name="request">The <see cref="CreateBucketRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<CreateBucketResponse>> IBackblazeBuckets.CreateAsync
            (CreateBucketRequest request)
        {
            return await _client.CreateBucketAsync(request, cancellationToken);
        }

        /// <summary>
        /// Deletes the bucket specified. Only buckets that contain no version of any files can be deleted. 
        /// </summary>
        /// <param name="bucketId">The buckete id to delete.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<DeleteBucketResponse>> IBackblazeBuckets.DeleteAsync(string bucketId)
        {
            var request = new DeleteBucketRequest(AccountId, bucketId);
            return await _client.DeleteBucketAsync(request, cancellationToken);
        }

        /// <summary>
        /// List all buckets in alphabetical order by bucket name.
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListBucketsResponse>> IBackblazeBuckets.ListAsync()
        {
            var request = new ListBucketsRequest(AccountId);
            return await _client.ListBucketsAsync(request, cancellationToken);
        }

        /// <summary>
        /// List all buckets associated with an account in alphabetical order by bucket name. When using an authorization token
        /// that is restricted to a bucket you must include the <see cref="ListBucketsRequest.BucketId"/>
        /// or <see cref="ListBucketsRequest.BucketName"/> of that bucket in the request or the request will be denied. 
        /// </summary>
        /// <param name="request">The <see cref="ListBucketsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<ListBucketsResponse>> IBackblazeBuckets.ListAsync(ListBucketsRequest request, int cacheTTL)
        {
            return await _client.ListBucketsAsync(request, cacheTTL, cancellationToken);
        }

        /// <summary>
        /// Update an existing bucket. 
        /// </summary>
        /// <param name="bucketId">The buckete id to update.</param>
        /// <param name="bucketType">The bucket secuirty authorization type.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<UpdateBucketResponse>> IBackblazeBuckets.UpdateAsync
            (string bucketId, BucketType bucketType)
        {
            var request = new UpdateBucketRequest(AccountId, bucketId, bucketType);
            return await _client.UpdateBucketAsync(request, cancellationToken);
        }

        /// <summary>
        /// Update an existing bucket. 
        /// </summary>
        /// <param name="request">The <see cref="UpdateBucketRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IApiResults<UpdateBucketResponse>> IBackblazeBuckets.UpdateAsync
            (UpdateBucketRequest request)
        {
            return await _client.UpdateBucketAsync(request, cancellationToken);
        }

        #endregion

        /// <summary>
        /// Gets all buckets associated with an account in alphabetical order by bucket name. 
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<BucketItem>> IBackblazeBuckets.GetAsync()
        {
            var request = new ListBucketsRequest(AccountId);
            return await Buckets.GetAsync(request, 0);
        }

        /// <summary>
        /// Gets all buckets associated with an account in alphabetical order by bucket name. When using an authorization token
        /// that is restricted to a bucket you must include the <see cref="ListBucketsRequest.BucketId"/>
        /// or <see cref="ListBucketsRequest.BucketName"/> of that bucket in the request or the request will be denied. 
        /// </summary>
        /// <param name="request">The <see cref="ListBucketsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<IEnumerable<BucketItem>> IBackblazeBuckets.GetAsync(ListBucketsRequest request, int cacheTTL)
        {
            var results = await _client.ListBucketsAsync(request, cacheTTL, cancellationToken);
            if (results.IsSuccessStatusCode)
            {
                return results.Response.Buckets;
            }

            return new List<BucketItem>();
        }

        /// <summary>
        /// Finds bucket by id.
        /// </summary>
        /// <param name="bucketId">The bucket id to retrive.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<BucketItem> IBackblazeBuckets.FindByIdAsync(string bucketId, int cacheTTL)
        {
            var request = new ListBucketsRequest(AccountId) { BucketId = bucketId };
            var results = await _client.ListBucketsAsync(request, cacheTTL, cancellationToken);
            if (results.IsSuccessStatusCode & results.Response.Buckets.Count > 0)
            {
                return results.Response.Buckets.First();
            }

            return null;
        }

        /// <summary>
        /// Find bucket by name.
        /// </summary>
        /// <param name="bucketName">The bucket id to retrive.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now in seconds.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        async Task<BucketItem> IBackblazeBuckets.FindByNameAsync(string bucketName, int cacheTTL)
        {
            var request = new ListBucketsRequest(AccountId) { BucketName = bucketName };
            var results = await _client.ListBucketsAsync(request, cacheTTL, cancellationToken);
            if (results.IsSuccessStatusCode & results.Response.Buckets.Count > 0)
            {
                return results.Response.Buckets.First();
            }

            return null;
        }
    }
}
