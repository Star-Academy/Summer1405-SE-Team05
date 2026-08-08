using System;
using System.Collections.Generic;
using CleanCode;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace cleanCode.test.Compilers;
public class SqlServerCompilerTests
{
    [Fact]
    public void SqlServerCompiler_Should_Generate_Correct_SqlInput()
    {
        // Arrange
        var expressionOperator = new SqlExpressionOperator();
        var sqlServerParameterIdentifier = new SqlServerParameterIdentifier();
        var fromBuilder = new SqlFromBuilder(sqlServerParameterIdentifier);
        var whereBuilder = new SqlWhereBuilder(sqlServerParameterIdentifier, expressionOperator);
        var selectBuilder = new SqlSelectBuilder(sqlServerParameterIdentifier);
        var commonCompiler = new SqlCompilerCommon(fromBuilder, selectBuilder, whereBuilder);

        var sut = new SqlServerCompiler(
            sqlServerParameterIdentifier,
            commonCompiler
        );

        var query = new Query()
            .From("Student")
            .Select("StudentNumber", "FirstName")
            .Where("Grade", ExpressionOperatorType.GreaterThanOrEqual, 16.0);

        // Act
        var result = sut.Compile(query);

        // Assert
        result.QueryString.Should().Be("SELECT [StudentNumber], [FirstName] FROM [Student] WHERE [Grade] >= @p0");
        result.Bindings.Should().ContainSingle().Which.Should().Be(16.0);
    }

    [Fact]
    public void SqlServerCompiler_Should_Delegate_Compilation_To_CommonCompiler()
    {
        // Arrange
        var substituteParamIdentifier = Substitute.For<IParameterIdentifier>();
        var substituteCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        var expectedResult = new DataBaseInput("SELECT * FROM [Student]", new List<object>());
        var query = new Query().From("Student");

        substituteCommonCompiler
            .Compile(query)
            .Returns(expectedResult);

        var sut = new SqlServerCompiler(
            substituteParamIdentifier,
            substituteCommonCompiler
        );

        // Act
        var result = sut.Compile(query);

        // Assert
        result.QueryString.Should().Be(expectedResult.QueryString);

        substituteCommonCompiler
            .Received(1)
            .Compile(query);
    }

    [Fact]
    public void SqlServerCompiler_Constructor_NullParamIdentifier_ThrowsArgumentNullException()
    {
        // Arrange
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        // Act
        Action act = () => new SqlServerCompiler(null!, mockCommonCompiler);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SqlServerCompiler_Constructor_NullCommonCompiler_ThrowsArgumentNullException()
    {
        // Arrange
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();

        // Act
        Action act = () => new SqlServerCompiler(mockParamIdentifier, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SqlServerCompiler_Compile_NullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();
        var sut = new SqlServerCompiler(mockParamIdentifier, mockCommonCompiler);

        // Act
        Action act = () => sut.Compile(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SqlServerCompiler_FormatParameterName_Calls_ParamIdentifier()
    {
        // Arrange
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        mockParamIdentifier.FormatParameterName(1).Returns("@p1");
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        var sut = new SqlServerCompiler(mockParamIdentifier, mockCommonCompiler);

        // Act
        var result = sut.FormatParameterName(1);

        // Assert
        result.Should().Be("@p1");
        mockParamIdentifier.Received(1).FormatParameterName(1);
    }

    [Theory]
    [ClassData(typeof(SqlServerTestData))]
    public void SqlServerCompiler_Should_Generate_Expected_Sql_Using_ClassData(
        Query query,
        string expectedSql,
        object[] expectedBindings)
    {
        // Arrange
        var expressionOperator = new SqlExpressionOperator();
        var sqlServerParameterIdentifier = new SqlServerParameterIdentifier();
        var fromBuilder = new SqlFromBuilder(sqlServerParameterIdentifier);
        var whereBuilder = new SqlWhereBuilder(sqlServerParameterIdentifier, expressionOperator);
        var selectBuilder = new SqlSelectBuilder(sqlServerParameterIdentifier);
        var commonCompiler = new SqlCompilerCommon(fromBuilder, selectBuilder, whereBuilder);

        var sut = new SqlServerCompiler(
            sqlServerParameterIdentifier,
            commonCompiler);

        // Act
        var result = sut.Compile(query);

        // Assert
        result.QueryString.Should().Be(expectedSql);
        result.Bindings.Should().Equal(expectedBindings);
    }
}