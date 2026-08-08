using System;
using System.Collections.Generic;
using CleanCode;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace cleanCode.test;

public class PostgresCompilerTests
{
    [Fact]
    public void PostgresCompiler_Should_Generate_Correct_SqlInput()
    {
        var expressionOperator = new SqlExpressionOperator();
        var postgresParameterIdentifier = new PostgresParameterIdentifier();
        var fromBuilder = new SqlFromBuilder(postgresParameterIdentifier);
        var whereBuilder = new SqlWhereBuilder(postgresParameterIdentifier, expressionOperator);
        var selectBuilder = new SqlSelectBuilder(postgresParameterIdentifier);
        var commonCompiler = new SqlCompilerCommon(fromBuilder, selectBuilder, whereBuilder);

        var sut = new PostgresCompiler(
            postgresParameterIdentifier,
            commonCompiler
        );

        var query = new Query()
            .From("student")
            .Select("studentnumber", "firstname")
            .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 16);

        var result = sut.Compile(query);

        result.QueryString.Should().Be("SELECT \"studentnumber\", \"firstname\" FROM \"student\" WHERE \"grade\" >= $1");
        result.Bindings.Should().ContainSingle().Which.Should().Be(16);
    }

    [Fact]
    public void PostgresCompiler_Should_Delegate_Compilation_To_CommonCompiler()
    {
        var substituteParamIdentifier = Substitute.For<IParameterIdentifier>();
        var substituteCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        var expectedResult = new DataBaseInput("SELECT * FROM \"student\"", new List<object>());
        var query = new Query().From("student");

        substituteCommonCompiler
            .Compile(query)
            .Returns(expectedResult);

        var sut = new PostgresCompiler(
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
    public void PostgresCompiler_Mocked_ExpressionOperator()
    {
        var pgIdentifier = new PostgresParameterIdentifier();

        var mockOperator = Substitute.For<IExpressionOperator>();
        mockOperator.GetSymbol(ExpressionOperatorType.GreaterThanOrEqual).Returns(">=");
        var postgresParameterIdentifier = new PostgresParameterIdentifier();
        var fromBuilder = new SqlFromBuilder(postgresParameterIdentifier);
        var whereBuilder = new SqlWhereBuilder(postgresParameterIdentifier, mockOperator);
        var selectBuilder = new SqlSelectBuilder(postgresParameterIdentifier);
        var commonCompiler = new SqlCompilerCommon(fromBuilder, selectBuilder, whereBuilder);

        var sut = new PostgresCompiler(pgIdentifier, commonCompiler);

        var query = new Query()
            .From("student")
            .Select("studentnumber", "firstname")
            .Where("grade", ExpressionOperatorType.GreaterThanOrEqual, 16);

        var result = sut.Compile(query);

        result.QueryString.Should().Be("SELECT \"studentnumber\", \"firstname\" FROM \"student\" WHERE \"grade\" >= $1");
        result.Bindings.Should().ContainSingle().Which.Should().Be(16);

        mockOperator.Received(1).GetSymbol(ExpressionOperatorType.GreaterThanOrEqual);
    }

    [Fact]
    public void PostgresCompiler_Constructor_NullParamIdentifier_ThrowsArgumentNullException()
    {
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        Action act = () => new PostgresCompiler(null!, mockCommonCompiler);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PostgresCompiler_Constructor_NullCommonCompiler_ThrowsArgumentNullException()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();

        Action act = () => new PostgresCompiler(mockParamIdentifier, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PostgresCompiler_Compile_NullQuery_ThrowsArgumentNullException()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();
        var sut = new PostgresCompiler(mockParamIdentifier, mockCommonCompiler);

        Action act = () => sut.Compile(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void PostgresCompiler_FormatParameterName_Calls_ParamIdentifier()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        mockParamIdentifier.FormatParameterName(2).Returns("$3");
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        var sut = new PostgresCompiler(mockParamIdentifier, mockCommonCompiler);
        var result = sut.FormatParameterName(2);

        result.Should().Be("$3");
        mockParamIdentifier.Received(1).FormatParameterName(2);
    }

    [Theory]
    [ClassData(typeof(PostgresTestData))]
    public void PostgresCompiler_Should_Generate_Expected_Sql_Using_ClassData(
        Query query,
        string expectedSql,
        object[] expectedBindings)
    {
        var expressionOperator = new SqlExpressionOperator();
        var postgresParameterIdentifier = new PostgresParameterIdentifier();
        var fromBuilder = new SqlFromBuilder(postgresParameterIdentifier);
        var whereBuilder = new SqlWhereBuilder(postgresParameterIdentifier, expressionOperator);
        var selectBuilder = new SqlSelectBuilder(postgresParameterIdentifier);
        var commonCompiler = new SqlCompilerCommon(fromBuilder, selectBuilder, whereBuilder);

        var sut = new PostgresCompiler(
            postgresParameterIdentifier,
            commonCompiler);

        var result = sut.Compile(query);

        result.QueryString.Should().Be(expectedSql);
        result.Bindings.Should().Equal(expectedBindings);
    }
}