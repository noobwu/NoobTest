using System;

namespace KeMai.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DefaultAttribute : AttributeBase
    {
        /// <summary>
        /// 
        /// </summary>
        public int IntValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double DoubleValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Type DefaultType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool OnUpdate { get; set; }

        public DefaultAttribute(int intValue)
        {
            this.IntValue = intValue;
            this.DefaultType = typeof(int);
            this.DefaultValue = this.IntValue.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doubleValue"></param>
        public DefaultAttribute(double doubleValue)
        {
            this.DoubleValue = doubleValue;
            this.DefaultType = typeof(double);
            this.DefaultValue = doubleValue.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultValue"></param>
        public DefaultAttribute(string defaultValue)
        {
            this.DefaultType = typeof(string);
            this.DefaultValue = defaultValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultType"></param>
        /// <param name="defaultValue"></param>
        public DefaultAttribute(Type defaultType, string defaultValue)
        {
            this.DefaultValue = defaultValue;
            this.DefaultType = defaultType;
        }
    }
}