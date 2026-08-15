namespace CleanCode;

internal interface ISqlCommonCompiler
{
    public DataBaseInput Compile(Query query);
}