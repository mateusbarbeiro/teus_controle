using System;
using System.Linq.Expressions;

namespace TeusControle.Infrastructure.Query
{
    public class FilterBy<TEntity> : ExpressionBase<Expression<Func<TEntity, bool>>> where TEntity : class
    {
        public FilterBy(Expression<Func<TEntity, bool>> expression)
        {
            Expressions = expression;
        }

        public FilterBy(DynamicFilter dynamicFilter)
        {
            if (dynamicFilter == null)
                return;

            Expressions = FilterExpression<TEntity>.Build(dynamicFilter);
        }

        public FilterBy<TEntity> AddExpression(Expression<Func<TEntity, bool>> newExpression)
        {
            if (newExpression == null) throw new ArgumentNullException(nameof(newExpression), $"{nameof(newExpression)} is null.");

            if (Expressions == null) Expressions = newExpression;

            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TEntity));

            var leftVisitor = new ReplaceExpressionVisitor(newExpression.Parameters[0], parameter);
            var left = leftVisitor.Visit(newExpression.Body);

            var rightVisitor = new ReplaceExpressionVisitor(Expressions.Parameters[0], parameter);
            var right = rightVisitor.Visit(Expressions.Body);

            Expressions = System.Linq.Expressions.Expression.Lambda<Func<TEntity, bool>>(System.Linq.Expressions.Expression.AndAlso(left, right), parameter);

            return this;
        }
    }
}
