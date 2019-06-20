using Com.ACBC.Framework.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace QuartzRedis.Dao
{
    public class TaskJobDao
    {
        public MemberCheckStore GetMemberCheckStore(string memberCheckStoreId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.SELECT_MEMBER_CHECK_STORE, memberCheckStoreId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                MemberCheckStore memberCheckStore = new MemberCheckStore
                {
                    checkTime = Convert.ToDateTime(dt.Rows[0]["CHECK_TIME"]),
                    consume = dt.Rows[0]["CONSUME"].ToString(),
                    memberId = dt.Rows[0]["MEMBER_ID"].ToString(),
                    storeId = dt.Rows[0]["STORE_ID"].ToString(),
                };
                return memberCheckStore;
            }

            return null;
        }

        public List<ActiveItem> GetActiveItems(string storeId, string consume)
        {
            List<ActiveItem> list = new List<ActiveItem>();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.SELECT_ACTIVE_ITEM, storeId, consume);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null)
            {
                DataRow[] checkItem = dt.Select("ACTION_TYPE = 1");
                DataRow[] consumeItem = dt.Select("ACTION_TYPE = 0", "CONSUME DESC");
                int maxConsume = 0;
                if(consumeItem.Length > 0)
                {
                    maxConsume = Convert.ToInt32(consumeItem[0]["CONSUME"]);
                }
                foreach (DataRow dr in checkItem)
                {
                    ActiveItem activeItem = new ActiveItem()
                    {
                        itemNums = dr["ITEM_NUMS"].ToString(),
                        actionType = dr["ACTION_TYPE"].ToString(),
                        itemValue = dr["ITEM_VALUE"].ToString(),
                        valueType = dr["VALUE_TYPE"].ToString(),
                        activeId = dr["ACTIVE_ID"].ToString(),
                    };
                    list.Add(activeItem);
                }

                foreach (DataRow dr in consumeItem)
                {
                    if(Convert.ToInt32(dr["CONSUME"]) == maxConsume)
                    {
                        ActiveItem activeItem = new ActiveItem()
                        {
                            itemNums = dr["ITEM_NUMS"].ToString(),
                            actionType = dr["ACTION_TYPE"].ToString(),
                            itemValue = dr["ITEM_VALUE"].ToString(),
                            valueType = dr["VALUE_TYPE"].ToString(),
                            activeId = dr["ACTIVE_ID"].ToString(),
                        };
                        list.Add(activeItem);
                    }
                }

                //foreach (DataRow dr in dt.Rows)
                //{
                //    ActiveItem activeItem = new ActiveItem()
                //    {
                //        itemNums = dr["ITEM_NUMS"].ToString(),
                //        actionType = dr["ACTION_TYPE"].ToString(),
                //        itemValue = dr["ITEM_VALUE"].ToString(),
                //        valueType = dr["VALUE_TYPE"].ToString(),
                //        activeId = dr["ACTIVE_ID"].ToString(),
                //    };
                //    list.Add(activeItem);
                //}
            }

            return list;
        }

        public bool LimitAdd(string storeId, string memberId, string actionValue, string num, string useDate, string active_id)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                TaskJobSqls.INSERT_LIMIT_ADD,
                storeId,
                memberId,
                actionValue,
                num,
                useDate,
                active_id);
            string sql = builder.ToString();
            return DatabaseOperation.ExecuteDML(sql);
        }

        public Member GetMember(string memberId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.SELECT_MEMBER_HEART_BY_MEMBER_ID, memberId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                Member member = new Member
                {
                    heart = dt.Rows[0]["HEART"].ToString(),
                    memberImg = dt.Rows[0]["MEMBER_IMG"].ToString(),
                    memberId = dt.Rows[0]["MEMBER_ID"].ToString(),
                    memberName = dt.Rows[0]["MEMBER_NAME"].ToString(),
                };
                return member;
            }

            return null;
        }

        public bool HeartAdd(string num, string memberId, string beforeMod, string storeId, string activeId)
        {
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                TaskJobSqls.INSERT_HEART_CHANGE,
                num,
                memberId,
                beforeMod,
                storeId,
                activeId);
            string sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(
                TaskJobSqls.UPDATE_MEMBER_HEART_BY_MEMBER_ID,
                memberId,
                num);
            sql = builder.ToString();
            list.Add(sql);
            return DatabaseOperation.ExecuteDML(list);
        }

        public Goods GetGoods(string goodsId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.SELECT_GOODS, goodsId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                Goods goods = new Goods
                {
                    goodsId = dt.Rows[0]["GOODS_ID"].ToString(),
                    goodsImg = dt.Rows[0]["GOODS_IMG"].ToString(),
                    goodsName = dt.Rows[0]["GOODS_NAME"].ToString(),
                    goodsPrice = Convert.ToDouble(dt.Rows[0]["GOODS_PRICE"]),
                    goodsStock = Convert.ToInt32(dt.Rows[0]["GOODS_STOCK"]),
                };
                return goods;
            }

            return null;
        }

        public Store GetStore(string storeId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.SELECT_STORE, storeId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                Store store = new Store
                {
                    storeCode = dt.Rows[0]["STORE_CODE"].ToString(),
                    storeId = dt.Rows[0]["STORE_ID"].ToString(),
                    storeAddr = dt.Rows[0]["STORE_ADDR"].ToString(),
                };
                return store;
            }

            return null;
        }

        public bool GoodsAdd(Goods goods, Store store, string memberId, int num)
        {
            double total = Math.Round(goods.goodsPrice * num, 2);
            string orderCode = store.storeCode + memberId.PadLeft(6, '0') + DateTime.Now.ToString("yyyyMMddHHmmss");
            ArrayList list = new ArrayList();
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                TaskJobSqls.INSERT_ORDER,
                orderCode,
                total,
                store.storeCode,
                memberId,
                store.storeAddr
                );
            string sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(
                TaskJobSqls.INSERT_ORDER_GOODS,
                orderCode,
                goods.goodsId,
                goods.goodsImg,
                goods.goodsName,
                goods.goodsPrice,
                num
                );
            sql = builder.ToString();
            list.Add(sql);
            builder.Clear();
            builder.AppendFormat(
                TaskJobSqls.UPDATE_GOODS_STOCK_BY_GOODS_ID,
                goods.goodsId,
                num);
            sql = builder.ToString();
            list.Add(sql);
            return DatabaseOperation.ExecuteDML(list);
        }

        public bool Done(string memberCheckStoreId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.UPDATE_MEMBER_CHECK_STORE_TASK_STATE, memberCheckStoreId, "2");
            string sql = builder.ToString();
            return DatabaseOperation.ExecuteDML(sql);
        }

        public bool NotYet(string memberCheckStoreId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.UPDATE_MEMBER_CHECK_STORE_TASK_STATE, memberCheckStoreId, "0");
            string sql = builder.ToString();
            return DatabaseOperation.ExecuteDML(sql);
        }

        public bool Error(string memberCheckStoreId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.UPDATE_MEMBER_CHECK_STORE_TASK_STATE, memberCheckStoreId, "3");
            string sql = builder.ToString();
            return DatabaseOperation.ExecuteDML(sql);
        }
    }

    public class TaskJobSqls
    {
        public const string SELECT_MEMBER_CHECK_STORE = ""
            + "SELECT * FROM T_BUSS_MEMBER_CHECK_STORE WHERE MEMBER_CHECK_STORE_ID = {0}";
        public const string SELECT_ACTIVE_ITEM = ""
            + "SELECT ITEM_NUMS,ITEM_VALUE,VALUE_TYPE,0 AS ACTION_TYPE,T.ACTIVE_ID,A.CONSUME AS CONSUME "
            + "FROM T_BUSS_ACTIVE T,T_BUSS_ACTIVE_CONSUME A "
            + "WHERE NOW() BETWEEN T.ACTIVE_TIME_FROM AND T.ACTIVE_TIME_TO "
            + "AND T.ACTIVE_TYPE = 0 "
            + "AND A.ACTIVE_ID = T.ACTIVE_ID "
            + "AND A.CONSUME <= {1} "
            + "AND T.ACTIVE_STORE = {0} "
            + "AND T.ACTIVE_STATE = '1' "
            + "UNION "
            + "SELECT ITEM_NUMS,ITEM_VALUE,VALUE_TYPE,1 AS ACTION_TYPE,T.ACTIVE_ID,0 AS CONSUME "
            + "FROM T_BUSS_ACTIVE T,T_BUSS_ACTIVE_CHECK A "
            + "WHERE NOW() BETWEEN T.ACTIVE_TIME_FROM AND T.ACTIVE_TIME_TO "
            + "AND T.ACTIVE_TYPE = 1 "
            + "AND A.ACTIVE_ID = T.ACTIVE_ID "
            + "AND T.ACTIVE_STORE = {0} "
            + "AND T.ACTIVE_STATE = '1' ";
        public const string INSERT_LIMIT_ADD = ""
            + "INSERT INTO T_BUSS_STORE_LIMIT_ADD"
            + "(STORE_ID, MEMBER_ID, ACTION_VALUE, OPERATE_TIME, NUM, USE_DATE, ACTIVE_ID) "
            + "VALUES({0},{1},{2},NOW(),{3},'{4}',{5})";
        public const string SELECT_MEMBER_HEART_BY_MEMBER_ID = ""
            + "SELECT * "
            + "FROM T_BASE_MEMBER "
            + "WHERE MEMBER_ID = {0} ";
        public const string INSERT_HEART_CHANGE = ""
            + "INSERT INTO T_BUSS_HEART_CHANGE "
            + "(CHANGE_TYPE,NUM,MEMBER_ID,BEFORE_MOD,STORE_ID,ACTIVITY_ID) "
            + "VALUES(2,{0},{1},{2},{3},{4}) ";
        public const string UPDATE_MEMBER_HEART_BY_MEMBER_ID = ""
            + "UPDATE T_BASE_MEMBER "
            + "SET HEART = HEART + {1} "
            + "WHERE MEMBER_ID = {0} ";
        public const string SELECT_GOODS = ""
            + "SELECT * FROM T_BUSS_GOODS WHERE GOODS_ID = {0}";
        public const string SELECT_STORE = ""
            + "SELECT * FROM T_BASE_STORE WHERE STORE_ID = {0}";
        public const string INSERT_ORDER = ""
            + "INSERT INTO T_BUSS_ORDER "
            + "(ORDER_CODE,TOTAL,STORE_CODE,MEMBER_ID,ADDR,STATE,TASK_STATE) "
            + "VALUES('{0}',{1},'{2}',{3},'{4}', 1, 1)";
        public const string INSERT_ORDER_GOODS = ""
            + "INSERT INTO T_BUSS_ORDER_GOODS "
            + "(GOODS_ID,GOODS_IMG,GOODS_NAME,PRICE,NUM,ORDER_CODE) "
            + "VALUES({1},'{2}','{3}',{4},{5},'{0}')";
        public const string UPDATE_GOODS_STOCK_BY_GOODS_ID = ""
                + "UPDATE T_BUSS_GOODS "
                + "SET GOODS_STOCK = GOODS_STOCK - {1} "
                + "WHERE GOODS_ID = {0} ";
        public const string UPDATE_MEMBER_CHECK_STORE_TASK_STATE = ""
            + "UPDATE T_BUSS_MEMBER_CHECK_STORE "
            + "SET TASK_STATE = {1} "
            + "WHERE MEMBER_CHECK_STORE_ID = {0} ";
    }
}
