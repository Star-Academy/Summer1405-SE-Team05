namespace CleanCode;

internal sealed record QueryClause(WhereCondition Condition, LogicalOperator LogicalOp);