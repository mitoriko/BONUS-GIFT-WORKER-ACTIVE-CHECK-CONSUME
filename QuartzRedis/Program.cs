using QuartzRedis.Buss;
using QuartzRedis.Common;
using System;

namespace QuartzRedis
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskJob.Worker();
            TaskJob.Subscribe();
            Console.ReadLine();
        }
    }
}
