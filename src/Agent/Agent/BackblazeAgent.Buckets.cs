using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bytewizer.Backblaze.Models;

namespace Bytewizer.Backblaze.Agent
{
    public partial class BackblazeAgent : IBackblazeBucketsAgent
    {
        public IBackblazeBucketsAgent Buckets { get { return this; } }

        async Task<BucketItem> IBackblazeBucketsAgent.FirstAsync()
        {
            var request = new ListBucketsRequest(AccountId);
            var buckets = await _client.ListBucketsAsync(request, cancellationToken);
            buckets.EnsureSuccessStatusCode();

            return buckets.Response.Buckets.First();
        }

        async Task<BucketItem> IBackblazeBucketsAgent.FirstAsync(Func<BucketItem, bool> predicate)
        {
            var request = new ListBucketsRequest(AccountId);
            var buckets = await  _client.ListBucketsAsync(request, cancellationToken);
            buckets.EnsureSuccessStatusCode();

            return buckets.Response.Buckets.First(predicate);
        }

        async Task<List<BucketItem>> IBackblazeBucketsAgent.ListAsync(ListBucketsRequest request, int cacheTTL)
        {
            var buckets = await  _client.ListBucketsAsync(request, cacheTTL, cancellationToken);
            buckets.EnsureSuccessStatusCode();

            return buckets.Response.Buckets;
        }

        async Task<IApiResults<ListBucketsResponse>> IBackblazeBucketsAgent.GetAsync()
        {
            var request = new ListBucketsRequest(AccountId);
            return await  _client.ListBucketsAsync(request, cancellationToken);
        }

        async Task<IApiResults<ListBucketsResponse>> IBackblazeBucketsAgent.GetAsync(ListBucketsRequest request)
        {
            return await _client.ListBucketsAsync(request, cancellationToken);
        }

        async Task<IApiResults<ListBucketsResponse>> IBackblazeBucketsAgent.GetAsync(string accountId)
        {
            var request = new ListBucketsRequest(accountId);
            return await _client.ListBucketsAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateBucketResponse>> IBackblazeBucketsAgent.CreateAsync
            (CreateBucketRequest request)
        {
            return await _client.CreateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateBucketResponse>> IBackblazeBucketsAgent.CreateAsync
            (string bucketName, BucketType bucketType)
        {
            var request = new CreateBucketRequest(AccountId, bucketName, bucketType);
            return await _client.CreateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<CreateBucketResponse>> IBackblazeBucketsAgent.CreateAsync
            (string accountId, string bucketName, BucketType bucketType)
        {
            var request = new CreateBucketRequest(AccountId, bucketName, bucketType);
            return await _client.CreateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<UpdateBucketResponse>> IBackblazeBucketsAgent.UpdateAsync
            (UpdateBucketRequest request)
        {
            return await _client.UpdateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<UpdateBucketResponse>> IBackblazeBucketsAgent.UpdateAsync
            (string bucketId, BucketType bucketType)
        {
            var request = new UpdateBucketRequest(AccountId, bucketId, bucketType);
            return await _client.UpdateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<UpdateBucketResponse>> IBackblazeBucketsAgent.UpdateAsync
            (string accountId, string bucketId, BucketType bucketType)
        {
            var request = new UpdateBucketRequest(AccountId, bucketId, bucketType);
            return await _client.UpdateBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<DeleteBucketResponse>> IBackblazeBucketsAgent.DeleteAsync(string bucketId)
        {
            var request = new DeleteBucketRequest(AccountId, bucketId);
            return await _client.DeleteBucketAsync(request, cancellationToken);
        }

        async Task<IApiResults<DeleteBucketResponse>> IBackblazeBucketsAgent.DeleteAsync(string accountId, string bucketId)
        {
            var request = new DeleteBucketRequest(accountId, bucketId);
            return await _client.DeleteBucketAsync(request, cancellationToken);
        }
    }
}
