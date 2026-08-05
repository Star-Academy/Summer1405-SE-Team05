namespace CleanCode;

public interface IWhereBuilder
{
    string Build(List<QueryClause> clauses, IParameterIdentifier paramIdentifier, List<object> bindings);
}
