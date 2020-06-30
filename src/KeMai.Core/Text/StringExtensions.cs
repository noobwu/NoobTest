using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.String;
namespace KeMai.Text
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class StringExtensions
    {
        private static readonly char[] SystemTypeChars = { '<', '>', '+' };
        private static readonly Regex SplitCamelCaseRegex = new Regex("([A-Z]|[0-9]+)", RegexOptions.IgnoreCase);
        #region Convert
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T To<T>(this string value)
        {
            return To(value, default(T));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T To<T>(this string value, T defaultValue)
        {
            try
            {
                return string.IsNullOrEmpty(value) ? defaultValue : (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T To<T>(this object value)
        {
            return To(value, default(T));

        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T To<T>(this object value, T defaultValue)
        {
            try
            {
                return value == null ? defaultValue : (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int ToInt(this string text)
        {
            return To<int>(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string text, int defaultValue)
        {
            int ret;
            return int.TryParse(text, out ret) ? ret : defaultValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static long ToInt64(this string text)
        {
            return To<long>(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToInt64(this string text, long defaultValue)
        {
            long ret;
            return long.TryParse(text, out ret) ? ret : defaultValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static float ToFloat(this string text)
        {
            return To<float>(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static float ToFloatInvariant(this string text)
        {
            return text == null ? default(float) : float.Parse(text, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float ToFloat(this string text, float defaultValue)
        {
            float ret;
            return float.TryParse(text, out ret) ? ret : defaultValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static double ToDouble(this string text)
        {
            return To<double>(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static double ToDoubleInvariant(this string text)
        {
            return text == null ? default(double) : double.Parse(text, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble(this string text, double defaultValue)
        {
            double ret;
            return double.TryParse(text, out ret) ? ret : defaultValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string text)
        {
            return To<decimal>(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static decimal ToDecimalInvariant(this string text)
        {
            return text == null ? default(decimal) : decimal.Parse(text, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string text, decimal defaultValue)
        {
            decimal ret;
            return decimal.TryParse(text, out ret) ? ret : defaultValue;
        }
        #endregion Convert
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FromUtf8Bytes(this byte[] bytes)
        {
            return bytes == null ? null
                : Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToUtf8Bytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="intVal"></param>
        /// <returns></returns>
        public static byte[] ToUtf8Bytes(this int intVal)
        {
            return FastToUtf8Bytes(intVal.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="longVal"></param>
        /// <returns></returns>
        public static byte[] ToUtf8Bytes(this long longVal)
        {
            return FastToUtf8Bytes(longVal.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ulongVal"></param>
        /// <returns></returns>
        public static byte[] ToUtf8Bytes(this ulong ulongVal)
        {
            return FastToUtf8Bytes(ulongVal.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doubleVal"></param>
        /// <returns></returns>
        public static byte[] ToUtf8Bytes(this double doubleVal)
        {
            var doubleStr = doubleVal.ToString(CultureInfo.InvariantCulture.NumberFormat);

            if (doubleStr.IndexOf('E') != -1 || doubleStr.IndexOf('e') != -1)
                doubleStr = DoubleConverter.ToExactString(doubleVal);

            return FastToUtf8Bytes(doubleStr);
        }
        /// <summary>
        /// from JWT spec
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToBase64UrlSafe(this byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.LeftPart('='); // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }
        /// <summary>
        /// from JWT spec
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] FromBase64UrlSafe(this string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break;  // One pad char
                default: throw new Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public static string LeftPart(this string strVal, char needle)
        {
            if (strVal == null) return null;
            var pos = strVal.IndexOf(needle);
            return pos == -1
                ? strVal
                : strVal.Substring(0, pos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public static string LeftPart(this string strVal, string needle)
        {
            if (strVal == null) return null;
            var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
            return pos == -1
                ? strVal
                : strVal.Substring(0, pos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public static string RightPart(this string strVal, char needle)
        {
            if (strVal == null) return null;
            var pos = strVal.IndexOf(needle);
            return pos == -1
                ? strVal
                : strVal.Substring(pos + 1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>

        public static string RightPart(this string strVal, string needle)
        {
            if (strVal == null) return null;
            var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
            return pos == -1
                ? strVal
                : strVal.Substring(pos + needle.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>

        public static string LastLeftPart(this string strVal, char needle)
        {
            if (strVal == null) return null;
            var pos = strVal.LastIndexOf(needle);
            return pos == -1
                ? strVal
                : strVal.Substring(0, pos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public static string LastLeftPart(this string strVal, string needle)
        {
            if (strVal == null) return null;
            var pos = strVal.LastIndexOf(needle, StringComparison.OrdinalIgnoreCase);
            return pos == -1
                ? strVal
                : strVal.Substring(0, pos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public static string LastRightPart(this string strVal, char needle)
        {
            if (strVal == null) return null;
            var pos = strVal.LastIndexOf(needle);
            return pos == -1
                ? strVal
                : strVal.Substring(pos + 1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public static string LastRightPart(this string strVal, string needle)
        {
            if (strVal == null) return null;
            var pos = strVal.LastIndexOf(needle, StringComparison.OrdinalIgnoreCase);
            return pos == -1
                ? strVal
                : strVal.Substring(pos + needle.Length);
        }
        /// <summary>
        /// Skip the encoding process for 'safe strings' 
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns></returns>
        private static byte[] FastToUtf8Bytes(string strVal)
        {
            var bytes = new byte[strVal.Length];
            for (var i = 0; i < strVal.Length; i++)
                bytes[i] = (byte)strVal[i];

            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public static string[] SplitOnFirst(this string strVal, char needle)
        {
            if (strVal == null) return TypeConstants.EmptyStringArray;
            var pos = strVal.IndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public static string[] SplitOnFirst(this string strVal, string needle)
        {
            if (strVal == null) return TypeConstants.EmptyStringArray;
            var pos = strVal.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + needle.Length) };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public static string[] SplitOnLast(this string strVal, char needle)
        {
            if (strVal == null) return TypeConstants.EmptyStringArray;
            var pos = strVal.LastIndexOf(needle);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + 1) };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVal"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public static string[] SplitOnLast(this string strVal, string needle)
        {
            if (strVal == null) return TypeConstants.EmptyStringArray;
            var pos = strVal.LastIndexOf(needle, StringComparison.OrdinalIgnoreCase);
            return pos == -1
                ? new[] { strVal }
                : new[] { strVal.Substring(0, pos), strVal.Substring(pos + needle.Length) };
        }
        public static string SplitCamelCase(this string value)
        {
            return SplitCamelCaseRegex.Replace(value, " $1").TrimStart();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsUserType(this Type type)
        {
            return type.IsClass
                && !type.IsSystemType();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSystemType(this Type type)
        {
            return type.Namespace == null
                || type.Namespace.StartsWith("System")
                || type.Name.IndexOfAny(SystemTypeChars) >= 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToRot13(this string value)
        {
            var array = value.ToCharArray();
            for (var i = 0; i < array.Length; i++)
            {
                var number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                    number += (number > 'm') ? -13 : 13;

                else if (number >= 'A' && number <= 'Z')
                    number += (number > 'M') ? -13 : 13;

                array[i] = (char)number;
            }
            return new string(array);
        }

        public static string FormatWith(this string text, params object[] args)
        {
            return Format(text, args);
        }

        public static string Fmt(this string text, params object[] args)
        {
            return Format(text, args);
        }

        public static string Fmt(this string text, object arg1)
        {
            return Format(text, arg1);
        }

        public static string Fmt(this string text, object arg1, object arg2)
        {
            return  Format(text, arg1, arg2);
        }

        public static string Fmt(this string text, object arg1, object arg2, object arg3)
        {
            return Format(text, arg1, arg2, arg3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string SafeSubstring(this string value, int startIndex)
        {
            return SafeSubstring(value, startIndex, value.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SafeSubstring(this string value, int startIndex, int length)
        {
            if (String.IsNullOrEmpty(value)) return Empty;
            if (startIndex < 0) startIndex = 0;
            if (value.Length >= (startIndex + length))
                return value.Substring(startIndex, length);

            return value.Length > startIndex ? value.Substring(startIndex) : Empty;
        }

        private const int LowerCaseOffset = 'a' - 'A';
        public static string ToCamelCase(this string value)
        {
            if (String.IsNullOrEmpty(value)) return value;

            var len = value.Length;
            var newValue = new char[len];
            var firstPart = true;

            for (var i = 0; i < len; ++i)
            {
                var c0 = value[i];
                var c1 = i < len - 1 ? value[i + 1] : 'A';
                var c0isUpper = c0 >= 'A' && c0 <= 'Z';
                var c1isUpper = c1 >= 'A' && c1 <= 'Z';

                if (firstPart && c0isUpper && (c1isUpper || i == 0))
                    c0 = (char)(c0 + LowerCaseOffset);
                else
                    firstPart = false;

                newValue[i] = c0;
            }

            return new string(newValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string value)
        {
            if (String.IsNullOrEmpty(value)) return value;

            if (value.IndexOf('_') >= 0)
            {
                var parts = value.Split('_');
                var sb = StringBuilderThreadStatic.Allocate();
                foreach (var part in parts)
                {
                    var str = part.ToCamelCase();
                    sb.Append(char.ToUpper(str[0]) + str.SafeSubstring(1, str.Length));
                }
                return StringBuilderThreadStatic.ReturnAndFree(sb);
            }

            var camelCase = value.ToCamelCase();
            return char.ToUpper(camelCase[0]) + camelCase.SafeSubstring(1, camelCase.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string value)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value).Replace("_", String.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLowercaseUnderscore(this string value)
        {
            if (String.IsNullOrEmpty(value)) return value;
            value = value.ToCamelCase();

            var sb = StringBuilderThreadStatic.Allocate();
            foreach (char t in value)
            {
                if (char.IsDigit(t) || (char.IsLetter(t) && char.IsLower(t)) || t == '_')
                {
                    sb.Append(t);
                }
                else
                {
                    sb.Append("_");
                    sb.Append(char.ToLowerInvariant(t));
                }
            }
            return StringBuilderThreadStatic.ReturnAndFree(sb);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startsWith"></param>
        /// <returns></returns>
        public static bool StartsWithIgnoreCase(this string text, string startsWith)
        {
            return text != null
                && text.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase);
        }
		/// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var list = id.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (list == null || list.Length == 0)
            {
                return null;
            }
            var ret = new List<T>();
            if (list == null) return ret;

            foreach (var item in list)
            {
                if (item == null) continue;

                var arr = item as IEnumerable;
                if (arr != null && !(item is string))
                {
                    ret.AddRange(arr.Cast<T>());
                }
                else
                {
                    ret.Add(item.To<T>());
                }
            }
            return ret.Distinct().ToList();
        }

    }
}
