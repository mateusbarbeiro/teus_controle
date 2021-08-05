using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TeusControle.Infrastructure.Query
{
    public static class BuilderOrderBy
    {
        public static string GetOrderBy(IList<OrderByField> sortFields)
        {
            if (sortFields == null || !sortFields.Any())
                return "";

            var token = new StringBuilder();

            var isDescending = false;
            var isLast = false;
            var lastValue = sortFields.Count - 1;
            var ext = "";
            for (int i = 0; i < sortFields.Count; i++)
            {
                var field = sortFields[i];
                isLast = (i == lastValue);
                isDescending = !field.IsAscending;
                ext = "";

                if (isDescending)
                    ext += " descending";

                if (!isLast)
                    ext += ", ";

                token.Append($"{field.PropertyName}{ext}");
            }

            return token.ToString();
        }

        public static Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> GetOrderBy<TEntity>(IEnumerable<OrderByField> sortFields)
        {
            Type typeQueryable = typeof(IQueryable<TEntity>);
            ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p");
            var outerExpression = Expression.Lambda(argQueryable, argQueryable);

            bool isFirst = true;
            Expression exp = outerExpression.Body;
            foreach (var field in sortFields)
            {
                exp = GetExpression<TEntity>(exp, field, isFirst);
                isFirst = false;
            }

            var finalLambda = Expression.Lambda(exp, argQueryable);

            return (Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>)finalLambda.Compile();
        }

        private static Expression GetExpression<TEntity>(Expression father, OrderByField field, bool isFirst)
        {
            var entityType = typeof(TEntity);
            ParameterExpression arg = Expression.Parameter(entityType, "x");

            Expression expression = arg;
            string[] properties = field.PropertyName.Split('.');
            foreach (string propertyName in properties)
            {
                PropertyInfo propertyInfo = entityType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                expression = Expression.Property(expression, propertyInfo);
                entityType = propertyInfo.PropertyType;
            }

            LambdaExpression lambda = Expression.Lambda(expression, arg);

            string methodName = "";
            if (isFirst)
                methodName = field.IsAscending ? "OrderBy" : "OrderByDescending";
            else
                methodName = field.IsAscending ? "ThenBy" : "ThenByDescending";

            MethodCallExpression resultExp = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(TEntity), entityType },
                father,
                Expression.Quote(lambda)
            );

            return resultExp;
        }
    }
}
