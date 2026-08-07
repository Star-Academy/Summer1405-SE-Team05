namespace CleanCode;

internal sealed class SqlServerCompiler : ICompiler
{
    private readonly ICommonCompiler _commonCompiler;
    private readonly IParameterIdentifier _paramIdentifier;
    

    public SqlServerCompiler(
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