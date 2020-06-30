using System;

namespace KeMai.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class AutoIncrementAttribute : AttributeBase
    {
    }
}