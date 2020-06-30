using System;
using System.Collections.Generic;
using System.Linq;

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public class ModelDefinition
    {
        /// <summary>
        /// 
        /// </summary>
        public ModelDefinition()
        {
            FieldDefinitions = new List<FieldDefinition>();
            IgnoredFieldDefinitions = new List<FieldDefinition>();
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public string Alias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Type ModelType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public const string RowVersionName = "RowVersion";

        /// <summary>
        /// 
        /// </summary>
        public bool HasAutoIncrementId
        {
            get { return PrimaryKey != null && PrimaryKey.AutoIncrement; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ModelName
        {
            get
            {
                return Alias ?? Name;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<FieldDefinition> IgnoredFieldDefinitions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FieldDefinition> FieldDefinitions { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FieldDefinition RowVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FieldDefinition PrimaryKey
        {
            get { return this.FieldDefinitions.First(x => x.IsPrimaryKey); }
        }
        public FieldDefinition[] FieldDefinitionsWithAliases { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public object GetPrimaryKey(object instance)
        {
            var pk = PrimaryKey;
            return pk != null
                ? pk.GetValue(instance)
                : instance.GetId();
        }
        private readonly object fieldDefLock = new object();
        private Dictionary<string, FieldDefinition> fieldDefinitionMap;
        private Func<string, string> fieldNameSanitizer;
        public Dictionary<string, FieldDefinition> GetFieldDefinitionMap(Func<string, string> sanitizeFieldName)
        {
            lock (fieldDefLock)
            {
                if (fieldDefinitionMap != null && fieldNameSanitizer == sanitizeFieldName)
                    return fieldDefinitionMap;

                fieldDefinitionMap = new Dictionary<string, FieldDefinition>(StringComparer.OrdinalIgnoreCase);
                fieldNameSanitizer = sanitizeFieldName;
                foreach (var fieldDef in FieldDefinitions)
                {
                    fieldDefinitionMap[sanitizeFieldName(fieldDef.FieldName)] = fieldDef;
                }
                return fieldDefinitionMap;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public FieldDefinition GetFieldDefinition(Func<string, bool> predicate)
        {
            foreach (var f in FieldDefinitionsWithAliases)
            {
                if (predicate(f.Alias))
                    return f;
            }
            foreach (var f in FieldDefinitions)
            {
                if (predicate(f.Name))
                    return f;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public FieldDefinition GetFieldDefinition(string fieldName)
        {
            if (fieldName != null)
            {
                foreach (var f in FieldDefinitionsWithAliases)
                {
                    if (f.Alias == fieldName)
                        return f;
                }
                foreach (var f in FieldDefinitions)
                {
                    if (f.Name == fieldName)
                        return f;
                }
                foreach (var f in FieldDefinitionsWithAliases)
                {
                    if (string.Equals(f.Alias, fieldName, StringComparison.OrdinalIgnoreCase))
                        return f;
                }
                foreach (var f in FieldDefinitions)
                {
                    if (string.Equals(f.Name, fieldName, StringComparison.OrdinalIgnoreCase))
                        return f;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public void AfterInit()
        {
            FieldDefinitionsWithAliases = FieldDefinitions.Where(x => x.Alias != null).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }


    }
}
