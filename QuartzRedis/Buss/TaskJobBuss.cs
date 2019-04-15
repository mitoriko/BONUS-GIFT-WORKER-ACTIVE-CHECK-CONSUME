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
            try
            {
                MemberCheckStore memberCheckStore = taskJobDao.GetMemberCheckStore(ids);
                if (memberCheckStore == null)
                {
                    taskJobDao.Error(ids);
                }
                else
                {
                    Member member = taskJobDao.GetMember(memberCheckStore.memberId);
                    Store store = taskJobDao.GetStore(memberCheckStore.storeId);
                    List<ActiveItem> list = taskJobDao.GetActiveItems(memberCheckStore.storeId, memberCheckStore.consume);
                    foreach (ActiveItem item in list)
                    {
                        switch (item.valueType)
                        {
                            case "0":
                                Goods goods = taskJobDao.GetGoods(item.itemValue);
                                if (goods == null)
                                {
                                    taskJobDao.Error(ids);
                                }
                                else
                                {
                                    if (taskJobDao.GoodsAdd(
                                        goods,
                                        store,
                                        member.memberId,
                                        Convert.ToInt32(item.itemNums)))
                                    {
                                        taskJobDao.Done(ids);
                                    }
                                    else
                                    {
                                        taskJobDao.Error(ids);
                                    }
                                }
                                break;
                            case "1":
                                int total = Convert.ToInt32(item.itemValue) * Convert.ToInt32(item.itemNums);
                                if (taskJobDao.HeartAdd(
                                        total.ToString(),
                                        member.memberId,
                                        member.heart,
                                        store.storeId,
                                        item.activeId))
                                {
                                    taskJobDao.Done(ids);
                                }
                                else
                                {
                                    taskJobDao.Error(ids);
                                }
                                break;
                            case "2":
                                int totalLimit = Convert.ToInt32(item.itemValue) * Convert.ToInt32(item.itemNums);
                                if (taskJobDao.LimitAdd(
                                    store.storeId,
                                    member.memberId,
                                    item.actionType,
                                    totalLimit.ToString(),
                                    memberCheckStore.checkTime.ToString("yyyyMMdd")))
                                {
                                    taskJobDao.Done(ids);
                                }
                                else
                                {
                                    taskJobDao.Error(ids);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "处理" + Global.TASK_JOB + ":" + ids + "完成");
            }
            catch
            {
                taskJobDao.Error(ids);
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "处理" + Global.TASK_JOB + ":" + ids + "失败");
            }
        }
    }
}
