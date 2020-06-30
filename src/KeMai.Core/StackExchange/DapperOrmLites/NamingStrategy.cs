using KeMai.Text;
using System.Collections.Generic;

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public class AliasNamingStrategy : DbNamingStrategyBase
    {
        public Dictionary<string, string> TableAliases = new Dictionary<string, string>();
        public Dictionary<string, string> ColumnAliases = new Dictionary<string, string>();
        public INamingStrategy UseNamingStrategy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetTableName(string name)
        {
            string alias;
            return UseNamingStrategy != null
                ? UseNamingStrategy.GetTableName(TableAliases.TryGetValue(name, out alias) ? alias : name)
                : base.GetTableName(TableAliases.TryGetValue(name, out alias) ? alias : name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetColumnName(string name)
        {
            string alias;
            return UseNamingStrategy != null
                ? UseNamingStrategy.GetColumnName(ColumnAliases.TryGetValue(name, out alias) ? alias : name)
                : base.GetColumnName(ColumnAliases.TryGetValue(name, out alias) ? alias : name);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LowercaseUnderscoreNamingStrategy : DbNamingStrategyBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetTableName(string name)
        {
            return name.ToLowercaseUnderscore();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string GetColumnName(string name)
        {
            return name.ToLowercaseUnderscore();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UpperCaseNamingStrategy : DbNamingStrategyBase
    {
        public override string GetTableName(string name)
        {
            return name.ToUpper();
        }

        public override string GetColumnName(string name)
        {
            return name.ToUpper();
        }
    }
}