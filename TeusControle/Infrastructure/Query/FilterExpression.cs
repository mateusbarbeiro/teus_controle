using System;
using System.Linq.Expressions;
using TeusControle.Infrastructure.Enum;

namespace TeusControle.Infrastructure.Query
{
    public class FilterExpression<TEntity> where TEntity : class
    {
        public static Expression<Func<TEntity, bool>> Build(DynamicFilter dynamicFilter)
        {
            return new FilterExpression<TEntity>().ToExpression(dynamicFilter);
        }

        private Expression<Func<TEntity, bool>> ToExpression(DynamicFilter dynamicFilter)
        {
            if (dynamicFilter == null)
                throw new ArgumentNullException();

            var param = Expression.Parameter(typeof(TEntity), "x");

            var exp = InnerList(null, dynamicFilter, param);
            if (exp == null)
                exp = Expression.Constant(true);

            return System.Linq.Expressions.Expression.Lambda<Func<TEntity, bool>>(exp, param);
        }

        private Expression InnerList(Expression expression, DynamicFilter dynamicFilter, ParameterExpression param)
        {
            if (dynamicFilter == null)
                return null;

            if (dynamicFilter.Type == typeof(FilterConnector))
            {
                var obj = dynamicFilter.Value as FilterConnector;
                foreach (var value in obj.Values)
                {
                    var exp = (value.Type == typeof(FilterField))
                        ? new BuilderFilter(param, value.Value as FilterField).ToExpression()
                        : InnerList(null, value, param);

                    if (exp != null)
                    {
                        if (obj.Connector == Connector.And || expression == null)
                        {
                            if (expression == null)
                                expression = Expression.Constant(true);

                            expression = Expression.AndAlso(expression, exp);
                        }
                        else
                        {
                            expression = Expression.OrElse(expression, exp);
                        }
                    }
                }
            }

            return expression;
        }
    }

    public class FilterField
    {
        public string PropertyName { get; set; }

        public Operation Operation { get; set; }

        public object Value { get; set; }
    }
}