using QuartzRedis.Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzRedis.Buss
{
    public class TaskJobBuss
    {
        public void doWork(string ids)
        {
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine(ids);
        }
    }
}
