namespace CleanCode;

internal sealed record QueryClause(WhereCondition WhereCondition, LogicalOperator LogicalOperator);