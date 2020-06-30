using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KeMai.Text
{
    /// <summary>
    /// 
    /// </summary>
    public static class TypeConstants
    {
        public static readonly string[] EmptyStringArray = new string[0];
        public static readonly long[] EmptyLongArray = new long[0];
        public static readonly int[] EmptyIntArray = new int[0];
        public static readonly char[] EmptyCharArray = new char[0];
        public static readonly bool[] EmptyBoolArray = new bool[0];
        public static readonly byte[] EmptyByteArray = new byte[0];
        public static readonly object[] EmptyObjectArray = new object[0];

        public static readonly FieldInfo[] EmptyFieldInfoArray = new FieldInfo[0];
        public static readonly PropertyInfo[] EmptyPropertyInfoArray = new PropertyInfo[0];
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class TypeConstants<T>
    {
        public static readonly T[] EmptyArray = new T[0];
        public static readonly List<T> EmptyList = new List<T>(0);
        public static readonly HashSet<T> EmptyHashSet = new HashSet<T>();
    }
}
