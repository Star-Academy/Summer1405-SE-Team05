namespace CleanCode;

internal sealed class SqlServerCompiler : ICompiler
{
    private readonly IParameterIdentifier _paramIdentifier;
    private readonly ISelectBuilder _selectBuilder;
    private readonly IFromBuilder _fromBuilder;
    private readonly IWhereBuilder _whereBuilder;
    private readonly SqlCompilerCommon _commonCompiler;

    public SqlServerCompiler(
        IParameterIdentifier paramIdentifier,
        ISelectBuilder selectBuilder,
        IFromBuilder fromBuilder,
        IWhereBuilder whereBuilder,
        SqlCompilerCommon commonCompiler)
    {
        _paramIdentifier = paramIdentifier;
        _selectBuilder = selectBuilder;
        _fromBuilder = fromBuilder;
        _whereBuilder = whereBuilder;
        _commonCompiler = commonCompiler;
    }

    public SqlInput Compile(Query query)
    {
        return _commonCompiler.Compile(query, _selectBuilder, _fromBuilder, _whereBuilder, _paramIdentifier);
    }

    public string FormatParameterName(int index)
    {
        return _paramIdentifier.FormatParameterName(index);
    }
}