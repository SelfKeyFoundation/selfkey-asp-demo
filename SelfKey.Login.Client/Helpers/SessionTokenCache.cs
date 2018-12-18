using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;

namespace SelfKey.Login.Client.Helpers
{
    /// <summary>
    /// Store the user's token information.
    /// </summary>
    public class SessionTokenCache
    {
        private static readonly object FileLock = new object();
        private readonly string _cacheId;
        private readonly IMemoryCache _memoryCache;
        private TokenCache _cache = new TokenCache();

        public SessionTokenCache(string userId, IMemoryCache memoryCache)
        {
            // Not object, we want the SUB.
            _cacheId = userId + "_TokenCache";
            _memoryCache = memoryCache;

            Load();
        }

        public TokenCache GetCacheInstance()
        {
            _cache.SetBeforeAccess(BeforeAccessNotification);
            _cache.SetAfterAccess(AfterAccessNotification);
            Load();

            return _cache;
        }

        public void SaveUserStateValue(string state)
        {
            lock (FileLock)
            {
                _memoryCache.Set(_cacheId + "_state", Encoding.ASCII.GetBytes(state));
            }
        }

        public string ReadUserStateValue()
        {
            lock (FileLock)
            {
                return Encoding.ASCII.GetString(_memoryCache.Get(_cacheId + "_state") as byte[]);
            }
        }

        // Empties the persistent store.
        public void Clear()
        {
            _cache = null;
            lock (FileLock)
            {
                _memoryCache.Remove(_cacheId);
            }
        }

        // Triggered right before MSAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load();
        }

        private void Load()
        {
            lock (FileLock)
            {
                _cache.Deserialize(_memoryCache.Get(_cacheId) as byte[]);
            }
        }

        // Triggered right after MSAL accessed the cache.
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // If the access operation resulted in a cache update.
            if (args.HasStateChanged)
            {
                lock (FileLock)
                {
                    // Reflect changes in the persistent store.
                    _memoryCache.Set(_cacheId, _cache.Serialize());
                }
            }
        }
    }
}
