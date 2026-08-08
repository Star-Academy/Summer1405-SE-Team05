using System.Collections;
using CleanCode;

namespace cleanCode.test;

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
            new Query().From("users").Select("id", "email").Where("is_active", ExpressionOperatorType.Equals, true),
            "SELECT \"id\", \"email\" FROM \"users\" WHERE \"is_active\" = $1",
            new object[] { true }
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

        // 2. بدون SELECT (باید * بزند) + بدون WHERE
        yield return new object[]
        {
            new Query().From("course"),
            "SELECT * FROM \"course\"",
            Array.Empty<object>()
        };

        // 3. تک ستون SELECT + بدون WHERE
        yield return new object[]
        {
            new Query().From("department").Select("name"),
            "SELECT \"name\" FROM \"department\"",
            Array.Empty<object>()
        };

        // 4. شرط Equals ساده با عدد
        yield return new object[]
        {
            new Query().From("student").Select("id").Where("id", ExpressionOperatorType.Equals, 101),
            "SELECT \"id\" FROM \"student\" WHERE \"id\" = $1",
            new object[] { 101 }
        };

        // 5. شرط Equals بدون ذکر عملگر (Overload پیش‌فرض)
        yield return new object[]
        {
            new Query().From("student").Select("firstname").Where("studentnumber", "991001"),
            "SELECT \"firstname\" FROM \"student\" WHERE \"studentnumber\" = $1",
            new object[] { "991001" }
        };

        // 6. شرط GreaterThanOrEqual
        yield return new object[]
        {
            new Query().From("student").Select("studentnumber", "firstname")
                .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 16),
            "SELECT \"studentnumber\", \"firstname\" FROM \"student\" WHERE \"grade\" >= $1",
            new object[] { 16 }
        };

        // 7. شرط GreaterThan
        yield return new object[]
        {
            new Query().From("employee").Select("salary")
                .Where("salary", ExpressionOperatorType.GreaterThan, 5000),
            "SELECT \"salary\" FROM \"employee\" WHERE \"salary\" > $1",
            new object[] { 5000 }
        };

        // 8. شرط LessThan
        yield return new object[]
        {
            new Query().From("product").Select("title")
                .Where("price", ExpressionOperatorType.LessThan, 100),
            "SELECT \"title\" FROM \"product\" WHERE \"price\" < $1",
            new object[] { 100 }
        };

        // 9. شرط LessThanOrEqual
        yield return new object[]
        {
            new Query().From("student").Select("grade")
                .Where("grade", ExpressionOperatorType.LessThanOrEqual, 12),
            "SELECT \"grade\" FROM \"student\" WHERE \"grade\" <= $1",
            new object[] { 12 }
        };

        // 10. شرط NotEquals
        yield return new object[]
        {
            new Query().From("users").Select("username")
                .Where("status", ExpressionOperatorType.NotEquals, "banned"),
            "SELECT \"username\" FROM \"users\" WHERE \"status\" <> $1",
            new object[] { "banned" }
        };

        // 11. شرط LIKE
        yield return new object[]
        {
            new Query().From("student").Select("firstname")
                .Where("firstname", ExpressionOperatorType.Like, "Ali%"),
            "SELECT \"firstname\" FROM \"student\" WHERE \"firstname\" LIKE $1",
            new object[] { "Ali%" }
        };

        // 12. شرط بوولین (true)
        yield return new object[]
        {
            new Query().From("users").Select("id", "email")
                .Where("is_active", ExpressionOperatorType.Equals, true),
            "SELECT \"id\", \"email\" FROM \"users\" WHERE \"is_active\" = $1",
            new object[] { true }
        };

        // 13. شرط بوولین بدون SELECT (فراخوانی *)
        yield return new object[]
        {
            new Query().From("users")
                .Where("is_active", ExpressionOperatorType.Equals, false),
            "SELECT * FROM \"users\" WHERE \"is_active\" = $1",
            new object[] { false }
        };

        // 14. دو شرط AND متوالی
        yield return new object[]
        {
            new Query().From("student").Select("studentnumber")
                .Where("is_active", ExpressionOperatorType.Equals, true)
                .Where("age", ExpressionOperatorType.GreaterThan, 20),
            "SELECT \"studentnumber\" FROM \"student\" WHERE \"is_active\" = $1 AND \"age\" > $2",
            new object[] { true, 20 }
        };

        // 15. دو شرط با OrWhere
        yield return new object[]
        {
            new Query().From("orders").Select("total")
                .Where("status", ExpressionOperatorType.Equals, "Pending")
                .OrWhere("status", ExpressionOperatorType.Equals, "Processing"),
            "SELECT \"total\" FROM \"orders\" WHERE \"status\" = $1 OR \"status\" = $2",
            new object[] { "Pending", "Processing" }
        };

        // 16. ترکیب چند شرط بازه‌ای (مثلاً نمره بین ۱۲ تا ۱۸)
        yield return new object[]
        {
            new Query().From("student").Select("grade")
                .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 12)
                .Where("grade", ExpressionOperatorType.LessThanOrEqual, 18),
            "SELECT \"grade\" FROM \"student\" WHERE \"grade\" >= $1 AND \"grade\" <= $2",
            new object[] { 12, 18 }
        };

        // 17. ترکیبی از AND و OR با عملگرهای مختلف
        yield return new object[]
        {
            new Query().From("employee").Select("name", "salary")
                .Where("department_id", ExpressionOperatorType.Equals, 5)
                .OrWhere("salary", ExpressionOperatorType.GreaterThan, 10000),
            "SELECT \"name\", \"salary\" FROM \"employee\" WHERE \"department_id\" = $1 OR \"salary\" > $2",
            new object[] { 5, 10000 }
        };

        // 18. انتخاب چند ستون + OrWhere بدون عملگر صریح
        yield return new object[]
        {
            new Query().From("course").Select("id", "title", "unit")
                .Where("unit", 3)
                .OrWhere("unit", 4),
            "SELECT \"id\", \"title\", \"unit\" FROM \"course\" WHERE \"unit\" = $1 OR \"unit\" = $2",
            new object[] { 3, 4 }
        };

        // 19. سه شرط متوالی با عملگرهای ترکیبی
        yield return new object[]
        {
            new Query().From("student").Select("firstname", "lastname")
                .Where("grade", ExpressionOperatorType.GreaterThan, 15)
                .Where("is_active", true)
                .OrWhere("is_tuition_paid", true),
            "SELECT \"firstname\", \"lastname\" FROM \"student\" WHERE \"grade\" > $1 AND \"is_active\" = $2 OR \"is_tuition_paid\" = $3",
            new object[] { 15, true, true }
        };

        // 20. کوئری با رشته متنی شامل کاراکترهای خاص
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