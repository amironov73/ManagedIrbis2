﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* ReflectionUtility.cs --
 *  Ars Magna project, http://arsmagna.ru
 *  ------------------------------------------------------
 *  Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AM.Logging;

using JetBrains.Annotations;

#endregion

namespace AM.Reflection
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public static class ReflectionUtility
    {
        #region Public methods

        ///// <summary>
        ///// Получение списка всех типов, загруженных на данный момент
        ///// в текущий домен.
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks>Осторожно: могут быть загружены сборки только
        ///// для рефлексии. Типы в них непригодны для использования.
        ///// </remarks>
        //public static Type[] GetAllTypes()
        //{
        //    Assembly[] assemblies = AppDomain.CurrentDomain
        //        .GetAssemblies();
        //    List<Type> result = new List<Type>();
        //    foreach (Assembly assembly in assemblies)
        //    {
        //        result.AddRange(assembly.GetTypes());
        //    }

        //    return result.ToArray();
        //}

        /// <summary>
        /// Get the custom attribute.
        /// </summary>
        public static T GetCustomAttribute<T>
            (
                Type classType
            )
            where T : Attribute
        {
            var all = classType.GetCustomAttributes
                (
                    typeof(T),
                    false
                );

            return (T)all.FirstOrDefault();
        }

        /// <summary>
        /// Get the custom attribute.
        /// </summary>
        public static T GetCustomAttribute<T>
            (
                Type classType,
                bool inherit
            )
            where T : Attribute
        {
            var all = classType.GetCustomAttributes
                (
                    typeof(T),
                    inherit
                );

            return (T) all.FirstOrDefault();
        }

        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        public static T GetCustomAttribute<T>
            (
                MemberInfo member
            )
            where T : Attribute
        {
            var all = member.GetCustomAttributes
                (
                    typeof(T),
                    false
                );

            return (T)all.FirstOrDefault();
        }

        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        public static T GetCustomAttribute<T>
            (
                FieldInfo fieldInfo
            )
            where T : Attribute
        {
            var all = fieldInfo.GetCustomAttributes
                (
                    typeof(T),
                    false
                );

            return (T)all.FirstOrDefault();
        }

        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        public static T GetCustomAttribute<T>
            (
                PropertyInfo propertyInfo
            )
            where T : Attribute
        {
            var all = propertyInfo.GetCustomAttributes
                (
                    typeof(T),
                    false
                );

            return (T) all.FirstOrDefault();
        }

        ///// <summary>
        ///// Gets the custom attribute.
        ///// </summary>
        //[CanBeNull]
        //public static T GetCustomAttribute<T>
        //    (
        //        [PropertyDescriptor propertyDescriptor
        //    )
        //    where T : Attribute
        //{
        //    return (T)propertyDescriptor.Attributes[typeof(T)];
        //}

        ///// <summary>
        ///// Get default constructor for given type.
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public static ConstructorInfo GetDefaultConstructor
        //    (
        //        Type type
        //    )
        //{
        //    ConstructorInfo result = type.GetConstructor
        //        (
        //            BindingFlags.Instance | BindingFlags.Public,
        //            null,
        //            Type.EmptyTypes,
        //            null
        //        );
        //    return result;
        //}

        /// <summary>
        /// Get field value either public or private.
        /// </summary>
        public static object? GetFieldValue<T>
            (
                T target,
                string fieldName
            )
        {
            Sure.NotNullNorEmpty(fieldName, nameof(fieldName));

            var fieldInfo = typeof(T).GetField
                (
                    fieldName,
                    BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Instance | BindingFlags.Static
                );
            if (ReferenceEquals(fieldInfo, null))
            {
                Log.Error
                    (
                        nameof(ReflectionUtility) + "::" + nameof(GetFieldValue)
                        + ": can't find field="
                        + fieldName
                    );

                throw new ArgumentException(fieldName);
            }

            return fieldInfo.GetValue(target);
        }

        /// <summary>
        /// Determines whether the specified type has the attribute.
        /// </summary>
        public static bool HasAttribute<T>
            (
                Type type,
                bool inherit
            )
            where T : Attribute
        {
            return !ReferenceEquals
                (
                    GetCustomAttribute<T>(type, inherit),
                    null
                );
        }

        /// <summary>
        /// Determines whether the specified member has the attribute.
        /// </summary>
        public static bool HasAttribute<T>
            (
                MemberInfo member
            )
            where T : Attribute
        {
            return !ReferenceEquals
                (
                    GetCustomAttribute<T>(member),
                    null
                );
        }

        /// <summary>
        /// Set field value either public or private.
        /// </summary>
        public static void SetFieldValue<TTarget, TValue>
            (
                TTarget target,
                string fieldName,
                TValue value
            )
            where TTarget : class
        {
            Sure.NotNullNorEmpty(fieldName, nameof(fieldName));

            var fieldInfo = typeof(TTarget).GetField
                (
                    fieldName,
                    BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Instance | BindingFlags.Static
                );
            if (ReferenceEquals(fieldInfo, null))
            {
                Log.Error
                    (
                        nameof(ReflectionUtility) + "::" + nameof(SetFieldValue)
                        + ": can't find field="
                        + fieldName
                    );

                throw new ArgumentException(fieldName);
            }

            fieldInfo.SetValue(target, value);
        }

        /// <summary>
        /// Get property value either public or private.
        /// </summary>
        public static object? GetPropertyValue<T>
            (
                T target,
                string propertyName
            )
        {
            Sure.NotNullNorEmpty(propertyName, nameof(propertyName));

            var propertyInfo = typeof(T).GetProperty
                (
                    propertyName,
                    BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Instance | BindingFlags.Static
                );
            if (ReferenceEquals(propertyInfo, null))
            {
                Log.Error
                    (
                        nameof(ReflectionUtility) + "::" + nameof(GetPropertyValue)
                        + ": can't find property="
                        + propertyName
                    );

                throw new ArgumentException(propertyName);
            }

            return propertyInfo.GetValue(target, null);
        }

        /// <summary>
        /// Gets the properties and fields.
        /// </summary>
        public static PropertyOrField[] GetPropertiesAndFields
            (
                Type type,
                BindingFlags bindingFlags
            )
        {
            Sure.NotNull(type, nameof(type));

            var result = new List<PropertyOrField>();
            foreach (var property in type.GetProperties(bindingFlags))
            {
                result.Add(new PropertyOrField(property));
            }
            foreach (var field in type.GetFields(bindingFlags))
            {
                result.Add(new PropertyOrField(field));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Set property value either public or private.
        /// </summary>
        public static void SetPropertyValue<TTarget, TValue>
            (
                TTarget target,
                string propertyName,
                TValue value
            )
        {
            Sure.NotNullNorEmpty(propertyName, nameof(propertyName));

            var propertyInfo = typeof(TTarget).GetProperty
                (
                    propertyName,
                    BindingFlags.Public | BindingFlags.NonPublic
                    | BindingFlags.Instance | BindingFlags.Static
                );
            if (ReferenceEquals(propertyInfo, null))
            {
                Log.Error
                    (
                        nameof(ReflectionUtility) + "::" + nameof(SetPropertyValue)
                        + ": can't find property="
                        + propertyName
                    );

                throw new ArgumentException(propertyName);
            }

            propertyInfo.SetValue(target, value, null);
        }

        #endregion
    }
}

