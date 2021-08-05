using System.Collections.Generic;
using TeusControle.Infrastructure.Enum;

namespace TeusControle.Infrastructure.Query
{
    public class FilterConnector : FilterConnectorGeneric<Connector, IList<DynamicFilter>> { }

    public class FilterConnectorRaw : FilterConnectorGeneric<string, IList<object>>
    {
        public Connector TreatConnector()
        {
            var token = Connector.ToLower().Trim();
            if (token == "and" || token == "&&" || token == "1")
                return TeusControle.Infrastructure.Enum.Connector.And;
            else
                return TeusControle.Infrastructure.Enum.Connector.Or;
        }
    }

    public class FilterConnectorGeneric<TConnector, TValues>
    {
        public TConnector Connector { get; set; }

        public TValues Values { get; set; }
    }
}
