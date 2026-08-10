using System;
using System.Collections.Generic;
using CleanCode;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace cleanCode.test.Compilers;

public class PostgresCompilerTests
{
    private readonly IParameterIdentifier _mockParamIdentifier;
    private readonly ISqlCommonCompiler _mockCommonCompiler;
    private readonly PostgresCompiler _sut;

    public PostgresCompilerTests()
    {
        _mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        _mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        _sut = new PostgresCompiler(_mockParamIdentifier, _mockCommonCompiler);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenParamIdentifierIsNull()
    {
        // Act
        var act = () => new PostgresCompiler(null!, _mockCommonCompiler);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("paramIdentifier");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenCommonCompilerIsNull()
    {
        // Act
        var act = () => new PostgresCompiler(_mockParamIdentifier, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("commonCompiler");
    }

    [Fact]
    public void Compile_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        // Act
        var act = () => _sut.Compile(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("query");
    }
    
    [Fact]
    public void Compile_ShouldThrowArgumentException_WhenFromClauseIsMissing()
    {
        // Arrange
        var queryWithoutFrom = new Query().Select("studentnumber");

        _mockCommonCompiler.Compile(queryWithoutFrom)
            .Returns(_ => throw new ArgumentException("Table name cannot be null or empty."));

        // Act
        var act = () => _sut.Compile(queryWithoutFrom);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Table name cannot be null or empty.");
    }

    [Fact]
    public void Compile_ShouldReturnDataBaseInput_WhenQueryIsValid()
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("studentnumber", "firstname")
            .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 16);

        var expectedResult = new DataBaseInput(
            "SELECT \"studentnumber\", \"firstname\" FROM \"student\" WHERE \"grade\" >= $1",
            new List<object> { 16 }
        );

        _mockCommonCompiler.Compile(query).Returns(expectedResult);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.QueryString.Should().Be("SELECT \"studentnumber\", \"firstname\" FROM \"student\" WHERE \"grade\" >= $1");
        result.Bindings.Should().ContainSingle().Which.Should().Be(16);
        _mockCommonCompiler.Received(1).Compile(query);
    }

    [Fact]
    public void Compile_ShouldDelegateCompilation_WhenCalledOnCommonCompiler()
    {
        // Arrange
        var expectedResult = new DataBaseInput("SELECT * FROM \"student\"", new List<object>());
        var query = new Query().From("student");

        _mockCommonCompiler.Compile(query).Returns(expectedResult);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.QueryString.Should().Be(expectedResult.QueryString);
        _mockCommonCompiler.Received(1).Compile(query);
    }

    [Fact]
    public void Compile_ShouldUseMockedExpressionOperator_WhenWhereClauseIsCompiled()
    {
        // Arrange
        var query = new Query()
            .From("student")
            .Select("studentnumber", "firstname")
            .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 16);

        var mockOperator = Substitute.For<IExpressionOperator>();
        mockOperator.GetSymbol(ExpressionOperatorType.GreaterThanOrEqual).Returns(">=");

        var mockWhereBuilder = Substitute.For<IWhereBuilder>();
        var mockFromBuilder = Substitute.For<IFromBuilder>();
        var mockSelectBuilder = Substitute.For<ISelectBuilder>();

        var expectedResult = new DataBaseInput(
            "SELECT \"studentnumber\", \"firstname\" FROM \"student\" WHERE \"grade\" >= $1",
            new List<object> { 16 }
        );

        _mockCommonCompiler.Compile(query).Returns(info =>
        {
            mockOperator.GetSymbol(ExpressionOperatorType.GreaterThanOrEqual);
            return expectedResult;
        });

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.QueryString.Should().Be("SELECT \"studentnumber\", \"firstname\" FROM \"student\" WHERE \"grade\" >= $1");
        result.Bindings.Should().ContainSingle().Which.Should().Be(16);
        mockOperator.Received(1).GetSymbol(ExpressionOperatorType.GreaterThanOrEqual);
    }

    [Theory]
    [ClassData(typeof(PostgresTestData))]
    public void Compile_ShouldReturnExpectedSqlQuery_WhenQueryMatchesClassData(
        Query query,
        string expectedSqlQuery,
        object[] expectedBindings)
    {
        // Arrange
        var expectedResult = new DataBaseInput(expectedSqlQuery, new List<object>(expectedBindings));
        _mockCommonCompiler.Compile(query).Returns(expectedResult);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.QueryString.Should().Be(expectedSqlQuery);
        result.Bindings.Should().Equal(expectedBindings);
        _mockCommonCompiler.Received(1).Compile(query);
    }

    [Fact]
    public void FormatParameterName_ShouldCallParamIdentifier_WhenIndexIsProvided()
    {
        // Arrange
        const int index = 2;
        const string expectedFormat = "$3";

        _mockParamIdentifier.FormatParameterName(index).Returns(expectedFormat);

        // Act
        var result = _sut.FormatParameterName(index);

        // Assert
        result.Should().Be(expectedFormat);
        _mockParamIdentifier.Received(1).FormatParameterName(index);
    }
}