namespace CleanCode;

public sealed record QueryClause(WhereCondition WhereCondition, LogicalOperator LogicalOperator);