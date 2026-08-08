using System;
using System.Collections.Generic;
using CleanCode;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace cleanCode.test;

public class SqlServerCompilerTests
{
    [Fact]
    public void SqlServerCompiler_Should_Generate_Correct_SqlInput()
    {
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

        var result = sut.Compile(query);

        result.QueryString.Should().Be("SELECT [StudentNumber], [FirstName] FROM [Student] WHERE [Grade] >= @p0");
        result.Bindings.Should().ContainSingle().Which.Should().Be(16.0);
    }

    [Fact]
    public void SqlServerCompiler_Should_Delegate_Compilation_To_CommonCompiler()
    {
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

        var result = sut.Compile(query);

        result.QueryString.Should().Be(expectedResult.QueryString);

        substituteCommonCompiler
            .Received(1)
            .Compile(query);
    }

    [Fact]
    public void SqlServerCompiler_Constructor_NullParamIdentifier_ThrowsArgumentNullException()
    {
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        Action act = () => new SqlServerCompiler(null!, mockCommonCompiler);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SqlServerCompiler_Constructor_NullCommonCompiler_ThrowsArgumentNullException()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();

        Action act = () => new SqlServerCompiler(mockParamIdentifier, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SqlServerCompiler_Compile_NullQuery_ThrowsArgumentNullException()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();
        var sut = new SqlServerCompiler(mockParamIdentifier, mockCommonCompiler);

        Action act = () => sut.Compile(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SqlServerCompiler_FormatParameterName_Calls_ParamIdentifier()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        mockParamIdentifier.FormatParameterName(1).Returns("@p1");
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        var sut = new SqlServerCompiler(mockParamIdentifier, mockCommonCompiler);
        var result = sut.FormatParameterName(1);

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
        var expressionOperator = new SqlExpressionOperator();
        var sqlServerParameterIdentifier = new SqlServerParameterIdentifier();
        var fromBuilder = new SqlFromBuilder(sqlServerParameterIdentifier);
        var whereBuilder = new SqlWhereBuilder(sqlServerParameterIdentifier, expressionOperator);
        var selectBuilder = new SqlSelectBuilder(sqlServerParameterIdentifier);
        var commonCompiler = new SqlCompilerCommon(fromBuilder, selectBuilder, whereBuilder);

        var sut = new SqlServerCompiler(
            sqlServerParameterIdentifier,
            commonCompiler);

        var result = sut.Compile(query);

        result.QueryString.Should().Be(expectedSql);
        result.Bindings.Should().Equal(expectedBindings);
    }
}