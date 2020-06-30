using KeMai.Domain.Entities;
using KeMai.Domain.Repositories;
using KeMai.StackExchange.Dapper;
using KeMai.Tests.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace KeMai.Tests.Entities.Repositories
{
    /// <summary>
    /// 卡券数据库相关操作
    /// </summary>
    public class MarCardRepository : RepositoryBaseOfTPrimaryKey<MarCard, int>
    {
        /// <summary>
        /// 餐饮相关数据库连接地址
        /// </summary>
        public string Conn
        {
            get { return ConfigurationManager.ConnectionStrings["Test2"].ToString(); }
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert(MarCard entity)
        {
            return base.Insert(Conn,entity);
        }
        /// <summary>
        /// 更新卡券记录
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="custId">商户Id</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Update(MarCard entity, int custId, int id)
        {
            string whereExpression = "WHERE CustId=@CustId AND RowId=@RowId";
            return base.Update(Conn,entity, whereExpression, new { CustId = custId, RowId = id });
        }
        /// <summary>
        /// 获取卡券记录
        /// </summary>
        /// <param name="custId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public MarCard Get(int custId, int id)
        {
            string sql = string.Format("SELECT * FROM {0}  WHERE CustId=@CustId AND RowId=@RowId", TableName);
            return base.Single(Conn,sql, new { CustId = custId, RowId = id });
        }
        /// <summary>
        ///获取卡券分页数据
        /// </summary>
        /// <param name="custId">商户Id</param>
        /// <param name="title">卡券名称</param>
        /// <param name="cardType">卡券类型(CASH:代金券,DISCOUNT:折扣券,GIFT:兑换券,GROUPON:团购券)</param>
        /// <param name="status">卡券状态(0:待审核,1:审核中,2:审核通过,2:审核未通过,3:待投放,4:已投放,5:下架)</param>
        /// <param name="orderColumn">排序列</param>
        /// <param name="orderBy">排序类型</param>
        /// <param name="offset">开始位置</param>
        /// <param name="pageRows">每页显示记录数</param>
        /// <param name="totalRows">总记录数</param>
        /// <returns>the list of query result.</returns>
        public IEnumerable<MarCard> GetPaggingList(int custId, string title, string cardType, byte? status, string orderColumn, ListResultsOrder orderBy, int? offset, int? pageRows, out int totalRows)
        {
            var builder = new SqlSelectBuilder();
            var template = builder.AddTemplate(new SqlSever2012Template(builder, "SELECT /**select**/", string.Format("FROM  {0}  /**where**/", TableName), "/**orderby**/", null));
            builder.Select("*");
            builder.Where("CustId=@CustId", new { CustId = custId });
            if (!string.IsNullOrEmpty(title))
            {
                title = "%" + title + "%";
                builder.Where("Title LIKE @Title", new { Title = title });
            }
            if (!string.IsNullOrEmpty(cardType))
            {
                builder.Where("CardType = @CardType", new { CardType = cardType });
            }
            if (status.HasValue)
            {
                builder.Where("Status = @Status", new { Status = status.Value });
            }
            return base.GetPaggingList(Conn, builder, template, orderColumn, orderBy, offset, pageRows, out totalRows);
        }
    }
}
