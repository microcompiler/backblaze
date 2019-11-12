using System;
using System.Threading.Tasks;
using System.Security.Authentication;

using Bytewizer.Backblaze.Models;
using System.Collections.Generic;

namespace Bytewizer.Backblaze.Client
{
    /// <summary>
    /// An interface for <see cref="Storage"/>.
    /// </summary>
    public interface IStorageBuckets
    {
        #region ApiClient

        /// <summary>
        /// Creates a new bucket. 
        /// </summary>
        /// <param name="bucketName">The name to give the new bucket.</param>
        /// <param name="bucketType">The bucket secuirty authorization type.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CreateBucketResponse>> CreateAsync(string bucketName, BucketType bucketType);

        /// <summary>
        /// Creates a new bucket. 
        /// </summary>
        /// <param name="request">The <see cref="CreateBucketRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<CreateBucketResponse>> CreateAsync(CreateBucketRequest request);

        /// <summary>
        /// Deletes the bucket id specified. Only buckets that contain no version of any files can be deleted. 
        /// </summary>
        /// <param name="bucketId">The buckete id to delete.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<DeleteBucketResponse>> DeleteAsync(string bucketId);

        /// <summary>
        /// List all buckets in alphabetical order by bucket name.
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListBucketsResponse>> ListAsync();

        /// <summary>
        /// List all buckets associated with an account in alphabetical order by bucket name. When using an authorization token
        /// that is restricted to a bucket you must include the <see cref="ListBucketsRequest.BucketId"/>
        /// or <see cref="ListBucketsRequest.BucketName"/> of that bucket in the request or the request will be denied. 
        /// </summary>
        /// <param name="request">The <see cref="ListBucketsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<ListBucketsResponse>> ListAsync(ListBucketsRequest request, TimeSpan cacheTTL = default);

        /// <summary>
        /// Update an existing bucket. 
        /// </summary>
        /// <param name="bucketId">The buckete id to update.</param>
        /// <param name="bucketType">The bucket secuirty authorization type.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UpdateBucketResponse>> UpdateAsync(string bucketId, BucketType bucketType);

        /// <summary>
        /// Update an existing bucket. 
        /// </summary>
        /// <param name="request">The <see cref="UpdateBucketRequest"/> to send.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IApiResults<UpdateBucketResponse>> UpdateAsync(UpdateBucketRequest request);

        #endregion

        /// <summary>
        /// Gets all buckets associated with an account in alphabetical order by bucket name.
        /// </summary>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IEnumerable<BucketItem>> GetAsync();

        /// <summary>
        /// Gets all buckets associated with an account in alphabetical order by bucket name. When using an authorization token
        /// that is restricted to a bucket you must include the <see cref="ListBucketsRequest.BucketId"/>
        /// or <see cref="ListBucketsRequest.BucketName"/> of that bucket in the request or the request will be denied. 
        /// </summary>
        /// <param name="request">The <see cref="ListBucketsRequest"/> to send.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<IEnumerable<BucketItem>> GetAsync(ListBucketsRequest request, TimeSpan cacheTTL = default);

        /// <summary>
        /// Finds a bucket by id.
        /// </summary>
        /// <param name="bucketId">The bucket id to retrive.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<BucketItem> FindByIdAsync(string bucketId, TimeSpan cacheTTL = default);

        /// <summary>
        /// Finds a bucket by name.
        /// </summary>
        /// <param name="bucketName">The bucket name to retrive.</param>
        /// <param name="cacheTTL">An absolute cache expiration time to live (TTL) relative to now.</param>
        /// <exception cref="AuthenticationException">Thrown when authentication fails.</exception>
        /// <exception cref="ApiException">Thrown when an error occurs during client operation.</exception>
        Task<BucketItem> FindByNameAsync(string bucketName, TimeSpan cacheTTL = default);
    }
}
