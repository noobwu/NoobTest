using KeMai.Domain.Entities;
using KeMai.Tests.AccessLogic;
using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;

namespace KeMai.Tests.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Model_OrmLiteBase<TEntity, TPrimaryKey, TRepository> : IModel_OrmLiteBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
         where TRepository : new()
    {
        #region 数据库连接地址
        /// <summary>
        /// 数据库连接地址
        /// </summary>
        private string connectionString;
        #endregion
        RepositoryBase<TEntity, TPrimaryKey, TRepository> repository = null;
        public Model_OrmLiteBase()
        {

        }
        public Model_OrmLiteBase(string namedConnection)
        {
            var connSettings = ConfigurationManager.ConnectionStrings[namedConnection];
            if (connSettings != null)
            {
                connectionString = connSettings.ConnectionString;
            }
            else
            {
                connectionString = ConfigurationManager.ConnectionStrings["CateringConnection"].ToString();
            }
            repository = new RepositoryBase<TEntity, TPrimaryKey, TRepository>();
        }
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual int Insert(TEntity entity, string connectionString)
        {
            connectionString = GetConnectionString(connectionString);
            return repository.Insert(connectionString, entity);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="updateFields"></param>
        /// <param name="id">主键Id</param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual int Update(Expression<Func<TEntity>> updateFields, TPrimaryKey id, string connectionString = null)
        {
            connectionString = GetConnectionString(connectionString);
            return repository.UpdateOnly(connectionString, updateFields, id);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual int Delete(TPrimaryKey id, string connectionString)
        {
            connectionString = GetConnectionString(connectionString);
            return repository.Delete(connectionString, id);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public virtual TEntity Get(TPrimaryKey id, string connectionString)
        {
            connectionString = GetConnectionString(connectionString);
            return repository.Single(connectionString, id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private string GetConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) return this.connectionString;
            return connectionString;
        }
    }
}
