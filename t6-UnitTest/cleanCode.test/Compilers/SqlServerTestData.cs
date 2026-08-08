using System.Collections;
using CleanCode;

namespace cleanCode.test.Compilers;
public class SqlServerTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new Query().From("Student").Select("StudentNumber", "FirstName", "LastName")
                .Where("IsMale", ExpressionOperatorType.Equals, 1),
            "SELECT [StudentNumber], [FirstName], [LastName] FROM [Student] WHERE [IsMale] = @p0",
            new object[] { 1 }
        };

        yield return new object[]
        {
            new Query().From("Student")
                .Where("Grade", ExpressionOperatorType.GreaterThanOrEqual, 16.0),
            "SELECT * FROM [Student] WHERE [Grade] >= @p0",
            new object[] { 16.0 }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("StudentNumber", "Grade"),
            "SELECT [StudentNumber], [Grade] FROM [Student]",
            Array.Empty<object>()
        };

        yield return new object[]
        {
            new Query().From("Student"),
            "SELECT * FROM [Student]",
            Array.Empty<object>()
        };

        yield return new object[]
        {
            new Query().From("Student").Select("FirstName"),
            "SELECT [FirstName] FROM [Student]",
            Array.Empty<object>()
        };

        yield return new object[]
        {
            new Query().From("Student").Select("FirstName", "LastName")
                .Where("StudentNumber", "00100450"),
            "SELECT [FirstName], [LastName] FROM [Student] WHERE [StudentNumber] = @p0",
            new object[] { "00100450" }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("StudentNumber", "LeftUnitsCount")
                .Where("LeftUnitsCount", ExpressionOperatorType.GreaterThan, 100),
            "SELECT [StudentNumber], [LeftUnitsCount] FROM [Student] WHERE [LeftUnitsCount] > @p0",
            new object[] { 100 }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("StudentNumber", "Grade")
                .Where("Grade", ExpressionOperatorType.LessThan, 12.0),
            "SELECT [StudentNumber], [Grade] FROM [Student] WHERE [Grade] < @p0",
            new object[] { 12.0 }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("FirstName", "LastName")
                .Where("LeftUnitsCount", ExpressionOperatorType.LessThanOrEqual, 0),
            "SELECT [FirstName], [LastName] FROM [Student] WHERE [LeftUnitsCount] <= @p0",
            new object[] { 0 }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("StudentNumber")
                .Where("IsMale", ExpressionOperatorType.NotEquals, 0),
            "SELECT [StudentNumber] FROM [Student] WHERE [IsMale] <> @p0",
            new object[] { 0 }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("FirstName", "LastName")
                .Where("FirstName", ExpressionOperatorType.Like, "علی%"),
            "SELECT [FirstName], [LastName] FROM [Student] WHERE [FirstName] LIKE @p0",
            new object[] { "علی%" }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("StudentNumber", "Grade")
                .Where("LastName", ExpressionOperatorType.Equals, "رضایی"),
            "SELECT [StudentNumber], [Grade] FROM [Student] WHERE [LastName] = @p0",
            new object[] { "رضایی" }
        };

        yield return new object[]
        {
            new Query().From("Student")
                .Where("DateOfBirth", ExpressionOperatorType.Equals, "2002-01-09T20:30:00.000Z"),
            "SELECT * FROM [Student] WHERE [DateOfBirth] = @p0",
            new object[] { "2002-01-09T20:30:00.000Z" }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("StudentNumber")
                .Where("IsMale", ExpressionOperatorType.Equals, 1)
                .Where("Grade", ExpressionOperatorType.GreaterThan, 15.0),
            "SELECT [StudentNumber] FROM [Student] WHERE [IsMale] = @p0 AND [Grade] > @p1",
            new object[] { 1, 15.0 }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("FirstName", "Grade")
                .Where("LastName", ExpressionOperatorType.Equals, "موسوی")
                .OrWhere("LastName", ExpressionOperatorType.Equals, "احمدی"),
            "SELECT [FirstName], [Grade] FROM [Student] WHERE [LastName] = @p0 OR [LastName] = @p1",
            new object[] { "موسوی", "احمدی" }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("StudentNumber", "Grade")
                .Where("Grade", ExpressionOperatorType.GreaterThanOrEqual, 12.0)
                .Where("Grade", ExpressionOperatorType.LessThanOrEqual, 18.0),
            "SELECT [StudentNumber], [Grade] FROM [Student] WHERE [Grade] >= @p0 AND [Grade] <= @p1",
            new object[] { 12.0, 18.0 }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("FirstName", "LastName")
                .Where("IsMale", ExpressionOperatorType.Equals, 0)
                .OrWhere("LeftUnitsCount", ExpressionOperatorType.GreaterThan, 100),
            "SELECT [FirstName], [LastName] FROM [Student] WHERE [IsMale] = @p0 OR [LeftUnitsCount] > @p1",
            new object[] { 0, 100 }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("StudentNumber", "FirstName")
                .Where("LeftUnitsCount", 0)
                .OrWhere("LeftUnitsCount", 12),
            "SELECT [StudentNumber], [FirstName] FROM [Student] WHERE [LeftUnitsCount] = @p0 OR [LeftUnitsCount] = @p1",
            new object[] { 0, 12 }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("FirstName", "LastName")
                .Where("Grade", ExpressionOperatorType.GreaterThan, 15.0)
                .Where("IsMale", 1)
                .OrWhere("LeftUnitsCount", ExpressionOperatorType.LessThan, 30),
            "SELECT [FirstName], [LastName] FROM [Student] WHERE [Grade] > @p0 AND [IsMale] = @p1 OR [LeftUnitsCount] < @p2",
            new object[] { 15.0, 1, 30 }
        };

        yield return new object[]
        {
            new Query().From("Student").Select("StudentNumber", "Grade")
                .Where("StudentNumber", ExpressionOperatorType.Like, "98%"),
            "SELECT [StudentNumber], [Grade] FROM [Student] WHERE [StudentNumber] LIKE @p0",
            new object[] { "98%" }
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}