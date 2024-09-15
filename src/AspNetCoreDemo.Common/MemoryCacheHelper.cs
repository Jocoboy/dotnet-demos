using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Xml.Serialization;

namespace AspNetCoreDemo.Common
{
    /// <summary>
    /// MemoryCache缓存帮助类
    /// </summary>
    public class MemoryCacheHelper
    {
        private readonly static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private static CancellationTokenSource _clearToken = new CancellationTokenSource();

        /// <summary>
        /// 验证码
        /// </summary> 
        /// <returns></returns>
        public static MemoryStream ValCode(string key)
        {
            MemoryStream ms = CommonHelper.CreateValiCode(out string code);
            code = code.ToLower();//验证码不分大小写//缓存验证码
            Set(key, code);
            return ms;
        }

        /// <summary>
        /// 生成随机手机验证码
        /// </summary>
        /// <param name="key"></param>
        public static string RandValCode(string key)
        {
            var valcode = CommonHelper.RndomStr(6);
            Set(key, valcode);
            return valcode;
        }

        /// <summary>
        /// 校验验证码
        /// </summary>
        /// <param name="key"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool CheckCode(string key, string code)
        {
            var value = Exists(key) && Get<string>(key) == code ? true : false;
            Remove(key);
            return value;
        }

        /// <summary>
        /// 校验手机验证码
        /// </summary>
        /// <param name="key"></param>
        /// <param name="code"></param>
        ///// <returns></returns>
        public static bool PhoneCheckCode(string key, string code)
        {
            var value = Exists(key) && Get<string>(key) == code ? true : false;
            if (value)
            {
                Remove(key);
            }
            return value;
        }


        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire">过期时间，默认5分钟</param>
        public static void Set<T>(string key, T value, int expire = 5)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
            {
                return;
            }

            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expire);
            options.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));
            _cache.Set(key, value, options);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return default;
            }

            bool isExists = _cache.TryGetValue(key, out T value);
            if (isExists)
            {
                // 深拷贝缓存值，防止使用缓存时直接更改缓存值
                return CommonHelper.DeepCopyByXml(value);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 获取或设置缓存
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="getValue">获取要设置的缓存值的Lambda表达式</param>
        /// <param name="expire">过期时间，默认5分钟</param>
        /// <returns></returns>
        public static T GetOrSet<T>(string cacheKey, Func<T> getValue, int expire = 5)
        {
            T value;
            if (Exists(cacheKey))
            {
                value = Get<T>(cacheKey);
            }
            else
            {
                value = getValue();
                Set(cacheKey, value, expire);
            }
            return value;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            if (Exists(key))
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// 是否存在缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            return _cache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCacheKeys()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var entries = _cache.GetType().GetField("_entries", flags).GetValue(_cache);
            var cacheItems = entries as IDictionary;
            var keys = new List<string>();
            if (cacheItems == null) return keys;
            foreach (DictionaryEntry cacheItem in cacheItems)
            {
                keys.Add(cacheItem.Key.ToString());
            }
            return keys;
        }

        /// <summary>
        /// 移除所有缓存
        /// </summary>
        public static void ClearCache()
        {
            _clearToken.Cancel();
            _clearToken.Dispose();
            _clearToken = new CancellationTokenSource();
        }




        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="keys"></param>
        public static void Remove(params string[] keys)
        {
            if (keys == null || keys.Length <= 0)
            {
                throw new Exception("argument [keys] is required to remove cache");
            }

            foreach (var key in keys)
            {
                if (Exists(key))
                {
                    _cache.Remove(key);
                }
            }
        }

    }
}
