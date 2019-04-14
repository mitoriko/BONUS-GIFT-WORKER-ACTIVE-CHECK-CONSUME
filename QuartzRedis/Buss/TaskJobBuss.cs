using Newtonsoft.Json;
using QuartzRedis.Common;
using QuartzRedis.Dao;
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
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "开始处理" + Global.TASK_JOB + ":" + ids);
            TaskJobDao taskJobDao = new TaskJobDao();
            Order order = taskJobDao.GetOrder(ids);
            if(order == null || order.state != "1")
            {
                taskJobDao.CutOut(ids);
            }

            if(taskJobDao.checkOrderGoodsState(ids))
            {
                if(taskJobDao.Done(ids))
                {
                }
            }
            else
            {
                taskJobDao.NotYet(ids);
            }
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "处理" + Global.TASK_JOB + ":" + ids + "完成");
        }
    }
}
