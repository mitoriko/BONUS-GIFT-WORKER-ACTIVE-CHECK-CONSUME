using Quartz;
using Quartz.Impl;
using QuartzRedis.Common;
using StackExchange.Redis;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace QuartzRedis
{
    class Program
    {
        //public static void onMessageHandle(ChannelMessage channelMessage)
        //{
        //    Console.WriteLine(channelMessage.Message.ToString());
        //}
        //static Action<ChannelMessage> action = new Action<ChannelMessage>(onMessageHandle);

        static void Main(string[] args)
        {
            StartAsync().GetAwaiter().GetResult();
        }

        static async Task StartAsync()
        {
            // construct a scheduler factory
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);

            // get a scheduler
            IScheduler sched = await factory.GetScheduler();
            await sched.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("myJob", "group1")
                .Build();
            
            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(1)
                    .RepeatForever())
            .Build();
            await sched.ScheduleJob(job, trigger);
            Console.ReadLine();
        }
    }

    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var redis = RedisManager.getRedisConn();
            var db = redis.GetDatabase(11);
            if (db.ListLength("Task." + "GIFT-ORDER-0-1") > 0)
            {
                RedisValue ids = await db.ListRightPopAsync("Task." + "GIFT-ORDER-0-1");
                if (!ids.IsNull)
                {
                    Console.WriteLine(ids);
                }
                
            }
        }
    }
}
