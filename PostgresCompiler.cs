using System.Text;

namespace ClassLibrary1;

public class PostgresCompiler
{
    public SqlResult Compile(Query query)
    {
        var result = new SqlResult();
        var sb = new StringBuilder();

        string cols = query.Columns.Count > 0 
            ? string.Join(", ", query.Columns.Select(c => $"\"{c}\"")) 
            : "*";

        sb.Append($"SELECT {cols} FROM \"{query.Table}\"");

        if (query.WhereConditions.Count > 0)
        {
            sb.Append(" WHERE ");

            var conditions = new List<string>();
            int paramIndex = 1;

            foreach (var condition in query.WhereConditions)
            {
                conditions.Add($"\"{condition.Key}\" = ${paramIndex}");
                result.Bindings.Add(condition.Value);
                paramIndex++;
            }

            sb.Append(string.Join(" AND ", conditions));
        }

        result.Sql = sb.ToString();
        return result;
    }
}