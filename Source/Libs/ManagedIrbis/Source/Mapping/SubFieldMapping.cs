// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

/* SubFieldMapping.cs --
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

#endregion

namespace ManagedIrbis.Mapping
{
    /// <summary>
    ///
    /// </summary>
    [PublicAPI]
    public sealed class SubFieldMapping<T>
    {
        #region Properties

        /// <summary>
        /// Subfield code.
        /// </summary>
        public char Code { get; set; }

        /// <summary>
        /// Property.
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        /// Forward converter.
        /// </summary>
        public Action<RecordField, T> ForwardConverter { get; set; }

        /// <summary>
        /// Backward converter.
        /// </summary>
        public Action<T, RecordField> BackwardConverter { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Create delegate for <see cref="BackwardConverter"/>.
        /// </summary>
        [Pure]
        [NotNull]
        [MustUseReturnValue]
        public Action<T, RecordField> CreateBackwardConverter()
        {
            //
            // What we want to get:
            //
            // field.SetSubField(code, SubFieldMapper.FromXXX(source.Property);
            //

            var sourceParameter = Expression.Parameter(typeof(T), "source");
            var fieldParameter = Expression.Parameter(typeof(RecordField), "field");
            var codeConstant = Expression.Constant(Code);
            var property = Expression.Property(sourceParameter, Property);
            var methodName = MappingUtility.GetBackwardMethodName(Property.PropertyType);
            var backwardMethod = typeof(SubFieldMapper).GetMethod
                (
                    methodName,
                    new[] { Property.PropertyType }
                );
            var setSubField = typeof(RecordField).GetMethod
                (
                    "SetSubField",
                    new[] { typeof(char), typeof(object) }
                );
            var body = Expression.Call
                (
                    fieldParameter,
                    setSubField,
                    codeConstant,
                    Expression.Call(backwardMethod, property)
                );
            var expression = Expression.Lambda<Action<T, RecordField>>
                (
                    body,
                    sourceParameter,
                    fieldParameter
                );
            var result = expression.Compile();

            return result;
        }

        /// <summary>
        /// Create delegate for <see cref="ForwardConverter"/>.
        /// </summary>
        [Pure]
        [NotNull]
        [MustUseReturnValue]
        public Action<RecordField, T> CreateForwardConverter()
        {
            //
            // What we want to get:
            //
            // target.Property = SubFieldMapper.ToXXX(field, code);
            //

            var fieldParameter = Expression.Parameter(typeof(RecordField), "field");
            var targetParameter = Expression.Parameter(typeof(T), "target");
            var codeConstant = Expression.Constant(Code);
            var property = Expression.Property(targetParameter, Property);
            var methodName = MappingUtility.GetForwardMethodName(Property.PropertyType);
            var methodInfo = typeof(SubFieldMapper).GetMethod
                (
                    methodName,
                    new[] { typeof(RecordField), typeof(char) }
                );
            var body = Expression.Assign
                (
                    property,
                    Expression.Call(methodInfo, fieldParameter, codeConstant)
                );
            var expression = Expression.Lambda<Action<RecordField, T>>
                (
                    body,
                    fieldParameter,
                    targetParameter
                );
            var result = expression.Compile();

            return result;
        }

        /// <summary>
        /// Ensure converters was created.
        /// </summary>
        [NotNull]
        public SubFieldMapping<T> EnsureConvertersCreated()
        {
            if (ReferenceEquals(ForwardConverter, null))
            {
                ForwardConverter = CreateForwardConverter();
            }

            if (ReferenceEquals(BackwardConverter, null))
            {
                BackwardConverter = CreateBackwardConverter();
            }

            return this;
        }

        #endregion
    }
}
