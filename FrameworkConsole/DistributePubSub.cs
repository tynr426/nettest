using ECF.Cache;
using ECF.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkConsole
{

    public class DistributePubSub
    {
        static void Main(string[] args)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>()
            {
                {"upId",4 },
                 {"downId",44 },
            };
            DistributePubSub.RedisPub("soul_account", dic);
            Console.ReadLine();
            DistributePubSub.RedisPub("soul_account", dic);
            Console.ReadLine();
        }
        /// <summary>
        /// 线程同步变量
        /// </summary>
        private static object syncObj = new object();
        private static string cacheKey = "pubsub";

        private static CacheServerProvider cacheServerProvider = CacheFactory.GetCacheServerProvider(cacheKey);
        /// <summary>
        /// 缓存实例
        /// </summary>
        private static PubSubRedisService _instance = null;

        static DistributePubSub()
        {
            GetInstance();
        }

        /// <summary>
        /// 获得实例
        /// </summary>
        private static void GetInstance()
        {
            if (_instance == null)
            {
                lock (syncObj)
                {
                    if (_instance == null)
                    {
                        _instance = new PubSubRedisService(cacheServerProvider.Server, cacheServerProvider.Password);
                    }
                }
            }
        }
        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long RedisPub<T>(string channel, T data)
        {
            return _instance.RedisPub(channel, data);

        }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="subChannael"></param>
        /// <param name="action"></param>
        public static void RedisSub<T>(string subChannael, Action<T> action)
        {
            if (_instance == null) return;
            _instance.RedisSub<T>(subChannael, action);

        }
    }
}
