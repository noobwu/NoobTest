//
// ServiceStack.OrmLite: Light-weight POCO ORM for .NET and Mono
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//   Tomasz Kubacki (tomasz.kubacki@gmail.com)
//
// Copyright 2012 Liquidbit Ltd.
//
// Licensed under the same terms of ServiceStack.
//

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public class DbNamingStrategyBase : INamingStrategy
    {
        public virtual string GetSchemaName(string name) => name;

        public virtual string GetSchemaName(ModelDefinition modelDef) => GetSchemaName(modelDef.Schema);

        public virtual string GetTableName(string name) => name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelDef"></param>
        /// <returns></returns>
        public virtual string GetTableName(ModelDefinition modelDef) {
            return GetTableName(modelDef.ModelName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string GetColumnName(string name)
        {
            return name;
        }
    }
}
