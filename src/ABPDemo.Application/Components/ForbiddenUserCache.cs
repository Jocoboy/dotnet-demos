using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace ABPDemo.Components
{
    /// <summary>
    /// 用户缓存禁用组件
    /// </summary>
    public class ForbiddenUserCache : ITransientDependency
    {
        private readonly DistributedCacheEntryOptions _options;
        private readonly IDistributedCache<string, Guid> _distributedCache;

        public ForbiddenUserCache(IDistributedCache<string, Guid> distributedCache,
            IOptions<AuthOptions> authOptions)
        {
            _distributedCache = distributedCache;

            var expireTime = TimeSpan.FromMinutes(authOptions.Value.AccessExpiration);
            _options = new DistributedCacheEntryOptions();
            _options.SetAbsoluteExpiration(expireTime);
        }

        public async Task ForbiddenAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _distributedCache.SetAsync(userId, string.Empty, _options, token: cancellationToken);
        }

        public async Task CancelForbiddenAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(userId, token: cancellationToken);
        }

        public async Task<bool> IsUserForbiddenAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cache = await _distributedCache.GetAsync(userId, token: cancellationToken);

            return cache != null;
        }
    }
}
