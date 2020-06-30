using System;
using System.Configuration;
namespace KeMai.Text
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CookieConstants
    {
        public const int COOKIE_EXPIRES = 30;    //Cookie过期时间分钟
        public const String ENCRYPT_KEY = "ukey!123?WIPISA456789a$d";//密钥
        public const string COOKIE_NAME = "KeMai";    //Cookie名称
        public const string COOKIE_ADMIN_NAME = "KeMaiAdmin";    //Cookie名称
        private const string COOKIE_ADMIN_PREFIX = "Admin::";    //键前缀
        public const string COOKIE_ADMIN_USERID = COOKIE_ADMIN_PREFIX + "UserID";    //UserID键
        public const string COOKIE_ADMIN_USER_KEY = COOKIE_ADMIN_PREFIX + "UserKey";    //UserKey键

        private const string COOKIE_PREFIX = "Web::";    //键前缀
        public const string COOKIE_USERID = COOKIE_PREFIX + "UserID";    //UserID键
        public const string COOKIE_USER_KEY = COOKIE_PREFIX + "UserKey";    //UserKey键
        public const string COOKIE_LOGIN_TYPE = COOKIE_PREFIX + "LoginType";    //LoginType键
        public const string COOKIE_HOST = COOKIE_PREFIX + "Host";    //Host键
        public const string COOKIE_CODE = COOKIE_PREFIX + "Code";    //Code键

    }

    /// <summary>
    /// 
    /// </summary>
    public partial class DataValueConstants
    {
        public const int MIN_INT = 0;//最小时间
        public const int MAX_INT = Int32.MaxValue;//最大时间
        public const int MAX_ROWX = 10000;//最大记录数
        public static DateTime MIN_DATE = new DateTime(1900, 1, 1);//最小时间
        public static DateTime MAX_DATE = new DateTime(2999, 1, 1);//最大时间
		/// <summary>
        /// 导出最大记录数
        /// </summary>
        public static int MaxExportRows
        {
            get { return ConfigurationManager.ConnectionStrings["MaxExportRows"].To(10000); }
        }
    }
}
