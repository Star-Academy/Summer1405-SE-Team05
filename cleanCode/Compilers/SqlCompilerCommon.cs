using System.Text;

namespace CleanCode;

internal sealed class SqlCompilerCommon : ISqlCommonCompiler
{
    private readonly IFromBuilder fromBuilder;
    private readonly ISelectBuilder selectBuilder;
    private readonly IWhereBuilder whereBuilder;

    public SqlCompilerCommon(IParameterIdentifier paramIdentifier, IExpressionOperator expressionOperator)
    {
        selectBuilder = new SqlSelectBuilder(paramIdentifier);
        fromBuilder = new SqlFromBuilder(paramIdentifier);
        whereBuilder = new SqlWhereBuilder(paramIdentifier, expressionOperator);
    }

    public DataBaseInput Compile(Query query)
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

        return new DataBaseInput(stringBuilder.ToString(), bindings);
    }
}