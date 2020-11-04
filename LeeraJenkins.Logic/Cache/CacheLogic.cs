using System.Runtime.Caching;

namespace LeeraJenkins.Logic.Cache
{
    public class CacheLogic : ICacheLogic
    {
        private ObjectCache _cache;

        public CacheLogic()
        {
            _cache = MemoryCache.Default;
        }

        public bool IsExists(string key)
        {
            return _cache.Contains(key);
        }

        public string Get(string key)
        {
            return (string)_cache.Get(key);
        }

        public void Set(string key, string obj)
        {
            var policy = new CacheItemPolicy();
            //policy.ChangeMonitors.;

            _cache.Set(key, obj, policy);
        }
    }
}
