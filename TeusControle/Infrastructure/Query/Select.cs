using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TeusControle.Infrastructure.Query
{
    public class Select<TEntity> : ExpressionBase<Expression<Func<TEntity, object>>> where TEntity : class
    {
        public Select(Expression<Func<TEntity, object>> expression)
        {
            Expressions = expression;
        }
    }

    public class Select : ExpressionBase<string>
    {
        public Select(DynamicSelect selectDynamics)
        {
            if (selectDynamics != null && selectDynamics.Properties != null && selectDynamics.Properties.Any())
                Expressions = new BuilderSelect(selectDynamics).Expressions;
        }
    }

    public class DynamicSelect
    {
        public List<SelectField> Properties;
    }

    public class SelectField
    {
        public string Name { get; set; }

        public string Ref { get; set; }

        public List<SelectField> Join { get; set; }

        public SelectUnaryField Unary { get; set; }

        public SelectListField List { get; set; }
    }

    public class SelectUnaryField
    {
        public string Condition { get; set; }

        public List<SelectField> Correct { get; set; }

        public List<SelectField> Incorrect { get; set; }
    }

    public class SelectListField
    {
        public string Prefix { get; set; }

        public List<SelectField> Selects { get; set; }

        public string Suffix { get; set; }
    }
}
