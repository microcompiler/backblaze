using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Storage
{
    public partial class BackblazeStorage : IBackblazeBuckets
    {
        public IBackblazeBuckets Buckets { get { return this; } }

        async Task<BucketItem> IBackblazeBuckets.FirstAsync()
        {
            var request = new ListBucketsRequest(AccountId);
            var buckets = await _client.ListBucketsAsync(request, cancellationToken);
            buckets.EnsureSuccessStatusCode();

            return buckets.Response.Buckets.First();
        }

        async Task<BucketItem> IBackblazeBuckets.FirstAsync(Func<BucketItem, bool> predicate)
        {
            var request = new ListBucketsRequest(AccountId);
            var buckets = await  _client.ListBucketsAsync(request, cancellationToken);
            buckets.EnsureSuccessStatusCode();

            return buckets.Response.Buckets.First(predicate);
        }

        async Task<List<BucketItem>> IBackblazeBuckets.ListAsync(ListBucketsRequest request, int cacheTTL)
        {
            var buckets = await  _client.ListBucketsAsync(request, cacheTTL, cancellationToken);
            buckets.EnsureSuccessStatusCode();

            return buckets.Response.Buckets;
        }

        async Task<IApiResults<ListBucketsResponse>> IBackblazeBuckets.GetAsync()
        {
            var request = new ListBucketsRequest(AccountId);
            return await  _client.ListBucketsAsync(request, cancellationToken);
        }

        async Task<IApiResults<ListBucketsResponse>> IBackblazeBuckets.GetAsync(ListBucketsRequest request)
        {
            return await _client.ListBucketsAsync(request, cancellationToken);
        }

        async Task<IApiResults<ListBucketsResponse>> IBackblazeBuckets.GetAsync(string accountId)
        {
            var request = new ListBucketsRequest(accountId);
            return await _client.ListBucketsAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateBucketResponse>> IBackblazeBuckets.CreateAsync
            (CreateBucketRequest request)
        {
            return await _client.CreateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateBucketResponse>> IBackblazeBuckets.CreateAsync
            (string bucketName, BucketType bucketType)
        {
            var request = new CreateBucketRequest(AccountId, bucketName, bucketType);
            return await _client.CreateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateBucketResponse>> IBackblazeBuckets.CreateAsync
            (string accountId, string bucketName, BucketType bucketType)
        {
            var request = new CreateBucketRequest(AccountId, bucketName, bucketType);
            return await _client.CreateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<UpdateBucketResponse>> IBackblazeBuckets.UpdateAsync
            (UpdateBucketRequest request)
        {
            return await _client.UpdateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<UpdateBucketResponse>> IBackblazeBuckets.UpdateAsync
            (string bucketId, BucketType bucketType)
        {
            var request = new UpdateBucketRequest(AccountId, bucketId, bucketType);
            return await _client.UpdateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<UpdateBucketResponse>> IBackblazeBuckets.UpdateAsync
            (string accountId, string bucketId, BucketType bucketType)
        {
            var request = new UpdateBucketRequest(AccountId, bucketId, bucketType);
            return await _client.UpdateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<DeleteBucketResponse>> IBackblazeBuckets.DeleteAsync(string bucketId)
        {
            var request = new DeleteBucketRequest(AccountId, bucketId);
            return await _client.DeleteBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<DeleteBucketResponse>> IBackblazeBuckets.DeleteAsync(string accountId, string bucketId)
        {
            var request = new DeleteBucketRequest(accountId, bucketId);
            return await _client.DeleteBucketAsync(request, cancellationToken);
        }
    }
}
