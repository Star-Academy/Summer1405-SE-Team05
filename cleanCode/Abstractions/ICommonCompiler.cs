namespace CleanCode;

internal interface ICommonCompiler
{
    public SqlInput Compile(
        Query query,
        ISelectBuilder selectBuilder,
        IFromBuilder fromBuilder,
        IWhereBuilder whereBuilder);
}