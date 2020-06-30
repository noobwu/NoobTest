using System;
namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public interface INamingStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetSchemaName(string name);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <returns></returns>
        string GetSchemaName(ModelDefinition modelDef);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetTableName(string name);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <returns></returns>
        string GetTableName(ModelDefinition modelDef);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetColumnName(string name);
    }
}
