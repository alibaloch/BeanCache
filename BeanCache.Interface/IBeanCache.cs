using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace BeanCache.Interface
{
    public interface IBeanCache : IService
    {
        Task SetAsync(string key, string value);
        Task<string> GetAsync(string key);
        Task RemoveAsync(string key);
        Task<long> GetHitsCount();
    }
}
