//
// ServiceStack.OrmLite: Light-weight POCO ORM for .NET and Mono
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2013 ServiceStack, Inc. All Rights Reserved.
//
// Licensed under the same terms of ServiceStack.
//

using KeMai.DataAnnotations;
using KeMai.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace KeMai.StackExchange.DapperOrmLites
{
    /// <summary>
    /// 
    /// </summary>
    public static class OrmLiteConfigExtensions
    {
        private static Dictionary<Type, ModelDefinition> typeModelDefinitionMap = new Dictionary<Type, ModelDefinition>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objProperties"></param>
        /// <returns></returns>
        internal static bool CheckForIdField(IEnumerable<PropertyInfo> objProperties)
        {
            // Not using Linq.Where() and manually iterating through objProperties just to avoid dependencies on System.Xml??
            foreach (var objProperty in objProperties)
            {
                if (objProperty.Name != OrmLiteConfig.IdField) continue;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        internal static void ClearCache()
        {
            typeModelDefinitionMap = new Dictionary<Type, ModelDefinition>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        internal static ModelDefinition GetModelDefinition(this Type modelType)
        {
            ModelDefinition modelDef;
            if (typeModelDefinitionMap.TryGetValue(modelType, out modelDef))
                return modelDef;

            if (modelType.IsValueType || modelType == typeof(string))
                return null;

            var modelAliasAttr = modelType.FirstAttribute<AliasAttribute>();
            var schemaAttr = modelType.FirstAttribute<SchemaAttribute>();
            modelDef = new ModelDefinition
            {
                ModelType = modelType,
                Name = modelType.Name,
                Alias = modelAliasAttr?.Name,
                Schema = schemaAttr?.Name,
            };

            var objProperties = modelType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance).ToList();
            var hasPkAttr = objProperties.Any(p => p.HasAttribute<PrimaryKeyAttribute>());

            var hasIdField = CheckForIdField(objProperties);

            var i = 0;
            foreach (var propertyInfo in objProperties)
            {
                if (propertyInfo.GetIndexParameters().Length > 0)
                    continue; //Is Indexer
                var computeAttr = propertyInfo.FirstAttribute<ComputeAttribute>();
                var computedAttr = propertyInfo.FirstAttribute<ComputedAttribute>();

                var isFirst = i++ == 0;

                var isPrimaryKey = (!hasPkAttr && (propertyInfo.Name == OrmLiteConfig.IdField || (!hasIdField && isFirst)))
               || propertyInfo.HasAttributeNamed(typeof(PrimaryKeyAttribute).Name);

                var isRowVersion = propertyInfo.Name == ModelDefinition.RowVersionName
                    && (propertyInfo.PropertyType == typeof(ulong) || propertyInfo.PropertyType == typeof(byte[]));

                var isNullableType = propertyInfo.PropertyType.IsNullableType();

                var isNullable = (!propertyInfo.PropertyType.IsValueType
                                   && !propertyInfo.HasAttributeNamed(typeof(RequiredAttribute).Name))
                                   || isNullableType;

                var propertyType = isNullableType
                    ? Nullable.GetUnderlyingType(propertyInfo.PropertyType)
                    : propertyInfo.PropertyType;

                Type treatAsType = null;
                if (propertyType.IsEnumFlags() || propertyType.HasAttribute<EnumAsIntAttribute>())
                    treatAsType = Enum.GetUnderlyingType(propertyType);

                var aliasAttr = propertyInfo.FirstAttribute<AliasAttribute>();

                var defaultValueAttr = propertyInfo.FirstAttribute<DefaultAttribute>();

                var fieldDefinition = new FieldDefinition
                {
                    Name = propertyInfo.Name,
                    Alias = aliasAttr?.Name,
                    FieldType = propertyType,
                    PropertyInfo = propertyInfo,
                    IsPrimaryKey = isPrimaryKey,
                    //AutoIncrement =isPrimaryKey &&propertyInfo.HasAttribute<AutoIncrementAttribute>(),
					AutoIncrement =propertyInfo.HasAttribute<AutoIncrementAttribute>(),
                    IsRowVersion = isRowVersion,
                    GetValueFn = propertyInfo.CreateGetter(),
                    SetValueFn = propertyInfo.CreateSetter(),
                    IgnoreOnInsert = propertyInfo.HasAttribute<IgnoreOnInsertAttribute>(),
                    IgnoreOnUpdate = propertyInfo.HasAttribute<IgnoreOnUpdateAttribute>(),
                    IsComputed = computeAttr != null || computedAttr != null,
                    TreatAsType = treatAsType,
                    FieldTypeDefaultValue = propertyType.GetDefaultValue(),
                    DefaultValue = defaultValueAttr?.DefaultValue,
                };

                var isIgnored = propertyInfo.HasAttribute<IgnoreAttribute>();
                if (isIgnored)
                {
                    modelDef.IgnoredFieldDefinitions.Add(fieldDefinition);
                }
                else
                {
                    modelDef.FieldDefinitions.Add(fieldDefinition);
                }
                if (isRowVersion)
                {
                    modelDef.RowVersion = fieldDefinition;
                }
            }

            modelDef.AfterInit();

            Dictionary<Type, ModelDefinition> snapshot, newCache;
            do
            {
                snapshot = typeModelDefinitionMap;
                newCache = new Dictionary<Type, ModelDefinition>(typeModelDefinitionMap) { [modelType] = modelDef };

            } while (!ReferenceEquals(
                Interlocked.CompareExchange(ref typeModelDefinitionMap, newCache, snapshot), snapshot));

            return modelDef;
        }
    }
}