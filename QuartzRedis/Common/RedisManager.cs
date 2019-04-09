using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzRedis.Common
{
    public class RedisManager
    {
        private static ConfigurationOptions connDCS = ConfigurationOptions.Parse("www.a-cubic.com:16379");

        private static readonly object Locker = new object();

        private static ConnectionMultiplexer redisConn;

        public static ConnectionMultiplexer getRedisConn()
        {
            if (redisConn == null)
            {
                lock (Locker)
                {
                    if (redisConn == null || !redisConn.IsConnected)
                    {
                        redisConn = ConnectionMultiplexer.Connect(connDCS);
                    }
                }
            }
            return redisConn;
        }
    }
}
