using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BeanCacheWeb.Models
{

    [DataContract]
    public class CacheResponse
    {
        [DataMember]
        public List<Cache> CacheInfo { get; set; }

        public CacheResponse()
        {
            CacheInfo = new List<Cache>();
        }
    }

    [DataContract]
    public class Cache
    {
        [DataMember]
        public string key { get; set; }

        [DataMember]
        public string value { get; set; }

    }
}
