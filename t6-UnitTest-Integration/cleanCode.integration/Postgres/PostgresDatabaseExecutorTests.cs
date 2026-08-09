using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using CleanCode;
using FluentAssertions;
using Npgsql;
using Xunit;

namespace cleanCode.integration.postgres;

public class PostGreSqlTests : IClassFixture<PostgresContainerFixture>
{
    private readonly PostgresContainerFixture _fixture;
    private readonly SqlExpressionOperator _expressionOperator;
    private readonly PostgresParameterIdentifier _postgresParameterIdentifier;
    private readonly SqlFromBuilder _fromBuilder;
    private readonly SqlWhereBuilder _whereBuilder;
    private readonly SqlSelectBuilder _selectBuilder;
    private readonly SqlCompilerCommon _commonCompiler;
    private readonly PostgresCompiler _postgresCompiler;

    public PostGreSqlTests(PostgresContainerFixture fixture)
    {
        _fixture = fixture;

        _expressionOperator = new SqlExpressionOperator();
        _postgresParameterIdentifier = new PostgresParameterIdentifier();
        _fromBuilder = new SqlFromBuilder(_postgresParameterIdentifier);
        _whereBuilder = new SqlWhereBuilder(_postgresParameterIdentifier, _expressionOperator);
        _selectBuilder = new SqlSelectBuilder(_postgresParameterIdentifier);
        _commonCompiler = new SqlCompilerCommon(_fromBuilder, _selectBuilder, _whereBuilder);
        _postgresCompiler = new PostgresCompiler(_postgresParameterIdentifier, _commonCompiler);
    }

    [Fact]
    public async Task Execute_ShouldThrowArgumentException_WhenTableNameIsNull()
    {
        // Arrange
        await using var connection = await _fixture.CreateConnectionAsync();
        var sut = new DatabaseExecutor(connection, _postgresCompiler);

        var query = new Query()
            .Select("studentnumber")
            .Where("age", ExpressionOperatorType.Equals, 20);

        // Act
        Action executeAction = () => sut.Execute(query, reader => reader.GetString(0));

        // Assert
        executeAction.Should().Throw<ArgumentException>()
            .WithMessage("Table name cannot be null or empty.");
    }

    [Theory]
    [InlineData(ExpressionOperatorType.GreaterThan, 17f, new[] { "98100201", "97100112", "01100523", "04100866" })]
    [InlineData(ExpressionOperatorType.LessThan, 5f, new[] { "00100412", "96100888", "98100456" })]
    [InlineData(ExpressionOperatorType.Equals, 20f, new[] { "01100523" })]
    [InlineData(ExpressionOperatorType.GreaterThanOrEqual, 15.5f,
        new[] { "98100201", "97100112", "97100999", "01100523", "04100866", "03100111" })]
    public async Task Execute_ShouldReturnFilteredStudents_WhenGradeConditionIsApplied(
        ExpressionOperatorType expressionOperator,
        float targetGrade,
        string[] expectedStudentNumbers)
    {
        // Arrange
        await using var connection = await _fixture.CreateConnectionAsync();
        var sut = new DatabaseExecutor(connection, _postgresCompiler);

        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .Where("grade", expressionOperator, targetGrade);

        // Act
        var result = sut.Execute(query, reader => reader.GetString(0));

        // Assert
        result.Should().BeEquivalentTo(expectedStudentNumbers);
    }

    [Theory]
    [InlineData(ExpressionOperatorType.GreaterThan, 22, new[] { "97100112", "97100999", "96100888", "98100456" })]
    [InlineData(ExpressionOperatorType.LessThan, 18,
        new[] { "02100644", "03100755", "04100866", "02100321", "03100111" })]
    [InlineData(ExpressionOperatorType.GreaterThanOrEqual, 24, new[] { "97100112", "97100999", "96100888" })]
    [InlineData(ExpressionOperatorType.LessThanOrEqual, 16, new[] { "03100755", "04100866", "03100111" })]
    public async Task Execute_ShouldReturnFilteredStudents_WhenAgeConditionIsApplied(
        ExpressionOperatorType expressionOperator,
        int targetAge,
        string[] expectedStudentNumbers)
    {
        // Arrange
        await using var connection = await _fixture.CreateConnectionAsync();
        var sut = new DatabaseExecutor(connection, _postgresCompiler);

        var query = new Query()
            .From("student")
            .Select("studentnumber")
            .Where("age", expressionOperator, targetAge);

        // Act
        var result = sut.Execute(query, reader => reader.GetString(0));

        // Assert
        result.Should().BeEquivalentTo(expectedStudentNumbers);
    }

    [Theory]
    [InlineData(20, 20f, new[] { "99100305", "01100523" })]
    [InlineData(22, 18.75f, new[] { "98100201" })]
    public async Task Execute_ShouldReturnStudentNumbers_WhenOrWhereConditionsAreAppliedWithoutSelect(
        int targetAge,
        float targetGrade,
        string[] expectedStudentNumbers)
    {
        // Arrange
        await using var connection = await _fixture.CreateConnectionAsync();
        var sut = new DatabaseExecutor(connection, _postgresCompiler);

        var query = new Query()
            .From("student")
            .OrWhere("age", targetAge)
            .OrWhere("grade", targetGrade);

        // Act
        var result = sut.Execute(query, reader => reader.GetString(0));

        // Assert
        result.Should().BeEquivalentTo(expectedStudentNumbers);
    }
}