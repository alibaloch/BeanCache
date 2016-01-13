using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BeanCacheWeb.Models
{
    [DataContract]
    public class HitsCountResponse
    {
        [DataMember]
        public long Total { get; set; }

        [DataMember]
        public List<CacheInfo> CacheInfo { get; set; }

        public HitsCountResponse()
        {
            CacheInfo = new List<Models.CacheInfo>();
        }
    }

    [DataContract]
    public class CacheInfo
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public long Hits { get; set; }

        [DataMember]
        public long LowKey{ get; set; }

        [DataMember]
        public long HighKey { get; set; }

    }
}
