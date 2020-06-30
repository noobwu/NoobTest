using System;

namespace KeMai.DataAnnotations
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Struct)]
    public class AliasAttribute : AttributeBase
    {
        public string Name { get; set; }

        public AliasAttribute(string name)
        {
            this.Name = name;
        }
    }
}