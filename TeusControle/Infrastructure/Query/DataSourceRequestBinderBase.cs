using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeusControle.Infrastructure.Conversors.Dynamic;

namespace TeusControle.Infrastructure.Query
{
    public abstract class DataSourceRequestBinderBase<TDataSource> : IModelBinder
    {
        protected List<KeyValuePair<string, StringValues>> kvps;

        public DataSourceRequestBinderBase()
        {
            kvps = new List<KeyValuePair<string, StringValues>>();
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            switch (bindingContext.HttpContext.Request.Method.ToLower())
            {
                case "post":
                    TreatPost(bindingContext);
                    break;
                case "put":
                    TreatPut(bindingContext);
                    break;
                default:
                    TreatGet(bindingContext);
                    break;
            }

            try
            {
                bindingContext.Result = ModelBindingResult.Success(TransformDataSourceRequest());
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
            }

            return Task.CompletedTask;
        }

        protected int TryGetValue(List<KeyValuePair<string, StringValues>> list, string key, int defaultValue)
        {
            int value = defaultValue;

            try
            {
                if (list.Any(x => x.Key == key))
                    value = int.Parse(kvps.Find(x => x.Key == key).Value);
            }
            catch
            {
                value = defaultValue;
            }

            return value;
        }

        protected string TryGetValue(List<KeyValuePair<string, StringValues>> list, string key, string defaultValue)
        {
            string value = defaultValue;

            try
            {
                if (list.Any(x => x.Key == key))
                    value = kvps.Find(x => x.Key == key).Value;
            }
            catch
            {
                value = defaultValue;
            }

            return value;
        }

        protected string GetDataBody(ModelBindingContext bindingContext)
        {
            var body = "";
            var request = bindingContext.ActionContext.HttpContext.Request;
            request.EnableBuffering();

            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                body = reader.ReadToEnd();
            }
            request.Body.Position = 0;

            return body;
        }

        protected virtual void TreatGet(ModelBindingContext bindingContext)
        {
            if (bindingContext.HttpContext.Request.QueryString != null)
            {
                kvps = bindingContext.ActionContext.HttpContext.Request.Query.ToList();
            }
            else if (bindingContext.HttpContext.Request.Form != null)
            {
                try
                {
                    kvps = bindingContext.ActionContext.HttpContext.Request.Form.ToList();
                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
                }
            }
            else
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "No input data");
            }
        }

        protected virtual void TreatPut(ModelBindingContext bindingContext)
        {
            TreatPost(bindingContext);
        }

        protected virtual void TreatPost(ModelBindingContext bindingContext)
        {
            if (bindingContext.HttpContext.Request.Body != null)
            {
                var body = GetDataBody(bindingContext);
                if (string.IsNullOrWhiteSpace(body))
                    return;

                try
                {
                    var valuesBody = JsonConvert.DeserializeObject<TDataSource>(body);

                    foreach (var prop in valuesBody.GetType().GetProperties())
                    {
                        var valueProp = prop.GetValue(valuesBody, null);

                        if (valueProp != null)
                        {
                            kvps.Add(new KeyValuePair<string, StringValues>(prop.Name, valueProp.ToString()));
                        }
                    }
                }
                catch
                {
                    kvps.Clear();
                }
            }
            else
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "No input data");
            }
        }

        protected abstract object TransformDataSourceRequest();

        protected DynamicFilter GetFilter()
        {
            var filter = kvps.Find(x => x.Key == "Filter").Value;
            if (!filter.Any())
                return null;

            return ConverterDynamic.Filter(filter);
        }

        /*protected DynamicGroupBy GetGroupBy()
        {
            var groupBy = kvps.Find(x => x.Key == "GroupBy").Value;
            if (!groupBy.Any())
                return null;

            return ConverterDynamic.GroupBy(groupBy);
        }*/

        protected DynamicOrderBy GetOrderBy()
        {
            var orderBy = kvps.Find(x => x.Key == "OrderBy").Value;
            if (!orderBy.Any())
                return null;

            return ConverterDynamic.OrderBy(orderBy);
        }

        protected DynamicSelect GetSelect()
        {
            var select = kvps.Find(x => x.Key == "Select").Value;
            if (!select.Any())
                return null;

            return ConverterDynamic.Select(select);
        }
    }
}