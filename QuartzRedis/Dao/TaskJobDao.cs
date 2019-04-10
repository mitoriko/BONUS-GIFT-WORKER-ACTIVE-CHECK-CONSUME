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
        public Order GetOrder(string orderId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.SELECT_ORDER, orderId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 1)
            {
                Order order = new Order
                {
                    orderId = dt.Rows[0]["ORDER_ID"].ToString(),
                    state = dt.Rows[0]["STATE"].ToString(),
                    taskState = dt.Rows[0]["TASK_STATE"].ToString(),
                };
                return order;
            }

            return null;
        }

        public bool checkOrderGoodsState(string orderId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.SELECT_ORDER_GOODS_STATE, orderId);
            string sql = builder.ToString();
            DataTable dt = DatabaseOperation.ExecuteSelectDS(sql, "T").Tables[0];
            if (dt != null && dt.Rows.Count == 0)
            {
                return true;
            }

            return false;
        }

        public bool Done(string orderId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.UPDATE_ORDER_STATE, orderId);
            string sql = builder.ToString();
            return DatabaseOperation.ExecuteDML(sql);
        }

        public bool NotYet(string orderId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.UPDATE_ORDER_TASK_STATE, orderId, "1");
            string sql = builder.ToString();
            return DatabaseOperation.ExecuteDML(sql);
        }

        public bool CutOut(string orderId)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(TaskJobSqls.UPDATE_ORDER_TASK_STATE, orderId, "3");
            string sql = builder.ToString();
            return DatabaseOperation.ExecuteDML(sql);
        }
    }

    public class TaskJobSqls
    {
        public const string SELECT_ORDER = ""
            + "SELECT * "
            + "FROM T_BUSS_ORDER "
            + "WHERE ORDER_ID = {0} ";
        public const string SELECT_ORDER_GOODS_STATE = ""
            + "SELECT * "
            + "FROM T_BUSS_ORDER_GOODS A, T_BUSS_ORDER B "
            + "WHERE A.ORDER_CODE = B.ORDER_CODE "
            + "AND B.ORDER_ID = {0} "
            + "AND A.GOODS_STATE <> 2 "
            + "AND B.STATE = 1";
        public const string UPDATE_ORDER_STATE = ""
            + "UPDATE T_BUSS_ORDER "
            + "SET STATE = 2, "
            + "TASK_STATE = 3 "
            + "WHERE ORDER_ID = {0} "
            + "AND STATE = 1";
        public const string UPDATE_ORDER_TASK_STATE = ""
            + "UPDATE T_BUSS_ORDER "
            + "SET TASK_STATE = {1} "
            + "WHERE ORDER_ID = {0}";
    }
}
