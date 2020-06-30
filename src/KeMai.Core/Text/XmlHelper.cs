using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace KeMai.Text
{
    public class XmlHelper
    {
        private static Regex regex = new Regex("<(\\w+?)[ >]", RegexOptions.Compiled);
        private static Dictionary<string, System.Xml.Serialization.XmlSerializer> parsers = new Dictionary<string, System.Xml.Serialization.XmlSerializer>();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="body"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static T DeserializeFromString<T>(string body, string charset)
        {
            System.Xml.Serialization.XmlSerializer serializer = null;
            string rootTagName = GetRootElement(body);

            bool inc = parsers.TryGetValue(rootTagName, out serializer);
            if (!inc || serializer == null)
            {
                XmlAttributes rootAttrs = new XmlAttributes();
                rootAttrs.XmlRoot = new XmlRootAttribute(rootTagName);

                XmlAttributeOverrides attrOvrs = new XmlAttributeOverrides();
                attrOvrs.Add(typeof(T), rootAttrs);

                serializer = new System.Xml.Serialization.XmlSerializer(typeof(T), attrOvrs);
                parsers[rootTagName] = serializer;
            }

            object obj = null;
            Encoding encoding = null;
            if (string.IsNullOrEmpty(charset))
            {
                encoding = Encoding.UTF8;
            }
            else
            {
                encoding = Encoding.GetEncoding(charset);
            }
            using (Stream stream = new MemoryStream(encoding.GetBytes(body)))
            {
                obj = serializer.Deserialize(stream);
            }

            return (T)obj;
        }

        /// <summary>
        /// 获取XML响应的根节点名称
        /// </summary>
        private static string GetRootElement(string body)
        {
            Match match = regex.Match(body);
            if (match.Success)
            {
                return match.Groups[1].ToString();
            }
            else
            {
                throw new Exception("Invalid XML response format!");
            }
        }
        /// <summary>
        /// xml序列化成字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>xml字符串</returns>
        public static string Serialize(object obj)
        {
            string returnStr = "";
            System.Xml.Serialization.XmlSerializer serializer = GetSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            XmlTextWriter xtw = null;
            StreamReader sr = null;
            try
            {
                xtw = new System.Xml.XmlTextWriter(ms, Encoding.UTF8);
                xtw.Formatting = System.Xml.Formatting.Indented;
                serializer.Serialize(xtw, obj);
                ms.Seek(0, SeekOrigin.Begin);
                sr = new StreamReader(ms);
                returnStr = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
                if (sr != null)
                    sr.Close();
                ms.Close();
            }
            return returnStr;

        }
        private static System.Xml.Serialization.XmlSerializer GetSerializer(Type t)
        {
            string typeHash = t.GetHashCode().ToString();

            if (!parsers.ContainsKey(typeHash))
                parsers.Add(typeHash, new System.Xml.Serialization.XmlSerializer(t));

            return parsers[typeHash];
        }

    }
}
