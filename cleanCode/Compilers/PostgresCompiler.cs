namespace CleanCode;

internal sealed class PostgresCompiler : ICompiler
{
    private readonly ISqlCommonCompiler _commonCompiler;
    private readonly IParameterIdentifier _paramIdentifier;

    public PostgresCompiler(
        IParameterIdentifier paramIdentifier,
        ISqlCommonCompiler commonCompiler)
    {
        ArgumentNullException.ThrowIfNull(paramIdentifier);
        ArgumentNullException.ThrowIfNull(commonCompiler);

        _paramIdentifier = paramIdentifier;
        _commonCompiler = commonCompiler;
    }

    public DataBaseInput Compile(Query query)
    {
        ArgumentNullException.ThrowIfNull(query);
        return _commonCompiler.Compile(query);
    }

    public string FormatParameterName(int index)
    {
        return _paramIdentifier.FormatParameterName(index);
    }
}