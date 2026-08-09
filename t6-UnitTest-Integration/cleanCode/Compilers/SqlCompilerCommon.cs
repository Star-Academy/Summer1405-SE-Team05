using System.Text;

namespace CleanCode;

internal sealed class SqlCompilerCommon : ISqlCommonCompiler
{
    private readonly IFromBuilder fromBuilder;
    private readonly ISelectBuilder selectBuilder;
    private readonly IWhereBuilder whereBuilder;

    public SqlCompilerCommon(IFromBuilder fromBuilder, ISelectBuilder selectBuilder, IWhereBuilder whereBuilder)
    {
        ArgumentNullException.ThrowIfNull(fromBuilder);
        ArgumentNullException.ThrowIfNull(selectBuilder);
        ArgumentNullException.ThrowIfNull(whereBuilder);

        this.fromBuilder = fromBuilder;
        this.selectBuilder = selectBuilder;
        this.whereBuilder = whereBuilder;
    }

    public DataBaseInput Compile(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);

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

        return new DataBaseInput(stringBuilder.ToString(), bindings);
    }
}