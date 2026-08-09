using System;
using System.Collections.Generic;
using CleanCode;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace cleanCode.test.Compilers;

public class SqlServerCompilerTests
{
    private readonly IParameterIdentifier _paramIdentifierMock;
    private readonly ISqlCommonCompiler _commonCompilerMock;
    private readonly SqlServerCompiler _sut;

    public SqlServerCompilerTests()
    {
        _paramIdentifierMock = Substitute.For<IParameterIdentifier>();
        _commonCompilerMock = Substitute.For<ISqlCommonCompiler>();

        _sut = new SqlServerCompiler(_paramIdentifierMock, _commonCompilerMock);
    }

    [Fact]
    public void Compile_ShouldGenerateCorrectSqlInput_WhenQueryIsValid()
    {
        // Arrange
        var query = new Query()
            .From("Student")
            .Select("StudentNumber", "FirstName")
            .Where("Grade", ExpressionOperatorType.GreaterThanOrEqual, 16.0);

        var expectedResult = new DataBaseInput(
            "SELECT [StudentNumber], [FirstName] FROM [Student] WHERE [Grade] >= @p0",
            new List<object> { 16.0 }
        );

        _commonCompilerMock.Compile(query).Returns(expectedResult);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.QueryString.Should().Be("SELECT [StudentNumber], [FirstName] FROM [Student] WHERE [Grade] >= @p0");
        result.Bindings.Should().ContainSingle().Which.Should().Be(16.0);
        _commonCompilerMock.Received(1).Compile(query);
    }

    [Fact]
    public void Compile_ShouldDelegateCompilation_WhenCalledOnCommonCompiler()
    {
        // Arrange
        var expectedResult = new DataBaseInput("SELECT * FROM [Student]", new List<object>());
        var query = new Query().From("Student");

        _commonCompilerMock.Compile(query).Returns(expectedResult);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.QueryString.Should().Be(expectedResult.QueryString);
        _commonCompilerMock.Received(1).Compile(query);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenParamIdentifierIsNull()
    {
        // Act
        Action act = () => new SqlServerCompiler(null!, _commonCompilerMock);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("paramIdentifier");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenCommonCompilerIsNull()
    {
        // Act
        Action act = () => new SqlServerCompiler(_paramIdentifierMock, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("commonCompiler");
    }

    [Fact]
    public void Compile_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        // Act
        Action act = () => _sut.Compile(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("query");
    }

    [Fact]
    public void FormatParameterName_ShouldCallParamIdentifier_WhenIndexIsProvided()
    {
        // Arrange
        const int index = 1;
        const string expectedFormat = "@p1";

        _paramIdentifierMock.FormatParameterName(index).Returns(expectedFormat);

        // Act
        var result = _sut.FormatParameterName(index);

        // Assert
        result.Should().Be(expectedFormat);
        _paramIdentifierMock.Received(1).FormatParameterName(index);
    }

    [Theory]
    [ClassData(typeof(SqlServerTestData))]
    public void Compile_ShouldGenerateExpectedSql_WhenUsingClassData(
        Query query,
        string expectedSql,
        object[] expectedBindings)
    {
        // Arrange
        var expectedResult = new DataBaseInput(expectedSql, new List<object>(expectedBindings));
        _commonCompilerMock.Compile(query).Returns(expectedResult);

        // Act
        var result = _sut.Compile(query);

        // Assert
        result.QueryString.Should().Be(expectedSql);
        result.Bindings.Should().Equal(expectedBindings);
        _commonCompilerMock.Received(1).Compile(query);
    }
}