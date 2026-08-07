namespace CleanCode;

internal sealed class PostgresCompiler : ICompiler
{
    private readonly ICommonCompiler _commonCompiler;
    private readonly IParameterIdentifier _paramIdentifier;
    

    public PostgresCompiler(
        IParameterIdentifier paramIdentifier,
        ICommonCompiler commonCompiler)
    {
        _paramIdentifier = paramIdentifier;
        
        _commonCompiler = commonCompiler;
    }

    public SqlInput Compile(Query query)
    {
        return _commonCompiler.Compile(query);
    }

    public string FormatParameterName(int index)
    {
        return _paramIdentifier.FormatParameterName(index);
    }
}