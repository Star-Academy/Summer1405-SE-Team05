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

    public record StudentDataTransferObject(
        string StudentNumber,
        string FirstName,
        string LastName,
        decimal Grade);

    #region Exception & Validation Tests

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

    #endregion

    #region Select & Projection Tests

    [Fact]
    public void Execute_ShouldSelectAllColumns_WhenNoSelectClauseIsProvided()
    {
        // Arrange
        var query = new Query().From("student");

        // Act
        var results = _sut.Execute(query, reader => new StudentDataTransferObject(
            reader.GetString(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetDecimal(3)));

        // Assert
        results.Should().HaveCount(5);
        results.All(s => s != null).Should().BeTrue();
    }

    [Fact]
    public void Execute_ShouldSelectSingleColumn_WhenOnlyOneColumnIsSpecifiedInSelect()
    {
        // Arrange
        var query = new Query().From("student").Select("studentnumber");

        // Act
        var results = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        results.Should().HaveCount(5);
        results.All(id => !string.IsNullOrEmpty(id)).Should().BeTrue();
    }

    [Fact]
    public void Execute_ShouldSelectMultipleColumns_WhenMultipleColumnsAreSpecifiedInSelect()
    {
        // Arrange
        var query = new Query().From("student").Select("studentnumber", "grade");

        // Act
        var results = _sut.Execute(query, reader => new
        {
            StudentNumber = reader.GetString(0),
            Grade = reader.GetDecimal(1)
        });

        // Assert
        results.Should().HaveCount(5);
        results.All(item => item.StudentNumber != null && item.Grade >= 0).Should().BeTrue();
    }

    [Fact]
    public void Execute_ShouldMapAllFieldsCorrectly_WhenSelectingAllColumnsForSingleStudent()
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

    #endregion

    #region Where Single Condition Tests

    [Theory]
    [InlineData(17.00, new[] { "97100166" })]
    [InlineData(19.00, new[] { "97100112", "98100201" })]
    [InlineData(20.00, new string[0])]
    public void Execute_ShouldFilterByExactGrade_WhenDefaultEqualityWhereOverloadIsUsed(
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
    public void Execute_ShouldFilterByGradeComparisonOperator_WhenExplicitOperatorIsProvided(
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

    #endregion

    #region Logical Operator (AND / OR) Combination Tests

    [Fact]
    public void Execute_ShouldFilterByRange_WhenMultipleWhereClausesAreChainedWithAnd()
    {
        // Arrange 
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 18.00m)
            .Where("grade", ExpressionOperatorType.LessThanOrEqual, 19.50m);

        // Act
        var actualStudentNumbers = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        actualStudentNumbers.Should().BeEquivalentTo(new[] { "98100201", "97100112" });
    }

    [Fact]
    public void Execute_ShouldReturnMatchingStudents_WhenOrWhereIsCombinedWithGradeAndStudentNumber()
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .OrWhere("grade", ExpressionOperatorType.GreaterThanOrEqual, 19.00m)
            .OrWhere("studentnumber", "99100305");

        // Act
        var actualStudentNumbers = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        actualStudentNumbers.Should().BeEquivalentTo(new[] { "98100201", "97100112", "99100305" });
    }

    [Fact]
    public void Execute_ShouldReturnMatchingStudents_WhenMultipleOrWhereClausesAreChainedForStudentNumbers()
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .OrWhere("studentnumber", ExpressionOperatorType.Equals, "98100201")
            .OrWhere("studentnumber", ExpressionOperatorType.Equals, "97100999");

        // Act
        var actualStudentNumbers = _sut.Execute(query, reader => reader.GetString(0));

        // Assert
        actualStudentNumbers.Should().BeEquivalentTo(new[] { "98100201", "97100999" });
    }

    #endregion
}