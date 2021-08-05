using System;

namespace TeusControle.Infrastructure.Query
{
    public class DynamicFilter
    {
        /// <summary>
        /// Tipo
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Valor
        /// </summary>
        public dynamic Value { get; set; }
    }
}
