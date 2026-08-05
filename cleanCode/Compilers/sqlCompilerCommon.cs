using System.Text;

namespace CleanCode;

internal sealed class SqlCompilerCommon : ICommonCompiler
{
    public SqlInput Compile(
        Query query,
        ISelectBuilder selectBuilder,
        IFromBuilder fromBuilder,
        IWhereBuilder whereBuilder)
    {
        var bindings = new List<object>();
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(selectBuilder.Build(query.Columns));
        stringBuilder.Append(" ");
        stringBuilder.Append(fromBuilder.Build(query.Table));

        if (query.QueryClauses.Count > 0)
        {
            stringBuilder.Append(" ");
            stringBuilder.Append(whereBuilder.Build(query.QueryClauses, bindings));
        }

        return new SqlInput(stringBuilder.ToString(), bindings);
    }
}