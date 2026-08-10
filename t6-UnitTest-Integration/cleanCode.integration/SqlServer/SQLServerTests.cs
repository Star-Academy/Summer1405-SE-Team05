using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using CleanCode;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Xunit;

namespace cleanCode.integration.sqlserver;

public class SQLServerTests : IClassFixture<MsSqlDatabaseFixture>, IAsyncLifetime
{
    private readonly MsSqlDatabaseFixture _fixture;
    private readonly SqlServerCompiler _sqlServerCompiler;
    private SqlConnection _connection = null!;
    private DatabaseExecutor _sut = null!;

    public SQLServerTests(MsSqlDatabaseFixture fixture)
    {
        _fixture = fixture;

        var expressionOperator = new SqlExpressionOperator();
        var parameterIdentifier = new SqlServerParameterIdentifier();
        var whereBuilder = new SqlWhereBuilder(parameterIdentifier, expressionOperator);
        var fromBuilder = new SqlFromBuilder(parameterIdentifier);
        var selectBuilder = new SqlSelectBuilder(parameterIdentifier);
        var commonCompiler = new SqlCompilerCommon(fromBuilder, selectBuilder, whereBuilder);

        _sqlServerCompiler = new SqlServerCompiler(parameterIdentifier, commonCompiler);
    }

    public async Task InitializeAsync()
    {
        _connection = await _fixture.CreateConnectionAsync();
        _sut = new DatabaseExecutor(_connection, _sqlServerCompiler);
    }

    public async Task DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
    }

    [Fact]
    public void Execute_ShouldThrowArgumentException_WhenTableNameIsNull()
    {
        // Arrange
        var query = new Query()
            .Select("studentnumber")
            .Where("grade", ExpressionOperatorType.Equals, 19.00m);

        // Act
        Action executeAction = () => _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        executeAction.Should().Throw<ArgumentException>()
            .WithMessage("Table name cannot be null or empty.");
    }

    [Theory]
    [InlineData(17.00, new[] { "97100166" })]
    [InlineData(19.00, new[] { "97100112", "98100201" })]
    [InlineData(20.00, new string[0])]
    public void Execute_ShouldReturnExpectedStudentNumbers_WhenEqualityConditionWithoutOperatorIsApplied(
        decimal thresholdGrade,
        string[] expectedStudentNumbers)
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .Where("grade", thresholdGrade);

        // Act
        var actualStudentNumbers = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        actualStudentNumbers.Should().BeEquivalentTo(expectedStudentNumbers);
    }

    [Theory]
    [InlineData(ExpressionOperatorType.LessThan, 15.00, new[] { "99100305" })]
    [InlineData(ExpressionOperatorType.GreaterThan, 19.00, new string[0])]
    [InlineData(ExpressionOperatorType.LessThanOrEqual, 16.00, new[] { "99100305", "97100999" })]
    [InlineData(ExpressionOperatorType.GreaterThanOrEqual, 17.00, new[] { "98100201", "97100112", "97100166" })]
    [InlineData(ExpressionOperatorType.NotEquals, 19.00, new[] { "97100166", "99100305", "97100999" })]
    public void Execute_ShouldReturnExpectedStudentNumbers_WhenComparisonOperatorsAreApplied(
        ExpressionOperatorType expressionOperator,
        decimal thresholdGrade,
        string[] expectedStudentNumbers)
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .Where("grade", expressionOperator, thresholdGrade);

        // Act
        var actualStudentNumbers = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        actualStudentNumbers.Should().BeEquivalentTo(expectedStudentNumbers);
    }

    [Theory]
    [MemberData(nameof(MultipleTestData))]
    public void Execute_ShouldReturnExpectedStudentNumbers_WhenMultipleWhereOrConditionsAreCombined(
        Query query,
        string[] expectedStudentNumbers)
    {
        // Arrange & Act
        var actualStudentNumbers = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        actualStudentNumbers.Should().BeEquivalentTo(expectedStudentNumbers);
    }

    public static IEnumerable<object[]> MultipleTestData()
    {
        yield return new object[]
        {
            new Query()
                .From("student")
                .Select("studentnumber")
                .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 18.00m)
                .Where("grade", ExpressionOperatorType.LessThanOrEqual, 19.50m),
            new[] { "98100201", "97100112" }
        };

        yield return new object[]
        {
            new Query()
                .From("student")
                .Select("studentnumber")
                .OrWhere("grade", ExpressionOperatorType.GreaterThanOrEqual, 19.00m)
                .OrWhere("studentnumber", "99100305"),
            new[] { "98100201", "97100112", "99100305" }
        };

        yield return new object[]
        {
            new Query()
                .From("student")
                .Select("studentnumber")
                .OrWhere("studentnumber", ExpressionOperatorType.Equals, "98100201")
                .OrWhere("studentnumber", ExpressionOperatorType.Equals, "97100999"),
            new[] { "98100201", "97100999" }
        };
    }

    public record StudentDataTransferObject(
        string StudentNumber,
        string FirstName,
        string LastName,
        decimal Grade);

    public static IEnumerable<object[]> SelectProjectionsTestData()
    {
        yield return new object[]
        {
            new Query().From("student"),
            new Func<DbDataReader, object>(reader => new StudentDataTransferObject(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetDecimal(3))),
            5
        };

        yield return new object[]
        {
            new Query().From("student").Select("studentnumber"),
            new Func<DbDataReader, object>(reader => reader.GetString(0)),
            5
        };

        yield return new object[]
        {
            new Query().From("student").Select("studentnumber", "grade"),
            new Func<DbDataReader, object>(reader => new
            {
                StudentNumber = reader.GetString(0),
                Grade = reader.GetDecimal(1)
            }),
            5
        };
    }

    [Theory]
    [MemberData(nameof(SelectProjectionsTestData))]
    public void Execute_ShouldReturnExpectedProjections_WhenSelectingColumnsDynamically(
        Query query,
        Func<DbDataReader, object> resultMapper,
        int expectedResultCount)
    {
        // Arrange & Act
        var actualResults = _sut.Execute(query, resultMapper);

        // Assert
        actualResults.Should().NotBeNull();
        actualResults.Should().HaveCount(expectedResultCount);
        actualResults.All(item => item != null).Should().BeTrue();
    }

    [Fact]
    public void Execute_ShouldMapAllFieldsCorrectly_WhenSelectingAllColumns()
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Where("studentnumber", ExpressionOperatorType.Equals, "98100201");

        // Act
        var results = _sut.Execute(query, reader => new StudentDataTransferObject(
            reader.GetString(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetDecimal(3)));

        // Assert
        var student = results.Should().ContainSingle().Subject;
        student.FirstName.Should().Be("سارا");
        student.LastName.Should().Be("رضایی");
        student.Grade.Should().Be(19.00m);
    }
}