<properties
   pageTitle="BeanCache"
   description="Simple cache using Service Fabric"
   services="service-fabric"
   documentationCenter=".net"
   authors="Ali Baloch, Praveen Veerath"
   manager=""
   editor=""/>

#BeanCache - A reliable cache using Service Fabric / Microservices
Ali Baloch and Praveen Veerath.
12/18/2014


##Prerequisite
This article assumes that you are familiar with:

- Azure Service Fabric
- Microservices
- ASP .NET MVC / WebAPI

##Introduction
BeanCache is build using [Service Fabric](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-overview/), which is a distributed systems platform used to build scalable, reliable, and easily-managed applications for the cloud.

BeanCache is a distributed cache which is composed of two [microservices](https://msdn.microsoft.com/en-us/magazine/mt595752.aspx). In a microservices, an application logical units are segregated into small autonomous services, which can scale out independently across VMs/containers, whereas, in a more traditional application, most of application functional units reside in same process address space and/or do not scale independently.

BeanCache is made up of two microservices build using Service Fabric:

- BeanCache Web
- BeanCache Service

## Architecture

![BeanCache Architecture](https://raw.githubusercontent.com/alibaloch/BeanCache/master/images/Architecture.png)

###BeanCache Web
BeanCache Web is self-hosted [Stateless Reliable Services built using OWIN](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-communication-webapi/). It exposes following interface using WebAPI to get/set/remove values from cache. GetHitsCount give information about how many cached items are in each [service fabric partition](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-concepts-partitioning/).


	public interface IBeanCache : IService
	{
		Task SetAsync(string key, string value);
		Task<string>
		GetAsync(string key);
		Task RemoveAsync(string key);
		Task<long>
		GetHitsCount();
	}

The MVC Web API controller looks like:

	public class BeanCacheController : ApiController
	{
		internal static Uri BeanCacheStatefulServiceName = new Uri("fabric:/BeanCacheApp/BeanCacheService");
		[HttpGet]
		public async Task<long>
		Set(string key, string value)
		{
			...
			long partitionKey = GetPartitionKey(key);
			var client = ServiceProxy.Create<IBeanCache>
			(partitionKey, BeanCacheStatefulServiceName);
			await client.SetAsync(key, value);
			...
		}
		
		[HttpGet]
		public async Task<GetResponse>
		Get(string key)
		{
			...
		}
	...
	}


###BeanCache Service
BeanCache Service is a Stateful service which stores <key, value=""> pairs in [Reliable dictionary collection](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reliable-services-reliable-collections/). The BeanCache Web service call BeanCache Service to get/set value in the reliable dictionary.

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
		...
	}


### Dashboard
Dashboard is set of static HTML and Javascript files build using AngularJS and exposed via BeanCache Web.

## Screenshots
![BeanCache Architecture](https://raw.githubusercontent.com/alibaloch/BeanCache/master/images/Dashboard2.png)
![BeanCache Architecture](https://raw.githubusercontent.com/alibaloch/BeanCache/master/images/Dashboard1.png)
## Disclaimer
The purpose of this sample program using Service Fabric is to understand the underpinning of Service Fabric and to build an end to end application. The goal is not to compete/replace existing caching services. 

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
