namespace TeusControle.Infrastructure.Query
{
    public abstract class ExpressionBase<TExpression>
    {
        public TExpression Expressions { get; protected set; }
    }
}
