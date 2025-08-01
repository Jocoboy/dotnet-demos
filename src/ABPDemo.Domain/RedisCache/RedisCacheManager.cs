using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace ABPDemo.RedisCache
{
    public class RedisCacheManager :  ITransientDependency
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisCacheManager(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task ClearByPrefixAsync(string keyPrefix)
        {
            var db = _redis.GetDatabase();
            //var server = _redis.GetServer(_redis.GetEndPoints().First());

            //var keys = new List<RedisKey>();
            //await foreach (var key in server.KeysAsync(pattern: $"{keyPrefix}*"))
            //{
            //    keys.Add(key);
            //}

            // 批量删除
            //if (keys.Count != 0)
            //{
            //    await db.KeyDeleteAsync(keys.ToArray());
            //}

            #region 使用 SCAN 迭代查询（避免 KEYS 阻塞）
            var script = @"
                                    local keys = redis.call('SCAN', 0, 'MATCH', ARGV[1], 'COUNT', 1000)
                                    for i, key in ipairs(keys[2]) do
                                        redis.call('DEL', key)
                                    end
                                    return keys[1]";

            try
            {
                // 递归扫描直到返回的游标为 0
                long cursor;
                do
                {
                    var result = (RedisResult[])await db.ScriptEvaluateAsync(script, values: new RedisValue[] { $"{keyPrefix}*" });

                    cursor = (long)result[0];
                } while (cursor != 0);
            }
            catch { }
            #endregion
        }
    }
}
