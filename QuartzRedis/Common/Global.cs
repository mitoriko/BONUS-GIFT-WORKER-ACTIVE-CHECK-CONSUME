using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzRedis.Common
{
    public class Global
    {
        public const string ENV = "PRO";
        public const string GROUP = "Task";

        public const string TASK_JOB = "GIFT-ORDER-0-1";

        public const string TASK_PREFIX = "Task";

        public const string CONFIG_TOPIC = "ConfigServerTopic";
        public const string TASK_TOPIC = "TaskTopic";

        public const string TOPIC_MESSAGE = "update";
        public const int REDIS_DB = 11;

        public static string Redis
        {
            get
            {
                return Environment.GetEnvironmentVariable("Redis");
            }
        }


    }
}
