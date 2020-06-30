using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KeMai.Tests.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface IModel_OrmLiteBase<TEntity, TPrimaryKey>
    {
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        int Insert(TEntity entity, string connectionString = null);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="updateFields"></param>
        /// <param name="id">主键Id</param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        int Update(Expression<Func<TEntity>> updateFields, TPrimaryKey id, string connectionString = null);
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        int Delete(TPrimaryKey id, string connectionString = null);
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        TEntity Get(TPrimaryKey id, string connectionString = null);
    }
}
