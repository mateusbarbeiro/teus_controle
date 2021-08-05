using System;
using System.Collections.Generic;
using System.Linq;
using TeusControle.Infrastructure.Enum;

namespace TeusControle.Infrastructure.Query
{
    public class OrderBy<TEntity> : ExpressionBase<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> where TEntity : class
    {
        public OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> expression)
        {
            Expressions = expression;
        }

        public OrderBy(string propertyName, OrderByDirection direction)
        {
            Expressions = BuilderOrderBy.GetOrderBy<TEntity>(new List<OrderByField>
            {
                new OrderByField(propertyName, direction)
            });
        }

        public OrderBy(DynamicOrderBy sortDynamics)
        {
            if (sortDynamics != null && sortDynamics.SortFields != null && sortDynamics.SortFields.Any())
                Expressions = BuilderOrderBy.GetOrderBy<TEntity>(sortDynamics.SortFields);
        }
    }

    public class OrderBy : ExpressionBase<string>
    {
        public OrderBy(DynamicOrderBy sortDynamics)
        {
            if (sortDynamics != null && sortDynamics.SortFields != null && sortDynamics.SortFields.Any())
                Expressions = BuilderOrderBy.GetOrderBy(sortDynamics.SortFields);
        }
    }

    public class DynamicOrderBy
    {
        public IList<OrderByField> SortFields { get; set; }

        public DynamicOrderBy()
        {
            SortFields = new List<OrderByField>();
        }

        public DynamicOrderBy(List<OrderByField> sortFields)
        {
            SortFields = sortFields;
        }

        public DynamicOrderBy(List<OrderByFieldRaw> sortFieldsRaw) : this()
        {
            if (sortFieldsRaw == null || !sortFieldsRaw.Any())
                return;

            sortFieldsRaw.ForEach(x => SortFields.Add(new OrderByField(x.PropertyName, x.TreatDir())));
        }
    }
}