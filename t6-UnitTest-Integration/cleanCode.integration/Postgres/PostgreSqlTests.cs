using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanCode;
using FluentAssertions;
using Npgsql;
using Xunit;

namespace cleanCode.integration.postgres;

public class PostgreSqlTests : IClassFixture<PostgresContainerFixture>, IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;
    private readonly PostgresCompiler _postgresCompiler;
    private NpgsqlConnection _connection = null!;
    private DatabaseExecutor _sut = null!;

    public PostgreSqlTests(PostgresContainerFixture fixture)
    {
        _fixture = fixture;

        var expressionOperator = new SqlExpressionOperator();
        var postgresParameterIdentifier = new PostgresParameterIdentifier();
        var fromBuilder = new SqlFromBuilder(postgresParameterIdentifier);
        var whereBuilder = new SqlWhereBuilder(postgresParameterIdentifier, expressionOperator);
        var selectBuilder = new SqlSelectBuilder(postgresParameterIdentifier);
        var commonCompiler = new SqlCompilerCommon(fromBuilder, selectBuilder, whereBuilder);
        _postgresCompiler = new PostgresCompiler(postgresParameterIdentifier, commonCompiler);
    }

    public async Task InitializeAsync()
    {
        _connection = await _fixture.CreateConnectionAsync();
        _sut = new DatabaseExecutor(_connection, _postgresCompiler);
    }

    public async Task DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
    }

    #region Exception & Validation Tests

    [Fact]
    public void Execute_ShouldThrowArgumentException_WhenTableNameIsNull()
    {
        // Arrange
        var query = new Query()
            .Select("studentnumber")
            .Where("age", ExpressionOperatorType.Equals, 20);

        // Act
        Action executeAction = () => _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        executeAction.Should().Throw<ArgumentException>()
            .WithMessage("Table name cannot be null or empty.");
    }

    #endregion

    #region Select Clause Tests

    [Fact]
    public void Execute_ShouldReturnAllColumns_WhenNoColumnsAreExplicitlySelected()
    {
        // Arrange 
        var query = new Query()
            .From("student")
            .Where("studentnumber", ExpressionOperatorType.Equals, "01100523");

        // Act
        var result = _sut.Execute(query, reader => new
        {
            StudentNumber = reader.GetString(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            Grade = reader.GetFloat(3),
            Age = reader.GetInt32(4)
        });

        // Assert
        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new
            {
                StudentNumber = "01100523",
                FirstName = "مریم",
                LastName = "حسینی",
                Grade = 20.00f,
                Age = 18
            });
    }

    [Fact]
    public void Execute_ShouldReturnMultipleMappedColumns_WhenMultipleColumnsAreSelected()
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("firstname", "lastname")
            .Where("age", ExpressionOperatorType.LessThan, 17);

        // Act
        var result = _sut.Execute(query, reader => (FirstName: reader.GetString(0), LastName: reader.GetString(1)));

        // Assert
        result.Should().BeEquivalentTo(new[]
        {
            ("پارسای", "شریفی"),
            ("آرمینا", "سهرابی")
        });
    }

    #endregion

    #region Where Single Condition Tests

    [Theory]
    [InlineData(ExpressionOperatorType.GreaterThan, 17f, new[] { "98100201", "97100112", "01100523", "04100866" })]
    [InlineData(ExpressionOperatorType.LessThan, 5f, new[] { "00100412", "96100888", "98100456" })]
    [InlineData(ExpressionOperatorType.Equals, 20f, new[] { "01100523" })]
    [InlineData(ExpressionOperatorType.GreaterThanOrEqual, 15.5f,
        new[] { "98100201", "97100112", "97100999", "01100523", "04100866", "03100111" })]
    public void Execute_ShouldFilterBySingleGradeComparisonOperator(
        ExpressionOperatorType expressionOperator,
        float targetGrade,
        string[] expectedStudentNumbers)
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .Where("grade", expressionOperator, targetGrade);

        // Act
        var result = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        result.Should().BeEquivalentTo(expectedStudentNumbers);
    }

    [Theory]
    [InlineData(ExpressionOperatorType.GreaterThan, 22, new[] { "97100112", "97100999", "96100888", "98100456" })]
    [InlineData(ExpressionOperatorType.LessThan, 18,
        new[] { "02100644", "03100755", "04100866", "02100321", "03100111" })]
    [InlineData(ExpressionOperatorType.GreaterThanOrEqual, 24, new[] { "97100112", "97100999", "96100888" })]
    [InlineData(ExpressionOperatorType.LessThanOrEqual, 16, new[] { "03100755", "04100866", "03100111" })]
    public void Execute_ShouldFilterBySingleAgeComparisonOperator(
        ExpressionOperatorType expressionOperator,
        int targetAge,
        string[] expectedStudentNumbers)
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .Where("age", expressionOperator, targetAge);

        // Act
        var result = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        result.Should().BeEquivalentTo(expectedStudentNumbers);
    }

    [Fact]
    public void Execute_ShouldFilterByExactValue_WhenDefaultEqualityWhereOverloadIsUsed()
    {
        // Arrange 
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .Where("firstname", "سارا");

        // Act
        var result = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        result.Should().ContainSingle().Which.Should().Be("98100201");
    }

    [Fact]
    public void Execute_ShouldExcludeMatchingRecords_WhenNotEqualsOperatorIsUsed()
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .Where("age", ExpressionOperatorType.NotEquals, 25);

        // Act
        var result = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        result.Should().NotContain(new[] { "97100999", "96100888" });
    }

    #endregion

    #region Logical Operator (AND / OR) Combination Tests

    [Fact]
    public void Execute_ShouldApplyConjunction_WhenMultipleWhereClausesAreChainedWithAnd()
    {
        // Arrange 
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .Where("age", ExpressionOperatorType.GreaterThanOrEqual, 18)
            .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 15.0f);

        // Act
        var result = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        result.Should().BeEquivalentTo(new[] { "98100201", "97100112", "97100999", "01100523" });
    }

    [Theory]
    [InlineData(20, 20f, new[] { "99100305", "01100523" })]
    [InlineData(22, 18.75f, new[] { "98100201" })]
    public void Execute_ShouldApplyDisjunction_WhenOrWhereClausesAreUsed(
        int targetAge,
        float targetGrade,
        string[] expectedStudentNumbers)
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .OrWhere("age", targetAge)
            .OrWhere("grade", targetGrade);

        // Act
        var result = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        result.Should().BeEquivalentTo(expectedStudentNumbers);
    }

    [Fact]
    public void Execute_ShouldCombineAndWithOrClauses_WhenBothWhereAndOrWhereAreChained()
    {
        // Arrange 
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .Where("age", ExpressionOperatorType.Equals, 15)
            .OrWhere("grade", ExpressionOperatorType.GreaterThan, 19f)
            .Where("age", ExpressionOperatorType.Equals, 18);

        // Act
        var result = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        result.Should().BeEquivalentTo(new[] { "04100866", "01100523" });
    }

    #endregion
}