namespace CleanCode;

internal sealed class PostgresCompiler : ICompiler
{
    private readonly ICommonCompiler _commonCompiler;
    private readonly IFromBuilder _fromBuilder;
    private readonly IParameterIdentifier _paramIdentifier;
    private readonly ISelectBuilder _selectBuilder;
    private readonly IWhereBuilder _whereBuilder;

    public PostgresCompiler(
        IParameterIdentifier paramIdentifier,
        ISelectBuilder selectBuilder,
        IFromBuilder fromBuilder,
        IWhereBuilder whereBuilder,
        ICommonCompiler commonCompiler)
    {
        _paramIdentifier = paramIdentifier;
        _selectBuilder = selectBuilder;
        _fromBuilder = fromBuilder;
        _whereBuilder = whereBuilder;
        _commonCompiler = commonCompiler;
    }

    public SqlInput Compile(Query query)
    {
        return _commonCompiler.Compile(query, _selectBuilder, _fromBuilder, _whereBuilder);
    }

    public string FormatParameterName(int index)
    {
        return _paramIdentifier.FormatParameterName(index);
    }
}