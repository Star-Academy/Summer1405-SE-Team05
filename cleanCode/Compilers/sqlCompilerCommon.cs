using System.Text;

namespace CleanCode;

public class SqlCompilerCommon 
{
    public SqlInput Compile(
        Query query, 
        ISelectBuilder selectBuilder, 
        IFromBuilder fromBuilder, 
        IWhereBuilder whereBuilder, 
        IParameterIdentifier paramIdentifier)
    {
        var bindings = new List<object>();
        var sb = new StringBuilder();

        sb.Append(selectBuilder.Build(query.Columns, paramIdentifier));
        sb.Append(" ");
        sb.Append(fromBuilder.Build(query.Table, paramIdentifier));

        if (query.Clauses.Count > 0)
        {
            sb.Append(" ");
            sb.Append(whereBuilder.Build(query.Clauses, paramIdentifier, bindings));
        }

        return new SqlInput(sb.ToString(), bindings);
    }
}