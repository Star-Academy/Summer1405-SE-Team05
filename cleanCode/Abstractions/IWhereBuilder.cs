namespace CleanCode;

internal interface IWhereBuilder
{
    string Build(List<QueryClause> clauses, IParameterIdentifier paramIdentifier, List<object> bindings);
}
