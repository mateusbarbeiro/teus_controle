using System.ComponentModel;

namespace TeusControle.Infrastructure.Enum
{
    public enum Operation
    {
        /// <summary>
        /// 1 - Igual
        /// </summary>
        [Description("Igual")]
        Equals,

        /// <summary>
        /// 2 - Contém (apenas string)
        /// </summary>
        [Description("Contém")]
        Contains,

        /// <summary>
        /// 3 - Inicia com (apenas string)
        /// </summary>
        [Description("Inicia com")]
        StartsWith,

        /// <summary>
        /// 4 - Termina com (apenas string)
        /// </summary>
        [Description("Termina com")]
        EndsWith,

        /// <summary>
        /// 5 - Diferente
        /// </summary>
        [Description("Diferente")]
        NotEquals,

        /// <summary>
        /// 6 - Maior do que
        /// </summary>
        [Description("Maior do que")]
        GreaterThan,

        /// <summary>
        /// 7 - Maior e igual a
        /// </summary>
        [Description("Maior e igual a")]
        GreaterThanOrEquals,

        /// <summary>
        /// 8 - Menor do que
        /// </summary>
        [Description("Menor do que")]
        LessThan,

        /// <summary>
        /// 9 - Menor ou igual a
        /// </summary>
        [Description("Menor ou igual a")]
        LessThanOrEquals
    }
}
