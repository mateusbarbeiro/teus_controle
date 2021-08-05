using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TeusControle.Infrastructure.Query;

namespace TeusControle.Infrastructure.Conversors.Dynamic
{
    public static class ConverterDynamic
    {
        public static DynamicFilter Filter(string value)
        {
            if (value == null)
                return null;

            var dynamicFilter = new DynamicFilter();

            try
            {
                TreatFilterOperation(
                    dynamicFilter,
                    JsonConvert.DeserializeObject<FilterConnectorRaw>(value)
                );
            }
            catch
            {
                dynamicFilter = null;
            }

            return dynamicFilter;
        }

        private static void TreatFilterOperation(DynamicFilter dynamicFilter, FilterConnectorRaw value)
        {
            var fc = new FilterConnector();
            fc.Connector = value.TreatConnector();
            fc.Values = new List<DynamicFilter>();

            dynamicFilter.Type = typeof(FilterConnector);
            dynamicFilter.Value = fc;

            foreach (object val in value.Values)
            {
                var dyn = new DynamicFilter();

                try
                {
                    var filterField = JsonConvert.DeserializeObject<FilterField>(val.ToString());
                    if (filterField.PropertyName != null)
                    {
                        dyn.Type = typeof(FilterField);
                        dyn.Value = filterField;
                    }
                    else
                    {
                        var converter = JsonConvert.DeserializeObject<FilterConnectorRaw>(val.ToString());
                        TreatFilterOperation(dyn, converter);
                    }

                    fc.Values.Add(dyn);
                }
                catch
                {
                    throw new Exception("Erro na conversão de valores");
                }
            }
        }

        public static DynamicOrderBy OrderBy(string value)
        {
            if (value == null)
                return null;

            var list = new List<OrderByFieldRaw>();
            try
            {
                list = JsonConvert.DeserializeObject<List<OrderByFieldRaw>>(value);
            }
            catch { }

            return new DynamicOrderBy(list);
        }

        public static DynamicSelect Select(string value)
        {
            if (value == null)
                return null;

            DynamicSelect dynamicSelect = null;

            try
            {
                dynamicSelect = JsonConvert.DeserializeObject<DynamicSelect>(value);
            }
            catch { }

            return dynamicSelect;
        }

        internal class InternalSelectField
        {
            public List<SelectField> Properties { get; set; }

            public InternalSelectField()
            {
                Properties = new List<SelectField>();
            }
        }
    }
}
