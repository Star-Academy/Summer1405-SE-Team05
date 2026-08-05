namespace CleanCode;

internal sealed class SqlServerCompiler : ICompiler
{
    private readonly ICommonCompiler _commonCompiler;
    private readonly IFromBuilder _fromBuilder;
    private readonly IParameterIdentifier _paramIdentifier;
    private readonly ISelectBuilder _selectBuilder;
    private readonly IWhereBuilder _whereBuilder;

    public SqlServerCompiler(
        IParameterIdentifier paramIdentifier,
        ISelectBuilder selectBuilder,
        IFromBuilder fromBuilder,
        IWhereBuilder whereBuilder,
        ICommonCompiler commonCompiler)
    {
        ArgumentNullException.ThrowIfNull(paramIdentifier);
        ArgumentNullException.ThrowIfNull(selectBuilder);
        ArgumentNullException.ThrowIfNull(fromBuilder);
        ArgumentNullException.ThrowIfNull(whereBuilder);
        ArgumentNullException.ThrowIfNull(commonCompiler);

        _paramIdentifier = paramIdentifier;
        _selectBuilder = selectBuilder;
        _fromBuilder = fromBuilder;
        _whereBuilder = whereBuilder;
        _commonCompiler = commonCompiler;
    }

    public SqlInput Compile(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);
        return _commonCompiler.Compile(query, _selectBuilder, _fromBuilder, _whereBuilder);
    }

    public string FormatParameterName(int index)
    {
        return _paramIdentifier.FormatParameterName(index);
    }
}