//
// ServiceStack.OrmLite: Light-weight POCO ORM for .NET and Mono
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2013 ServiceStack, Inc. All Rights Reserved.
//
// Licensed under the same terms of ServiceStack.
//

using KeMai.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace KeMai.StackExchange.DapperOrmLites
{
    public static class OrmLiteConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public const string IdField = "Id";
        /// <summary>
        /// 
        /// </summary>
        private const int defaultCommandTimeout = 30;
        //private static int? commandTimeout;

        /// <summary>
        /// 
        /// </summary>
        public static bool StripUpperInLike { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static Func<string, string> ParamNameFilter { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public static Func<string, string> SanitizeFieldNameForParamNameFn = fieldName => (fieldName ?? "").Replace(" ", "");

        private static IOrmLiteDialectProvider dialectProvider;
        /// <summary>
        /// 
        /// </summary>
        public static IOrmLiteDialectProvider DialectProvider
        {
            get
            {
                if (dialectProvider == null)
                {
                    throw new ArgumentNullException("DialectProvider",
                        "You must set the singleton 'OrmLiteConfig.DialectProvider' to use the OrmLiteWriteExtensions");
                }
                return dialectProvider;
            }
            set { dialectProvider = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static IOrmLiteDialectProvider GetOrmLiteDialectProvider(this IDbConnection db)
        {
            if (db is OrmLiteConnection)
            {
                OrmLiteConnection ormLiteConn = db as OrmLiteConnection;
                return ormLiteConn.DialectProvider as IOrmLiteDialectProvider;
            }
            return DialectProvider;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static IOrmLiteDialectProvider GetOrmLiteDialectProvider(this DefaultConnectionFactory factory)
        {
            if (factory is OrmLiteConnectionFactory)
            {
                return factory.DialectProvider as IOrmLiteDialectProvider;
            }
            return DialectProvider;
        }
    }
}