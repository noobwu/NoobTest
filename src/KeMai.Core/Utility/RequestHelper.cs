using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using KeMai.Text;
namespace KeMai
{
    /// <summary>
    /// Request操作类
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
        /// 判断当前页面是否接收到了Post请求
        /// </summary>
        /// <returns>是否接收到了Post请求</returns>
        public static bool IsPost(HttpRequestBase request = null)
        {
            if (request == null)
            {
                request = new HttpRequestWrapper(HttpContext.Current.Request);
            }
            return request.HttpMethod.Equals("POST");
        }
        /// <summary>
        /// 判断当前页面是否接收到了Get请求
        /// </summary>
        /// <returns>是否接收到了Get请求</returns>
        public static bool IsGet(HttpRequestBase request = null)
        {
            if (request == null)
            {
                request = new HttpRequestWrapper(HttpContext.Current.Request);
            }
            return request.HttpMethod.Equals("GET");
        }

        /// <summary>
        /// 返回指定的服务器变量信息
        /// </summary>
        /// <param name="strName">服务器变量名</param>
        /// <returns>服务器变量信息</returns>
        public static string GetServerString(string strName)
        {
            //
            if (HttpContext.Current.Request.ServerVariables[strName] == null)
            {
                return "";
            }
            return HttpContext.Current.Request.ServerVariables[strName].ToString();
        }

        /// <summary>
        /// 返回上一个页面的地址
        /// </summary>
        /// <returns>上一个页面的地址</returns>
        public static string GetUrlReferrer()
        {
            if (HttpContext.Current.Request.UrlReferrer == null) return string.Empty;
            string retVal = null;

            try
            {
                retVal = HttpContext.Current.Request.UrlReferrer.ToString();
            }
            catch { }

            if (retVal == null)
                return "";

            return retVal;

        }

        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (!request.Url.IsDefaultPort)
            {
                return string.Format("{0}://{1}:{2}", request.Url.Scheme, request.Url.Host, request.Url.Port.ToString());
            }
            return string.Format("{0}://{1}", request.Url.Scheme, request.Url.Host);
        }

        /// <summary>
        /// 得到主机头
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }


        /// <summary>
        /// 获取当前请求的原始 URL(URL 中域信息之后的部分,包括查询字符串(如果存在))
        /// </summary>
        /// <returns>原始 URL</returns>
        public static string GetRawUrl()
        {
            return HttpContext.Current.Request.RawUrl;
        }

        /// <summary>
        /// 判断当前访问是否来自浏览器软件
        /// </summary>
        /// <returns>当前访问是否来自浏览器软件</returns>
        public static bool IsBrowserGet()
        {
            string[] BrowserName = { "ie", "opera", "netscape", "mozilla", "konqueror", "firefox" };
            string curBrowser = HttpContext.Current.Request.Browser.Type.ToLower();
            for (int i = 0; i < BrowserName.Length; i++)
            {
                if (curBrowser.IndexOf(BrowserName[i]) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否来自搜索引擎链接
        /// </summary>
        /// <returns>是否来自搜索引擎链接</returns>
        public static bool IsSearchEnginesGet()
        {
            if (HttpContext.Current.Request.UrlReferrer == null && HttpContext.Current.Request.UserAgent == null)
            {
                return false;
            }
            string[] SearchEngine = { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou" };
            if (HttpContext.Current.Request.UrlReferrer != null)
            {
                string tmpReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
                tmpReferrer = Regex.Replace(tmpReferrer, "url=([\\s\\S])+", string.Empty);
                for (int i = 0; i < SearchEngine.Length; i++)
                {
                    if (tmpReferrer.IndexOf(SearchEngine[i]) >= 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                string strUserAgent = HttpContext.Current.Request.UserAgent;
                for (int i = 0; i < SearchEngine.Length; i++)
                {
                    if (strUserAgent.IndexOf(SearchEngine[i]) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 获得当前完整Url地址
        /// </summary>
        /// <returns>当前完整Url地址</returns>
        public static string GetUrl()
        {
            return HttpContext.Current.Request.Url.ToString();
        }


        /// <summary>
        /// 获得指定Url参数的值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName)
        {
            return GetQueryString(strName, false);
        }

        /// <summary>
        /// 获得指定Url参数的值
        /// </summary> 
        /// <param name="strName">Url参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.QueryString[strName] == null)
            {
                return "";
            }

            if (sqlSafeCheck && !Utils.IsSafeSqlString(HttpContext.Current.Request.QueryString[strName]))
            {
                return "unsafe string";
            }

            return HttpContext.Current.Request.QueryString[strName];
        }

        /// <summary>
        /// 获得当前页面的名称
        /// </summary>
        /// <returns>当前页面的名称</returns>
        public static string GetPageName()
        {
            string[] urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
            return urlArr[urlArr.Length - 1].ToLower();
        }

        /// <summary>
        /// 返回表单或Url参数的总个数
        /// </summary>
        /// <returns></returns>
        public static int GetParamCount()
        {
            return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
        }


        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string strName)
        {
            return GetFormString(strName, false);
        }

        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>表单参数的值</returns>
        public static string GetFormString(string strName, bool sqlSafeCheck)
        {
            if (HttpContext.Current.Request.Form[strName] == null)
            {
                return "";
            }

            if (sqlSafeCheck && !Utils.IsSafeSqlString(HttpContext.Current.Request.Form[strName]))
            {
                return "unsafe string";
            }

            return HttpContext.Current.Request.Form[strName];
        }

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string strName)
        {
            return GetString(strName, false);
        }

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">参数</param>
        /// <param name="sqlSafeCheck">是否进行SQL安全检查</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetString(string strName, bool sqlSafeCheck)
        {
            if ("".Equals(GetQueryString(strName)))
            {
                return GetFormString(strName, sqlSafeCheck);
            }
            else
            {
                return GetQueryString(strName, sqlSafeCheck);
            }
        }


        /// <summary>
        /// 获得指定Url参数的int类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static int GetQueryInt(string strName, int defValue)
        {
            return HttpContext.Current.Request.QueryString[strName].ToInt(defValue);
        }


        /// <summary>
        /// 获得指定表单参数的int类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的int类型值</returns>
        public static int GetFormInt(string strName, int defValue)
        {
            return HttpContext.Current.Request.Form[strName].ToInt(defValue);
        }

        /// <summary>
        /// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static int GetInt(string strName)
        {
            return GetInt(strName, 0);
        }
        /// <summary>
        /// 获得指定Url或表单参数的int类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static int GetInt(string strName, int defValue)
        {
            if (GetQueryInt(strName, defValue) == defValue)
            {
                return GetFormInt(strName, defValue);
            }
            else
            {
                return GetQueryInt(strName, defValue);
            }
        }



        /// <summary>
        /// 获得指定Url参数的long类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static long GetQueryLong(string strName, long defValue)
        {
            return HttpContext.Current.Request.QueryString[strName].To(defValue);
        }


        /// <summary>
        /// 获得指定表单参数的int类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的int类型值</returns>
        public static long GetFormLong(string strName, long defValue)
        {
            return HttpContext.Current.Request.Form[strName].To(defValue);
        }

        /// <summary>
        /// 获得指定Url或表单参数的long类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static long GetLong(string strName)
        {
            return GetLong(strName, 0);
        }
        /// <summary>
        /// 获得指定Url或表单参数的long类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static long GetLong(string strName, long defValue)
        {
            if (GetQueryLong(strName, defValue) == defValue)
            {
                return GetFormLong(strName, defValue);
            }
            else
            {
                return GetQueryLong(strName, defValue);
            }
        }
        /// <summary>
        /// 获得指定Url参数的float类型值
        /// </summary>
        /// <param name="strName">Url参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url参数的int类型值</returns>
        public static float GetQueryFloat(string strName, float defValue)
        {
            return HttpContext.Current.Request.QueryString[strName].ToFloat(defValue);
        }


        /// <summary>
        /// 获得指定表单参数的float类型值
        /// </summary>
        /// <param name="strName">表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>表单参数的float类型值</returns>
        public static float GetFormFloat(string strName, float defValue)
        {
            return HttpContext.Current.Request.Form[strName].ToFloat(defValue);
        }

        /// <summary>
        /// 获得指定Url或表单参数的float类型值, 先判断Url参数是否为缺省值, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="strName">Url或表单参数</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>Url或表单参数的int类型值</returns>
        public static float GetFloat(string strName, float defValue)
        {
            if (GetQueryFloat(strName, defValue) == defValue)
            {
                return GetFormFloat(strName, defValue);
            }
            else
            {
                return GetQueryFloat(strName, defValue);
            }
        }

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string result = String.Empty;
            HttpContext context = HttpContext.Current;
            if (context != null && context.Request != null)
            {
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(result))
                {
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(result))
                {
                    result = HttpContext.Current.Request.UserHostAddress;
                }
            }
            if (string.IsNullOrEmpty(result) || !Utils.IsIP(result))
            {
                return "127.0.0.1";
            }
            return result;

        }

        /// <summary>
        /// 保存用户上传的文件
        /// </summary>
        /// <param name="path">保存路径</param>
        public static void SaveRequestFile(string path)
        {
            if (HttpContext.Current.Request.Files.Count > 0)
            {
                HttpContext.Current.Request.Files[0].SaveAs(path);
            }
        }

        /// <summary>
        /// 返回当前页面是否是跨站提交
        /// </summary>
        /// <returns>当前页面是否是跨站提交</returns>
        public static bool IsCrossSitePost()
        {
            // 如果不是提交则为true
            if (!IsPost())
                return true;

            return IsCrossSitePost(GetUrlReferrer(), GetHost());
        }

        /// <summary>
        /// 判断是否是跨站提交
        /// </summary>
        /// <param name="urlReferrer">上个页面地址</param>
        /// <param name="host">论坛url</param>
        /// <returns>bool</returns>
        public static bool IsCrossSitePost(string urlReferrer, string host)
        {
            if (urlReferrer.Length < 7)
                return true;
            Uri u = new Uri(urlReferrer);
            return u.Host != host;
        }

        /// <summary>
        /// 获取当前应用网址
        /// </summary>
        /// <returns></returns>
        public static String GetApplicationUrl()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null) return string.Empty;
            String url = HttpContext.Current.Request.Url.IsDefaultPort
                ? HttpContext.Current.Request.Url.Host
                : string.Format("{0}:{1}", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port.ToString());
            if (HttpContext.Current.Request.ApplicationPath != "/")
                return "http://" + url + HttpContext.Current.Request.ApplicationPath;
            else return "http://" + url;
        }
        /// <summary>
        /// 获取当前应用虚拟路径
        /// </summary>
        public static string GetApplicationPath()
        {
            try
            {
                if (HttpContext.Current == null || HttpContext.Current.Request == null) return string.Empty;
                return HttpContext.Current.Request.ApplicationPath;
            }
            catch
            {

                return string.Empty;
            }


        }
        #region  htpp请求

        /// <summary>
        ///  http请求方法
        /// </summary>
        public enum EnumRequestMethod
        {
            /// <summary>
            /// 
            /// </summary>
            Get,
            /// <summary>
            /// 
            /// </summary>
            Post
        }
        /// <summary>
        /// http数据类型
        /// </summary>
        public enum EnumContentType
        {
            /// <summary>
            /// 
            /// </summary>
            Form,
            /// <summary>
            /// 
            /// </summary>
            Json,
            /// <summary>
            /// 
            /// </summary>
            Html,
            /// <summary>
            /// 
            /// </summary>
            Xml

        }

        /// <summary>
        /// 创建Url地址参数
        /// </summary>
        /// <param name="dicParms"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string CreateLinkString(IDictionary<string, string> dicParms, Encoding encoding = null)
        {
            if (dicParms == null || dicParms.Count == 0) return string.Empty;
            List<string> pairs = new List<string>();
            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("utf-8");
            }
            foreach (KeyValuePair<string, string> item in dicParms)
            {
                if (string.IsNullOrEmpty(item.Value))
                    continue;
                pairs.Add(string.Format("{0}={1}", item.Key, item.Value.Replace("+", "%2B")));//Base64字符串
            }
            return string.Join("&", pairs.ToArray());
        }

        /// <summary>
        /// 拼接NameValueCollection请求参数
        /// </summary>
        /// <param name="requestParms">请求参数</param>
        /// <returns></returns>
        public static string JoinRequestParmByNameValues(System.Collections.Specialized.NameValueCollection requestParms)
        {
            if (requestParms == null || requestParms.Keys.Count == 0) return string.Empty;
            List<string> pairs = new List<string>();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            foreach (string item in requestParms.Keys)
            {
                pairs.Add(string.Format("{0}={1}", item, requestParms[item]));
            }
            return string.Join("&", pairs.ToArray());
        }

        /// <summary>
        /// 拼接IDictionary请求参数
        /// </summary>
        /// <param name="dicParms">请求参数</param>
        /// <returns></returns>
        public static string JoinRequestParmByDic(IDictionary<string, string> dicParms)
        {
            if (dicParms == null || dicParms.Count == 0) return string.Empty;
            List<string> pairs = new List<string>();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            foreach (KeyValuePair<string, string> item in dicParms)
            {
                if (string.IsNullOrEmpty(item.Value))
                    continue;
                pairs.Add(string.Format("{0}={1}", item.Key, item.Value.Replace("+", "%2B")));//Base64字符串
            }
            return string.Join("&", pairs.ToArray());
        }

        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="requestData">请求数据</param>
        /// <param name="contentType">设置 Content-type HTTP 标头的值</param>
        /// <param name="method">请求方式</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="charset"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static string HttpRequest(string url, string requestData, EnumContentType contentType = EnumContentType.Form, EnumRequestMethod method = EnumRequestMethod.Get, int timeout = 30, string charset = "utf-8")
        {
            string rawUrl = string.Empty;
            UriBuilder uri = new UriBuilder(url);
            string result = string.Empty;
            switch (method)
            {
                case EnumRequestMethod.Get:
                    {
                        if (!string.IsNullOrEmpty(requestData))
                        {
                            uri.Query = requestData;
                        }
                    }
                    break;
            }
            HttpWebRequest http = WebRequest.Create(uri.Uri) as HttpWebRequest;
            http.ServicePoint.Expect100Continue = false;
            http.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)";
            http.Timeout = timeout * 1000;
            // http.Referer = "";
            if (string.IsNullOrEmpty(charset)) charset = "utf-8";
            Encoding encoding = Encoding.GetEncoding(charset);
            switch (method)
            {
                case EnumRequestMethod.Get:
                    {
                        http.Method = "GET";
                    }
                    break;
                case EnumRequestMethod.Post:
                    {
                        http.Method = "POST";
                        switch (contentType)
                        {
                            case EnumContentType.Form:
                                http.ContentType = "application/x-www-form-urlencoded;charset=" + charset;
                                break;
                            case EnumContentType.Json:
                                http.ContentType = "application/json";
                                break;
                            case EnumContentType.Xml:
                                http.ContentType = "text/xml";
                                break;
                            default:
                                http.ContentType = "application/x-www-form-urlencoded;charset=" + charset;
                                break;
                        }

                        byte[] bytesRequestData = encoding.GetBytes(requestData);
                        http.ContentLength = bytesRequestData.Length;
                        using (Stream requestStream = http.GetRequestStream())
                        {
                            requestStream.Write(bytesRequestData, 0, bytesRequestData.Length);
                        }
                    }
                    break;
            }
            using (WebResponse webResponse = http.GetResponse())
            {
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                    reader.Close();
                }
                webResponse.Close();
            }
            http = null;
            return result;
        }
        #endregion

        /// <summary>
        /// Author：carl.wu
        /// Date：2017-12-05
        /// Desc：获取支付宝POST,Get过来通知消息，并以排序后的“参数名=参数值”的形式组成数组
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isSort">是否排序</param>
        /// <returns>request回来的信息组成的数组</returns>
        public static IDictionary<string, string> GetRequestParams(HttpRequestBase request = null, bool isSort = false)
        {
            if (request == null)
            {
                request = new HttpRequestWrapper(HttpContext.Current.Request);
            }
            int i = 0;
            IDictionary<string, string> dicRequest = null;
            if (isSort)
            {
                dicRequest = new SortedDictionary<string, string>();
            }
            else
            {
                dicRequest = new Dictionary<string, string>();
            }
            NameValueCollection nameValues;
            if (IsPost(request))
            {
                nameValues = request.Form;
                // Get names of all forms into a string array.
                String[] requestItem = nameValues.AllKeys;
                for (i = 0; i < requestItem.Length; i++)
                {
                    dicRequest.Add(requestItem[i], request.Form[requestItem[i]]);
                }
            }
            else
            {
                nameValues = request.QueryString;
                // Get names of all forms into a string array.
                String[] requestItem = nameValues.AllKeys;
                for (i = 0; i < requestItem.Length; i++)
                {
                    dicRequest.Add(requestItem[i], request.QueryString[requestItem[i]]);
                }
            }
            return dicRequest;
        }
    }
}