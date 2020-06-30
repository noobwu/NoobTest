using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using KeMai.Text;
namespace KeMai
{
    /// <summary>
    /// 
    /// </summary>
    public class Utils
    {


        /// <summary>
        /// 默认时间
        /// </summary>
        public static DateTime DefaultTime = new DateTime(1900, 1, 1);
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>创建是否成功</returns>
        [DllImport("dbgHelp", SetLastError = true)]
        private static extern bool MakeSureDirectoryPathExists(string name);

        /// <summary>
        /// 建立文件夹
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CreateDir(string name)
        {
            return MakeSureDirectoryPathExists(name);
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            return MD5(str, Encoding.Default);
        }


        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (encoding == null) encoding = Encoding.Default;
            byte[] bytes = encoding.GetBytes(str);
            return MD5ByBytes(bytes);
        }

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns>MD5结果</returns>
        public static string MD5ByBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return string.Empty;
            bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += bytes[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }
        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="stream">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5ByStream(Stream stream)
        {
            byte[] bytes = new MD5CryptoServiceProvider().ComputeHash(stream);
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += bytes[i].ToString("x").PadLeft(2, '0');
            }
            return ret;
        }
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <param name="serverUtility"></param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (string.IsNullOrEmpty(strPath))
            {
                return strPath;
            }
            else
            {
                if (strPath.IndexOf("/") != 0)
                {
                    if (Path.IsPathRooted(strPath))
                    {
                        return strPath;
                    }
                }
            }
            try
            {
                if (HttpContext.Current != null)
                {
                    strPath = strPath.Replace("\\", "/");
                    string strIndex = strPath.Substring(0, 1);
                    if (strIndex != "/" && strIndex != "~")
                    {
                        strPath = "~/" + strPath;
                    }
                    return HostingEnvironment.MapPath(strPath);
                    //return HttpContext.Current.Server.MapPath(strPath);
                }
                else //非web程序引用
                {
                    return GetAppPath(strPath);
                }
            }
            catch (Exception)
            {
                return GetAppPath(strPath);
            }

        }
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <param name="serverUtility"></param>
        /// <returns>绝对路径</returns>
        public static string GetAppPath(string strPath)
        {
            if (string.IsNullOrEmpty(strPath)) return string.Empty;
            strPath = strPath.Replace("~/", "\\").Replace("/", "\\");
            if (strPath.StartsWith("\\"))
            {
                strPath = strPath.TrimStart('\\');
            }
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }


        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                    {
                        return i;
                    }
                }
                else
                {
                    if (strSearch == stringArray[i])
                    {
                        return i;
                    }
                }

            }
            return -1;
        }

        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>		
        public static int GetInArrayID(string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0;
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">字符串数组</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string[] stringarray)
        {
            return InArray(str, stringarray, false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, SplitString(stringarray, ","), false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, SplitString(stringarray, strsplit), false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, strsplit), caseInsensetive);
        }


        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                {
                    string[] tmp = { strContent };
                    return tmp;
                }
                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
            {
                return new string[0] { };
            }
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            string[] result = new string[count];

            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 填充字符串
        /// </summary>
        /// <param name="padString">填充字符串</param>
        /// <param name="count">填充的次数</param>
        public static string PadString(string padString, int count)
        {
            if (count <= 1) return padString;
            StringBuilder builder = new StringBuilder(1000);
            for (int i = 0; i < count; i++)
            {
                builder.Append(padString);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 过滤字符串数组中每个元素为合适的大小
        /// 当长度小于minLength时，忽略掉,-1为不限制最小长度
        /// 当长度大于maxLength时，取其前maxLength位
        /// 如果数组中有null元素，会被忽略掉
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="minLength">单个元素最小长度</param>
        /// <param name="maxLength">单个元素最大长度</param>
        /// <returns></returns>
        public static string[] PadStringArray(string[] strArray, int minLength, int maxLength)
        {
            if (minLength > maxLength)
            {
                int t = maxLength;
                maxLength = minLength;
                minLength = t;
            }

            int iMiniStringCount = 0;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (minLength > -1 && strArray[i].Length < minLength)
                {
                    strArray[i] = null;
                    continue;
                }
                if (strArray[i].Length > maxLength)
                {
                    strArray[i] = strArray[i].Substring(0, maxLength);
                }
                iMiniStringCount++;
            }

            string[] result = new string[iMiniStringCount];
            for (int i = 0, j = 0; i < strArray.Length && j < result.Length; i++)
            {
                if (strArray[i] != null && strArray[i] != string.Empty)
                {
                    result[j] = strArray[i];
                    j++;
                }
            }


            return result;
        }

        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {

            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|游客|^Guest");
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

        }


        /// <summary>
        /// 是否是Ip范围
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIPSect(string ip)
        {
            return Regex.IsMatch(ip,
                @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");

        }



        /// <summary>
        /// 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] iparray)
        {

            string[] userip = Utils.SplitString(ip, @".");
            for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                string[] tmpip = Utils.SplitString(iparray[ipIndex], @".");
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                    {
                        return true;
                    }

                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                        {
                            r++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                }
                if (r == 4)
                {
                    return true;
                }


            }
            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetHashCode(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
        /// <summary>
        /// string型转换为DateTime型
        /// </summary>
        /// <param name="objValue">要转换的字符串</param>
        /// <returns>转换后的DateTime类型结果</returns>
        public static DateTime StrToDateTime(object objValue)
        {
            DateTime defValue = DefaultTime;
            if (objValue == null || string.IsNullOrEmpty(objValue.ToString())) return defValue;
            DateTime result = defValue;
            DateTime.TryParse(objValue.ToString(), out result);
            return result;
        }
        /// <summary>
        /// string型转换为DateTime型
        /// </summary>
        /// <param name="objValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的DateTime类型结果</returns>
        public static DateTime StrToDateTime(object objValue, DateTime defValue)
        {
            if (defValue == null || defValue == DateTime.MinValue) defValue = DefaultTime;
            if (objValue == null || string.IsNullOrEmpty(objValue.ToString())) return defValue;
            DateTime result = defValue;
            DateTime.TryParse(objValue.ToString(), out result);
            return result;
        }
        /// <summary>
        /// string型转换为DateTime型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的DateTime类型结果</returns>
        public static DateTime StrToDateTime(string strValue, DateTime defValue)
        {
            if (string.IsNullOrEmpty(strValue)) return defValue;
            DateTime result = defValue;
            DateTime.TryParse(strValue, out result);
            return result;
        }

        /// <summary>
        /// 获取星号字符串如158****0617
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="leftLen">左边字符串长度</param>
        /// <param name="rightLen">右边字符串长度</param>
        /// <param name="asteriskLen">星号字符长度</param>
        /// <returns></returns>
        public static string GetAsteriskString(string input, int leftLen, int rightLen, int asteriskLen = 0)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            if ((input.Length <= (leftLen + rightLen)))
            {
                return input;
            }
            else
            {
                if (asteriskLen > 0)
                {
                    if (input.Length < (leftLen + rightLen + asteriskLen))
                    {
                        asteriskLen = (input.Length - leftLen - rightLen);
                    }
                }
                else
                {
                    asteriskLen = (input.Length - leftLen - rightLen);
                }
                return input.Substring(0, leftLen) + PadString("*", asteriskLen) + input.Substring(input.Length - asteriskLen);
            }

        }

        /// <summary>
        /// 转换时间为unix时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetUnixTimestamp()
        {
            //
            //tick:10000000(tick)为一秒
            DateTime originTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (DateTime.Now.ToUniversalTime().Ticks - originTime.Ticks) / TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// unix时间戳转换为DateTime
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(long timestamp)
        {
            DateTime originTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            try
            {
                DateTime dateTime = originTime.AddSeconds(timestamp).ToLocalTime();
                return dateTime;
            }
            catch
            {
                return DataValueConstants.MIN_DATE;
            }
        }
        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex = startIndex - length;
                    }
                }


                if (startIndex > str.Length)
                {
                    return "";
                }


            }
            else
            {
                if (length < 0)
                {
                    return "";
                }
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            if (str.Length - startIndex < length)
            {
                length = str.Length - startIndex;
            }

            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }



        /// <summary>
        ///  Md5签名(sign = Md5(原字符串&key=商户密钥).ToUpper())
        ///  Item1:签名信息
        ///  Item2：签名的参数
        /// </summary>
        /// <param name="dicSignParms"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Tuple<string, string> BuildSign(IDictionary<string, string> dicSignParms, string key)
        {
            StringBuilder signBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicSignParms)
            {
                if (FilerSignParam(temp))
                {
                    continue;
                }
                signBuilder.Append("&" + temp.Key + "=" + temp.Value);
            }
            signBuilder.Append("&key=" + key.Trim());
            string strSource = signBuilder.ToString().Trim('&');
            string strSign = MD5(strSource);
            return Tuple.Create(strSign, strSource);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool FilerSignParam(KeyValuePair<string, string> item)
        {
            if (string.IsNullOrEmpty(item.Value) || string.IsNullOrEmpty(item.Value) || item.Key.ToLower() == "charset" || item.Key.ToLower() == "sign")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证手机号码格式
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns></returns>
        public static bool IsValidMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;
            string pattern =@"^((13[0-9])|(14[5|7])|(15([0-3]|[5-9]))|(18[0,5-9]))\d{8}$"; //@"^(13|14|15|17|18|19)[0-9]\d{8}$";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            return Regex.IsMatch(mobile, pattern);
        }

        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;
            email = email.Trim();
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

        }
        /// <summary>
        /// 验证固定电话
        /// </summary>
        /// <param name="phone">固定电话号码</param>
        /// <returns></returns>
        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;
            string pattern = @"((\d{11})|(400\d{7})|(400-(\d{4}|\d{3})-(\d{3}|\d{4}))|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            return Regex.IsMatch(phone, pattern);
        }
    }
}
