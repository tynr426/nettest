using ECF.Caching;
using ECF.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sdk
{
    public class WebCache
    {
        
        internal const string CachePrefix = "Connector_";

        /// <summary>
        /// 线程同步变量
        /// </summary>
        private static object syncObj = new object();

        private static bool Enable = true;

        /// <summary>
        /// 缓存实例
        /// </summary>
        private static ICacheService _instance = null;

        static WebCache()
        {
            GetInstance();
        }

        private static string WrapKey(string key)
        {
            return (CachePrefix + key).ToLower();
        }

        /// <summary>
        /// 获得实例
        /// </summary>
        private static void GetInstance()
        {
            if (Enable && _instance == null)
            {
                lock (syncObj)
                {
                    //string cacheKey = "vast";

                    //if (!string.IsNullOrEmpty(cacheKey))
                    //{
                    //    _instance = CacheFactory.GetCacheService(CacheFactory.GetCacheServerProvider(cacheKey));
                    //}

                    // 当前在的配置错误没有取到配置的缓存服务读取默认服务 
                    if (_instance == null)
                        _instance = CacheFactory.GetDefaultService();
                }
            }
        }
        /// <summary>
        /// 获得cache实例
        /// </summary>
        /// <returns></returns>
        public static ICacheService GetCache()
        {
            // _instance.
            return _instance;
        }

        /// <summary>
        /// 获得指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        public static object Get(string key)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(key))
                return null;
            return _instance.Get(WrapKey(key));
        }

        /// <summary>
        /// 获得指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        public static T Get<T>(string key)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(key))
                return default(T);
            return _instance.Get<T>(WrapKey(key));
        }

        /// <summary>
        /// 将指定键的对象添加到缓存中
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        public static void Insert(string key, object data)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(key) || data == null)
                return;

            _instance.Insert(WrapKey(key), data);

        }
        /// <summary>
        /// 将指定键的对象添加到缓存中
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        public static void Add<T>(string key, T data)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(key) || data == null)
                return;

            _instance.Insert<T>(WrapKey(key), data);

        }
        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间(分钟)</param>
        public static void Insert(string key, object data, int cacheTime)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(key) || data == null)
                return;
            _instance.Insert(WrapKey(key), data, cacheTime);

        }

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间(分钟)</param>
        public static void Add<T>(string key, T data, int cacheTime)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(key) || data == null)
                return;
            _instance.Insert<T>(WrapKey(key), data, cacheTime);


        }

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间</param>
        public static void Insert(string key, object data, DateTime cacheTime)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(key) || data == null)
                return;
            _instance.Insert(WrapKey(key), data, cacheTime);


        }

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间</param>
        public static void Add<T>(string key, T data, DateTime cacheTime)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(key) || data == null)
                return;
            _instance.Insert<T>(WrapKey(key), data, cacheTime);


        }

        /// <summary>
        /// 设置最大缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public static void Max(string key, object data)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(key) || data == null)
                return;

            _instance.Insert(WrapKey(key), data, DateTime.MaxValue);

        }

        /// <summary>
        /// 从缓存中移除指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        public static void Remove(string key)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(key))
                return;

            _instance.Remove(WrapKey(key));

        }

        /// <summary>
        /// 清除前缀开头的
        /// </summary>
        /// <param name="prefixKey"></param>
        public static void Clear(string prefixKey)
        {
            if (string.IsNullOrWhiteSpace(prefixKey)) return;
            _instance.Clear(WrapKey(prefixKey));
        }

        /// <summary>
        /// 根据正则表达式进行移除
        /// </summary>
        /// <param name="pattern"></param>
        public static void RemoveByPattern(string pattern)
        {
            if (_instance == null || string.IsNullOrWhiteSpace(pattern)) return;
            _instance.RemoveByPattern(pattern);
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void Clear()
        {
            _instance.Clear();
        }
       
    }
}
