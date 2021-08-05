using TeusControle.Infrastructure.Dtos;

namespace TeusControle.Infrastructure.Query
{
    public class DataSourceRequestBinder<TDataSource> : DataSourceRequestBinderBase<TDataSource>
    {
        protected override object TransformDataSourceRequest()
        {
            var ds = new DataSourceRequest();

            GetValueHeaders(ds);

            return ds;
        }

        protected void GetValueHeaders(DataSourceRequest ds)
        {
            const int PAGE = 1;
            const int PAGE_SIZE = 10;

            ds.Page = TryGetValue(kvps, "Page", PAGE);
            if (ds.Page < PAGE)
                ds.Page = PAGE;

            ds.PageSize = TryGetValue(kvps, "PageSize", PAGE_SIZE);
            if (ds.PageSize < PAGE)
                ds.PageSize = PAGE_SIZE;

            ds.OrderBy = GetOrderBy();
            ds.Filter = GetFilter();
        }
    }
}