//Copyright (c) ServiceStack, Inc. All Rights Reserved.
//License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System.Data;

namespace KeMai.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHasDbConnection
    {
        IDbConnection DbConnection { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IHasDbCommand
    {
        IDbCommand DbCommand { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IHasDbTransaction
    {
        IDbTransaction DbTransaction { get; }
    }
}