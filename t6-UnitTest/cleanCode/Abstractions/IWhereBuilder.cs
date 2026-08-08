namespace CleanCode;

internal interface IWhereBuilder
{
    string Build(List<QueryClause> clauses, List<object> bindings);
}