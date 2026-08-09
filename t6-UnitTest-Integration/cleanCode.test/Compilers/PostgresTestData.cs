using System.Collections;
using CleanCode;

namespace cleanCode.test.Compilers;
public class PostgresTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new Query().From("student").Select("studentnumber", "firstname")
                .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 16),
            "SELECT \"studentnumber\", \"firstname\" FROM \"student\" WHERE \"grade\" >= $1",
            new object[] { 16 }
        };

        yield return new object[]
        {
            new Query().From("users").Select("id", "email").Where("is_active", ExpressionOperatorType.Equals, false),
            "SELECT \"id\", \"email\" FROM \"users\" WHERE \"is_active\" = $1",
            new object[] { false }
        };

        yield return new object[]
        {
            new Query().From("users").Where("is_active", ExpressionOperatorType.Equals, true),
            "SELECT * FROM \"users\" WHERE \"is_active\" = $1",
            new object[] { true }
        };
        
        yield return new object[]
        {
            new Query().From("student").Select("studentnumber", "firstname"),
            "SELECT \"studentnumber\", \"firstname\" FROM \"student\"",
            Array.Empty<object>()
        };

        yield return new object[]
        {
            new Query().From("course"),
            "SELECT * FROM \"course\"",
            Array.Empty<object>()
        };

        yield return new object[]
        {
            new Query().From("department").Select("name"),
            "SELECT \"name\" FROM \"department\"",
            Array.Empty<object>()
        };

        yield return new object[]
        {
            new Query().From("student").Select("id").Where("id", ExpressionOperatorType.Equals, 101),
            "SELECT \"id\" FROM \"student\" WHERE \"id\" = $1",
            new object[] { 101 }
        };

        yield return new object[]
        {
            new Query().From("student").Select("firstname").Where("studentnumber", "991001"),
            "SELECT \"firstname\" FROM \"student\" WHERE \"studentnumber\" = $1",
            new object[] { "991001" }
        };

        yield return new object[]
        {
            new Query().From("student").Select("studentnumber", "firstname")
                .Where("grade", ExpressionOperatorType.LessThan, 19),
            "SELECT \"studentnumber\", \"firstname\" FROM \"student\" WHERE \"grade\" < $1",
            new object[] { 19 }
        };

        yield return new object[]
        {
            new Query().From("employee").Select("salary")
                .Where("salary", ExpressionOperatorType.GreaterThan, 5000),
            "SELECT \"salary\" FROM \"employee\" WHERE \"salary\" > $1",
            new object[] { 5000 }
        };

        yield return new object[]
        {
            new Query().From("product").Select("title")
                .Where("price", ExpressionOperatorType.LessThan, 100),
            "SELECT \"title\" FROM \"product\" WHERE \"price\" < $1",
            new object[] { 100 }
        };

        yield return new object[]
        {
            new Query().From("student").Select("grade")
                .Where("grade", ExpressionOperatorType.LessThanOrEqual, 12),
            "SELECT \"grade\" FROM \"student\" WHERE \"grade\" <= $1",
            new object[] { 12 }
        };

        yield return new object[]
        {
            new Query().From("users").Select("username")
                .Where("status", ExpressionOperatorType.NotEquals, "banned"),
            "SELECT \"username\" FROM \"users\" WHERE \"status\" <> $1",
            new object[] { "banned" }
        };

        yield return new object[]
        {
            new Query().From("student").Select("firstname")
                .Where("firstname", ExpressionOperatorType.Like, "Ali%"),
            "SELECT \"firstname\" FROM \"student\" WHERE \"firstname\" LIKE $1",
            new object[] { "Ali%" }
        };

        yield return new object[]
        {
            new Query().From("users").Select("id", "email")
                .Where("is_active", ExpressionOperatorType.Equals, true),
            "SELECT \"id\", \"email\" FROM \"users\" WHERE \"is_active\" = $1",
            new object[] { true }
        };

        yield return new object[]
        {
            new Query().From("users")
                .Where("is_active", ExpressionOperatorType.Equals, false),
            "SELECT * FROM \"users\" WHERE \"is_active\" = $1",
            new object[] { false }
        };

        yield return new object[]
        {
            new Query().From("student").Select("studentnumber")
                .Where("is_active", ExpressionOperatorType.Equals, true)
                .Where("age", ExpressionOperatorType.GreaterThan, 20),
            "SELECT \"studentnumber\" FROM \"student\" WHERE \"is_active\" = $1 AND \"age\" > $2",
            new object[] { true, 20 }
        };

        yield return new object[]
        {
            new Query().From("orders").Select("total")
                .Where("status", ExpressionOperatorType.Equals, "Pending")
                .OrWhere("status", ExpressionOperatorType.Equals, "Processing"),
            "SELECT \"total\" FROM \"orders\" WHERE \"status\" = $1 OR \"status\" = $2",
            new object[] { "Pending", "Processing" }
        };

        yield return new object[]
        {
            new Query().From("student").Select("grade")
                .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 12)
                .Where("grade", ExpressionOperatorType.LessThanOrEqual, 18),
            "SELECT \"grade\" FROM \"student\" WHERE \"grade\" >= $1 AND \"grade\" <= $2",
            new object[] { 12, 18 }
        };

        yield return new object[]
        {
            new Query().From("employee").Select("name", "salary")
                .Where("department_id", ExpressionOperatorType.Equals, 5)
                .OrWhere("salary", ExpressionOperatorType.GreaterThan, 10000),
            "SELECT \"name\", \"salary\" FROM \"employee\" WHERE \"department_id\" = $1 OR \"salary\" > $2",
            new object[] { 5, 10000 }
        };

        yield return new object[]
        {
            new Query().From("course").Select("id", "title", "unit")
                .Where("unit", 3)
                .OrWhere("unit", 4),
            "SELECT \"id\", \"title\", \"unit\" FROM \"course\" WHERE \"unit\" = $1 OR \"unit\" = $2",
            new object[] { 3, 4 }
        };

        yield return new object[]
        {
            new Query().From("student").Select("firstname", "lastname")
                .Where("grade", ExpressionOperatorType.GreaterThan, 15)
                .Where("is_active", true)
                .OrWhere("is_tuition_paid", true),
            "SELECT \"firstname\", \"lastname\" FROM \"student\" WHERE \"grade\" > $1 AND \"is_active\" = $2 OR \"is_tuition_paid\" = $3",
            new object[] { 15, true, true }
        };

        yield return new object[]
        {
            new Query().From("logs").Select("message")
                .Where("level", ExpressionOperatorType.Equals, "ERROR_500"),
            "SELECT \"message\" FROM \"logs\" WHERE \"level\" = $1",
            new object[] { "ERROR_500" }
        };
        
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}