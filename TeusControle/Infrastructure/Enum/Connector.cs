using System.ComponentModel;

namespace TeusControle.Infrastructure.Enum
{
    public enum Connector
    {
        /// <summary>
        /// 0 - Conector OU
        /// </summary>
        [Description("Ou")]
        Or = 0,

        /// <summary>
        /// 1 - Conector E
        /// </summary>
        [Description("E")]
        And = 1
    }
}
