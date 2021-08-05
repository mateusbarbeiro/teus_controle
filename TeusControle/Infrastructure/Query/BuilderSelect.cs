using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeusControle.Infrastructure.Query
{
    public class BuilderSelect
    {
        public string Expressions { get; private set; }

        public BuilderSelect(DynamicSelect dynamicSelect)
        {
            Expressions = TreatRoot(dynamicSelect.Properties);
        }

        private string TreatRoot(List<SelectField> properties)
        {
            if (properties == null || !properties.Any())
                return "";

            var token = new StringBuilder();

            token.Append("new(");
            TreatProperty(token, properties);
            token.Append(")");

            return token.ToString();
        }

        private void TreatProperty(StringBuilder token, List<SelectField> properties)
        {
            var lastValue = properties.Count - 1;
            for (int i = 0; i < properties.Count; i++)
                token.Append(TreatSelect(properties[i], (i == lastValue)));
        }

        private string TreatSelect(SelectField field, bool isLast)
        {
            var token = new StringBuilder();

            if (string.IsNullOrWhiteSpace(field.Ref))
                field.Ref = field.Name;

            if (field.Join != null)
                return token.Append(TreatJoin(field.Name, field.Join, isLast)).ToString();

            if (field.Unary != null)
                return token.Append(TreatUnary(field.Name, field.Unary, isLast)).ToString();

            if (field.List != null)
                return token.Append(TreatList(field.Name, field.Ref, field.List, isLast)).ToString();

            token.Append($"{field.Ref} as {field.Name}{LastChar(isLast)}");

            return token.ToString();
        }

        private string TreatList(string name, string refe, SelectListField listField, bool isLast)
        {
            if (string.IsNullOrWhiteSpace(refe))
                throw new ArgumentNullException("O campo Ref deve ser informado quando a List for utilizada.");

            if (listField == null)
                throw new ArgumentNullException("Os valores dentro do List deverão ser passados.");

            var hasSelect = (listField.Selects != null && listField.Selects.Any());

            if (string.IsNullOrWhiteSpace(listField.Prefix) && (!hasSelect))
                throw new ArgumentException("É necessário passar pelo menos um select ou uma pré-condição");

            var token = new StringBuilder();

            token.Append($"{refe}");

            if (!string.IsNullOrWhiteSpace(listField.Prefix))
                token.Append($".{listField.Prefix}");

            if (hasSelect)
            {
                token.Append(".Select(new(");

                TreatProperty(token, listField.Selects);

                var suffix = (!string.IsNullOrWhiteSpace(listField.Suffix)) ? $".{listField.Suffix}" : "";
                token.Append($")){suffix}");
            }

            token.Append($" as {name}");

            return token.ToString();
        }

        private string TreatUnary(string name, SelectUnaryField unary, bool isLast)
        {
            var token = new StringBuilder();

            token.Append($"iif({unary.Condition},");

            token.Append((unary.Correct == null || !unary.Correct.Any()) ? "null," : TreatRoot(unary.Correct) + ",");
            token.Append((unary.Incorrect == null || !unary.Incorrect.Any()) ? "null" : TreatRoot(unary.Incorrect));

            token.Append($") as {name}{LastChar(isLast)}");

            return token.ToString();
        }

        private string TreatJoin(string name, List<SelectField> joins, bool isLast)
        {
            var token = new StringBuilder();

            token.Append("new (");
            TreatProperty(token, joins);
            token.Append($") as {name}{LastChar(isLast)}");

            return token.ToString();
        }

        private string LastChar(bool isLast)
        {
            return (isLast) ? "" : ", ";
        }
    }
}