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
                    StringBuilder builder = new StringBuilder();
                    builder.AppendFormat(Global.SMS_CODE_URL, Global.SMS_CODE, Global.SMS_TPL, order.phone);
                    string url = builder.ToString();
                    string res = Utils.GetHttp(url);

                    SmsCodeRes smsCodeRes = JsonConvert.DeserializeObject<SmsCodeRes>(res);
                    if (smsCodeRes == null || smsCodeRes.error_code != 0)
                    {
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "发送短信失败原因:" + smsCodeRes.reason);
                    }
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
