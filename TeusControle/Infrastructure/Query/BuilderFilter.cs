using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TeusControle.Infrastructure.Enum;

namespace TeusControle.Infrastructure.Query
{
    public class BuilderFilter
    {
        private MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private MethodInfo trimMethod = typeof(string).GetMethod("Trim", new Type[0]);
        private MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", new Type[0]);
        private MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        private Dictionary<Operation, Func<Expression, Expression, Expression>> Expressions;

        private Expression _expressionGenerated;

        private FilterField _statement;

        private int i = 0;

        public BuilderFilter(ParameterExpression param, FilterField statement)
        {
            PopulateExpressions();

            _statement = statement;
            _expressionGenerated = DeepTree(param, ExtractProperties.Extract(statement.PropertyName));
        }

        public Expression ToExpression() => _expressionGenerated;

        private Expression DeepTree(Expression param, InternalProperty prop)
        {
            if (prop.Chield == null)
                return GetSimpleExpression(param, prop.Property);

            if (!prop.IsList)
                return DeepTree(Expression.Property(param, prop.Property), prop.Chield);

            var type = param.Type.GetProperty(prop.Property).PropertyType.GetGenericArguments()[0];
            ParameterExpression listItemParam = Expression.Parameter(type, "i" + (++i));
            var lambda = Expression.Lambda(DeepTree(listItemParam, prop.Chield), listItemParam);
            var member = Expression.Property(param, prop.Property);
            var tipoEnumerable = typeof(Enumerable);
            var anyInfo = tipoEnumerable.GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "Any" && m.GetParameters().Count() == 2);
            anyInfo = anyInfo.MakeGenericMethod(type);

            return Expression.Call(anyInfo, member, lambda);
        }

        private Expression GetSimpleExpression(Expression param, string propertyName)
        {
            Expression member = Expression.Property(param, propertyName);

            var valueConstant = member != null && member.Type.BaseType != typeof(string)
                ? ChangeType(_statement.Value, member.Type)
                : _statement.Value;

            Expression constant = System.Linq.Expressions.Expression.Convert(Expression.Constant(valueConstant), member.Type);
            if (valueConstant is string)
            {
                var trimMemberCall = Expression.Call(member, trimMethod);
                member = Expression.Call(trimMemberCall, toLowerMethod);
                var trimConstantCall = Expression.Call(constant, trimMethod);
                constant = Expression.Call(trimConstantCall, toLowerMethod);
            }

            return Expressions[_statement.Operation].Invoke(member, constant);
        }

        private void PopulateExpressions()
        {
            Expressions = new Dictionary<Operation, Func<Expression, Expression, Expression>>();
            Expressions.Add(Operation.Equals, Expression.Equal);
            Expressions.Add(Operation.NotEquals, Expression.NotEqual);
            Expressions.Add(Operation.GreaterThan, Expression.GreaterThan);
            Expressions.Add(Operation.GreaterThanOrEquals, Expression.GreaterThanOrEqual);
            Expressions.Add(Operation.LessThan, Expression.LessThan);
            Expressions.Add(Operation.LessThanOrEquals, Expression.LessThanOrEqual);
            Expressions.Add(Operation.Contains, Contains);
            Expressions.Add(Operation.StartsWith, (member, constant) => Expression.Call(member, startsWithMethod, constant));
            Expressions.Add(Operation.EndsWith, (member, constant) => Expression.Call(member, endsWithMethod, constant));
        }

        private Expression Contains(Expression member, Expression expression)
        {
            if (expression is ConstantExpression)
            {
                var constant = (ConstantExpression)expression;
                if (constant.Value is IList && constant.Value.GetType().IsGenericType)
                {
                    var type = constant.Value.GetType();
                    var containsInfo = type.GetMethod("Contains", new[] { type.GetGenericArguments()[0] });
                    var contains = Expression.Call(constant, containsInfo, member);
                    return contains;
                }
            }

            return Expression.Call(member, containsMethod, expression);
        }

        private object ChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
                throw new ArgumentNullException("conversionType");

            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;

                var nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;

                if (conversionType.IsEnum)
                    return ConvertEnum(value);

                if (conversionType == typeof(Guid))
                    return ConvertGuid(value);
            }

            if (conversionType.IsEnum)
                return ConvertEnum(value);

            if (conversionType == typeof(Guid))
                return ConvertGuid(value);

            return Convert.ChangeType(value, conversionType);
        }

        private object ConvertGuid(object value)
        {
            if (value == null)
                return null;

            if (Guid.TryParse(value.ToString(), out Guid guidValue))
                return guidValue;
            else
                throw new InvalidCastException("Tipo do dado inválido");
        }

        private object ConvertEnum(object value)
        {
            if (value == null)
                return null;

            int enumValue = 0;
            if (int.TryParse(value.ToString(), out enumValue))
                return enumValue;
            else
                throw new InvalidCastException("Tipo do dado inválido");
        }

        internal class InternalProperty
        {
            public string Property { get; set; }

            public bool IsList { get; set; }

            public InternalProperty Chield { get; set; }
        }

        internal class ExtractProperties
        {
            public static InternalProperty Extract(string value)
            {
                return new ExtractProperties().GetProperties(value);
            }

            private InternalProperty GetProperties(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;

                int pos = 0;
                bool isList = false;

                var father = new StringBuilder();

                foreach (var item in value.ToCharArray())
                {
                    if (item == '.')
                        break;

                    if (item == '[')
                    {
                        isList = true;
                        break;
                    }

                    father.Append(item);

                    pos++;
                }

                var startSub = pos + 1;
                var lengthSub = value.Length - startSub;
                if (isList)
                    lengthSub -= 1;

                return new InternalProperty
                {
                    Property = father.ToString(),
                    IsList = isList,
                    Chield = (lengthSub > 0) ? GetProperties(value.Substring(startSub, lengthSub)) : null
                };
            }

            private bool IsList(string value)
            {
                bool isList = false;
                foreach (var item in value.ToCharArray())
                {
                    if (item == '.')
                        break;

                    if (item == '[')
                    {
                        isList = true;
                        break;
                    }
                }

                return isList;
            }
        }
    }
}
