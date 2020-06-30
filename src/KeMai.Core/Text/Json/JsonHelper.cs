using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace KeMai.Text
{
    /// <summary>
    /// Defines helper methods to work with JSON.
    /// </summary>
    public static class JsonHelper
    {
        private const char TypeSeperator = '|';

        /// <summary>
        /// Serializes an object with a type information included.
        /// So, it can be deserialized using <see cref="DeserializeWithType"/> method later.
        /// </summary>
        public static string SerializeWithType(object obj)
        {
            return SerializeWithType(obj, obj.GetType());
        }

        /// <summary>
        /// Serializes an object with a type information included.
        /// So, it can be deserialized using <see cref="DeserializeWithType"/> method later.
        /// </summary>
        public static string SerializeWithType(object obj, Type type)
        {
            var serialized = obj.ToJsonString();

            return string.Format(
                "{0}{1}{2}",
                type.AssemblyQualifiedName,
                TypeSeperator,
                serialized
                );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeByDataContract(object obj)
        {
#if NET45
            DataContractJsonSerializerSettings serializerSettings = new DataContractJsonSerializerSettings();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType(), serializerSettings);
#else
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
#endif

            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }

        }
        /// <summary>
        /// JSON反序列化
        /// <param name="strJson"></param>
        /// </summary>
        public static T DeserializeByDataContract<T>(string strJson)
        {
            if (string.IsNullOrEmpty(strJson)) return default(T);
#if NET45
            DataContractJsonSerializerSettings serializerSettings = new DataContractJsonSerializerSettings();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T), serializerSettings);
#else
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
#endif

            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(strJson)))
            {
                return (T)serializer.ReadObject(memoryStream);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeByDefault(object obj)
        {
            if (obj == null) return null;
            return JsonConvert.SerializeObject(obj, Formatting.None, DefaultSettings());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static JsonSerializerSettings DefaultSettings()
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore   //主要是这一句
            };
            jsonSetting.Converters.Clear();
            jsonSetting.Converters.Add(new IsoDateTimeConverter()
            {
                DateTimeFormat = "yyyy'-'MM'-'dd HH:mm:ss"
            });
            return jsonSetting;
        }
        /// <summary>
        /// Deserializes an object serialized with <see cref="SerializeWithType(object)"/> methods.
        /// </summary>
        public static T DeserializeWithType<T>(string serializedObj)
        {
            return (T)DeserializeWithType(serializedObj);
        }

        /// <summary>
        /// Deserializes an object serialized with <see cref="SerializeWithType(object)"/> methods.
        /// </summary>
        public static object DeserializeWithType(string serializedObj)
        {
            var typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator);
            var type = Type.GetType(serializedObj.Substring(0, typeSeperatorIndex));
            var serialized = serializedObj.Substring(typeSeperatorIndex + 1);

            var options = new JsonSerializerSettings();
            //options.Converters.Insert(0, new AbpDateTimeConverter());

            return JsonConvert.DeserializeObject(serialized, type, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static bool ValidateJson(this string strJson)
        {
            if (string.IsNullOrEmpty(strJson)) return false;
            try
            {
                dynamic tmpObj = JsonConvert.DeserializeObject<dynamic>(strJson, DefaultSettings());
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
