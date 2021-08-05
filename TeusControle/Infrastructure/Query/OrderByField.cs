using TeusControle.Infrastructure.Enum;

namespace TeusControle.Infrastructure.Query
{
    public class OrderByField : OrderByFieldGeneric<OrderByDirection>
    {
        public bool IsAscending
        {
            get
            {
                return (Dir == OrderByDirection.Ascending);
            }
        }

        public OrderByField() { }

        public OrderByField(string propertyName, OrderByDirection direction)
        {
            this.PropertyName = propertyName;
            this.Dir = direction;
        }
    }

    public class OrderByFieldRaw : OrderByFieldGeneric<string>
    {
        public OrderByDirection TreatDir()
        {
            if (Dir.ToUpper().HasValueList(new string[] { "0", "A", "ASC", "ASCENDING" }))
                return OrderByDirection.Ascending;
            else
                return OrderByDirection.Descending;
        }
    }

    public class OrderByFieldGeneric<TDir>
    {
        public string PropertyName { get; set; }

        public TDir Dir { get; set; }
    }

    /// <summary>
    /// Classe que contem implementações genéricas para qualquer objeto
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Testa se o valor em questão existe na listagem passada por parametro
        /// </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool HasValueList<T>(this T value, T[] values)
        {
            if (value == null)
                return false;

            bool has = false;

            foreach (var item in values)
            {
                if (item.Equals(value))
                {
                    has = true;
                    break;
                }
            }

            return has;
        }
    }
}