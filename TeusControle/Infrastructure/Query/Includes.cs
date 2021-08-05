using System;
using System.Linq;

namespace TeusControle.Infrastructure.Query
{
    public class Includes<TEntity> : ExpressionBase<Func<IQueryable<TEntity>, IQueryable<TEntity>>> where TEntity : class
    {
        public Includes(Func<IQueryable<TEntity>, IQueryable<TEntity>> expression)
        {
            Expressions = expression;
        }
    }
}
