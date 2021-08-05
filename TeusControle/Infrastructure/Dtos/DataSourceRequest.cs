using Microsoft.AspNetCore.Mvc;
using TeusControle.Infrastructure.Query;

namespace TeusControle.Infrastructure.Dtos
{
    [ModelBinder(typeof(DataSourceRequestBinder<DataSourceRequest>))]
    public class DataSourceRequest : DataSourceRequestBase
    {
        public int Page { get; set; }

        public int PageSize { get; set; }
    }

    public abstract class DataSourceRequestBase
    {
        public dynamic Filter { get; set; }

        public dynamic OrderBy { get; set; }
    }
}
