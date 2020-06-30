using KeMai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeMai.Tests.AccessLogic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class RepositoryBase<TEntity, TPrimaryKey, TRepository> : KeMai.Domain.Repositories.RepositoryBaseOfTPrimaryKey<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TRepository : new()
    {
        #region 数据库信息
        /// <summary>
        /// 基本
        /// </summary>
        protected string Conn_Base
        {
            get { return ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString(); }
        }
        /// <summary>
        /// NK数据库
        /// </summary>
        protected string Conn_NK
        {
            get { return ConfigurationManager.ConnectionStrings["NKConnection"].ToString(); }
        }
        /// <summary>
        /// 餐饮数据库
        /// </summary>
        protected string Conn_Catering
        {
            get { return ConfigurationManager.ConnectionStrings["CateringConnection"].ToString(); }
        }
        /// <summary>
        /// 零售数据库
        /// </summary>
        protected string Conn_Retail
        {
            get { return ConfigurationManager.ConnectionStrings["RetailConnection"].ToString(); }
        }
        /// <summary>
        /// 支付数据库
        /// </summary>
        protected string Conn_Pay
        {
            get { return ConfigurationManager.ConnectionStrings["PaymentConnConnection"].ToString(); }
        }
        /// <summary>
        /// 日志数据库
        /// </summary>
        protected string Conn_Log
        {
            get { return ConfigurationManager.ConnectionStrings["LogConnConnection"].ToString(); }
        }
        /// <summary>
        /// 汇总数据库
        /// </summary>
        protected string Conn_Summary
        {
            get { return ConfigurationManager.ConnectionStrings["SummaryConnConnection"].ToString(); }
        }

        /// <summary>
        /// 获取业务数据库连接字符串，0非公司软件，1商城，2餐饮.
        /// </summary>
        public string GetConn(int custid)
        {
            //CustInfo cust = KM.MyCache.CacheService.GetCustInfo(custid);
            //string p = cust == null ? "" : cust.ProductionCust;
            string p = null;
            switch (p)
            {
                case "0":
                    return Conn_NK;
                case "1":
                    return Conn_Retail;
                case "2":
                    return Conn_Catering;
                case "3":
                    return Conn_Retail;
                default:
                    return Conn_Catering;
            }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        private static TRepository instance = new TRepository();
        /// <summary>
        /// 
        /// </summary>
        public static TRepository Instance
        {
            get { return instance; }
        }
    }
}
