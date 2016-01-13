using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanCacheWeb.Controllers
{
    using BeanCache.Interface;
    using Microsoft.ServiceFabric.Services;
    using Microsoft.ServiceFabric.Services.Remoting.Client;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web.Http;

    [RoutePrefix("api")]
    public class BeanCacheController : ApiController
    {
        internal static Uri BeanCacheStatefulServiceName = new Uri("fabric:/BeanCacheApp/BeanCacheService");

        [HttpGet]
        public async Task<long> Set(string key, string value)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Determine the partition key that should handle the request
            long partitionKey = GetPartitionKey(key);

            var client = ServiceProxy.Create<IBeanCache>(partitionKey, BeanCacheStatefulServiceName);

            await client.SetAsync(key, value);
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        [HttpGet]
        public async Task<GetResponse> Get(string key)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Determine the partition key that should handle the request
            long partitionKey = GetPartitionKey(key);

            var client = ServiceProxy.Create<IBeanCache>(partitionKey, BeanCacheStatefulServiceName);

            var val = await client.GetAsync(key);
            sw.Stop();

            return new GetResponse()
            {
                TimeMilliseconds = sw.ElapsedMilliseconds,
                Value = val
            };
        }

        [HttpGet]
        public async Task<long> Remove(string key)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Determine the partition key that should handle the request
            long partitionKey = GetPartitionKey(key);
            var client = ServiceProxy.Create<IBeanCache>(partitionKey, BeanCacheStatefulServiceName);

            await client.RemoveAsync(key);
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private static long GetPartitionKey(string key)
        {
            return Math.Abs(key.GetHashCode() % 255);
        }
    }

    public class GetResponse
    {
        public string Value;
        public long TimeMilliseconds;
    };
}
