using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using BeanCache.Interface;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System.Collections.Generic;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace BeanCacheService
{
    public class BeanCacheService : StatefulService, IBeanCache
    {
        private readonly string BeanCacheDictionaryName = "BeanCacheDictionary";
        private readonly string StatisticsDictionaryName = "BeanStatisticsDictionary";

        public async Task<string> GetAsync(string key)
        {
            try
            {
                using (var tx = this.StateManager.CreateTransaction())
                {
                    var beanCache = await GetBeanCache();
                    var result = await beanCache.TryGetValueAsync(tx, key);

                    await AddHits();
                    return result.HasValue ? result.Value : "";
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(this, ex.ToString());
                throw ex;
            }
        }

        public async Task SetAsync(string key, string value)
        {
            try {
                using (var tx = this.StateManager.CreateTransaction())
                {
                    var beanCache = await GetBeanCache();

                    await beanCache.AddOrUpdateAsync(tx, key, value, (k, v) => v = value);
                    await tx.CommitAsync();
                }
            }
            catch(Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(this, ex.ToString());
                throw ex;
            }

            await AddHits();
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                using (var tx = this.StateManager.CreateTransaction())
                {
                    var beanCache = await GetBeanCache();

                    await beanCache.TryRemoveAsync(tx, key);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(this, ex.ToString());
                throw;
            }

            await AddHits();
        }

        public Task<long> GetHitsCount()
        {
            return AddHits(0);
        }

        private async Task<long> AddHits(int count=1)
        {
            long numberOfProcessedWords = 0;

            var statsDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>(StatisticsDictionaryName);
            try
            {
                using (var tx = this.StateManager.CreateTransaction())
                {
                    numberOfProcessedWords = await statsDictionary.AddOrUpdateAsync(
                                tx,"NumberOfHits", 1,
                                (key, oldValue) => oldValue + count);
                    await tx.CommitAsync();
                }
            }
            catch (TimeoutException ex)
            {
                //Service Fabric uses timeouts on collection operations to prevent deadlocks.
                //If this exception is thrown, it means that this transaction was waiting the default
                //amount of time (4 seconds) but was unable to acquire the lock. In this case we simply
                //retry after a random backoff interval. You can also control the timeout via a parameter
                //on the collection operation.
                Thread.Sleep(TimeSpan.FromSeconds(new Random().Next(100, 300)));
                throw ex;
            }
            catch (Exception exception)
            {
                //For sample code only: simply trace the exception.
                ServiceEventSource.Current.Message(exception.ToString());
                throw exception;
            }

            return numberOfProcessedWords;
        }

        private Task<IReliableDictionary<string,string>> GetBeanCache()
        {
            return this.StateManager.GetOrAddAsync<IReliableDictionary<string, string>>(BeanCacheDictionaryName);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(parameters => 
                new ServiceRemotingListener<IBeanCache>(parameters, this)) };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var tx = this.StateManager.CreateTransaction())
                {
                }
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
