using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ServiceFabric.Services;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Query;
using BeanCache.Interface;
using System.Collections.Concurrent;
using System.Text;
using BeanCacheWeb.Models;
using Newtonsoft.Json;
using System.Net;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace BeanCacheWeb.Controllers
{
    public class CacheManagementController : ApiController
    {
        private BeanCacheController beanCacheController = new BeanCacheController();

        [HttpGet]
        public async Task<HttpResponseMessage> Generate(int numberOfRecords)
        {
            for (int i = 0; i < numberOfRecords; i++)
            {
                string key = GetRandomString(4);
                string value = GetRandomString(7);

                await beanCacheController.Set(key, value);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, "Cache Generated Successfully.");
            return response;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Get(int numberOfRecords)
        {
            var response = new CacheResponse {
                CacheInfo = new System.Collections.Generic.List<Cache>()
            };

            for (int i = 0; i < numberOfRecords; i++)
            {
                var res = await beanCacheController.Get(i.ToString());
                Cache cache = new Cache
                {
                    key = i.ToString(),
                    value = res.Value
                };

                response.CacheInfo.Add(cache);
            }

            var jsonContent = JsonConvert.SerializeObject(response);
            return new HttpResponseMessage() { Content = new StringContent(jsonContent, Encoding.UTF8, "application/json") };
        }

       
        private static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var r = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[r.Next(s.Length)]).ToArray());
        }



        [HttpGet]
        public async Task<HitsCountResponse> GetHitsCount()
        {
            // Get the list of representative service partition clients.
            var partitionClients = await this.GetServicePartitionClientsAsync();

            // For each partition client, keep track of partition information and the number of words
            var totals = new ConcurrentDictionary<Int64RangePartitionInformation, Task<long>>();
            IList<Task> tasks = new List<Task>(partitionClients.Count);
            foreach (var partitionClient in partitionClients)
            {
                // partitionClient internally resolves the address and retries on transient errors based on the configured retry policy.
                Task<long> tt = partitionClient.beanCache.GetHitsCount();
                tasks.Add(tt);
                totals[partitionClient.part as Int64RangePartitionInformation] = tt;
            }
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                // Sample code: print exception
                ServiceEventSource.Current.Message(ex.Message, "Count - run web request");
                throw ex;
            }

            var response = new HitsCountResponse();
            response.Total = totals.Aggregate(0, (total, next) => (int)next.Value.Result + total);
 
            foreach (var partitionData in totals.OrderBy(partitionData => partitionData.Key.LowKey))
            {
                var cachInfo = new CacheInfo();

                cachInfo.Id = partitionData.Key.Id;
                cachInfo.LowKey = partitionData.Key.LowKey;
                cachInfo.HighKey = partitionData.Key.HighKey;
                cachInfo.Hits = partitionData.Value.Result;

                response.CacheInfo.Add(cachInfo);
            }
            return response;
        }

        /// <summary>
        /// Returns a list of service partition clients pointing to one key in each of the WordCount service partitions.
        /// The returned representative key is the min key served by each partition.
        /// </summary>
        /// <returns>The service partition clients pointing at a key in each of the WordCount service partitions.</returns>
        private async Task<IList<BeanPartInfo>> GetServicePartitionClientsAsync()
        {
            for (int i = 0; i < MaxQueryRetryCount; i++)
            {
                try
                {
                    // Get the list of partitions up and running in the service.
                    ServicePartitionList partitionList =
                        await fabricClient.QueryManager.GetPartitionListAsync(BeanCacheController.BeanCacheStatefulServiceName);

                    // For each partition, build a service partition client used to resolve the low key served by the partition.
                    IList<BeanPartInfo> partitionClients = new List<BeanPartInfo>(partitionList.Count);
                    foreach (Partition partition in partitionList)
                    {
                        Int64RangePartitionInformation partitionInfo = partition.PartitionInformation as Int64RangePartitionInformation;
                        if (partitionInfo == null)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    "The service {0} should have a uniform Int64 partition. Instead: {1}",
                                    BeanCacheController.BeanCacheStatefulServiceName,
                                    partition.PartitionInformation.Kind));
                        }

                        partitionClients.Add( new BeanPartInfo()
                        {
                            part = partitionInfo, 
                            beanCache = ServiceProxy.Create<IBeanCache>(partitionInfo.LowKey, BeanCacheController.BeanCacheStatefulServiceName)
                        });
                    }

                    return partitionClients;
                }
                catch (FabricTransientException ex)
                {
                    ServiceEventSource.Current.Message(ex.Message, "create representative partition clients");
                    if (i == MaxQueryRetryCount - 1)
                    {
                        throw;
                    }
                }

                await Task.Delay(BackoffQueryDelay);
            }

            throw new TimeoutException("Retry timeout is exhausted and creating representative partition clients wasn't successful");
        }
        private const int MaxQueryRetryCount = 20;
        private static TimeSpan BackoffQueryDelay = TimeSpan.FromSeconds(3);
        private static FabricClient fabricClient = new FabricClient();
    }
    class BeanPartInfo
    {
        public Int64RangePartitionInformation part { set; get; }
        public IBeanCache beanCache { set; get; }
    }

}
