using Microsoft.Extensions.Caching.Memory;

namespace ELOR.Razzle.Services.Infrastructure
{
    // Stores responses of completed write operations keyed by user + method + idempotency key
    public sealed class IdempotencyStore
    {
        private static readonly TimeSpan Retention = TimeSpan.FromHours(1);
        private readonly IMemoryCache _cache;

        public IdempotencyStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool TryGet(string key, out object value) => _cache.TryGetValue(key, out value);

        public void Set(string key, object value) => _cache.Set(key, value, Retention);
    }
}
