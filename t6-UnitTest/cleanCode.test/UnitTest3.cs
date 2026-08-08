using System;
using CleanCode;
using NSubstitute;
using Xunit;

namespace cleanCode.test;

public class UnitTest3
{
    #region PostgresCompiler Tests

    [Fact]
    public void PostgresCompiler_Constructor_NullParamIdentifier_ThrowsArgumentNullException()
    {
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();
        Assert.Throws<ArgumentNullException>(() => new PostgresCompiler(null!, mockCommonCompiler));
    }

    [Fact]
    public void PostgresCompiler_Constructor_NullCommonCompiler_ThrowsArgumentNullException()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        Assert.Throws<ArgumentNullException>(() => new PostgresCompiler(mockParamIdentifier, null!));
    }

    [Fact]
    public void PostgresCompiler_Compile_NullQuery_ThrowsArgumentNullException()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();
        var compiler = new PostgresCompiler(mockParamIdentifier, mockCommonCompiler);

        Assert.Throws<ArgumentNullException>(() => compiler.Compile(null!));
    }

    [Fact]
    public void PostgresCompiler_FormatParameterName_Calls_ParamIdentifier()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        mockParamIdentifier.FormatParameterName(2).Returns("$3");
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        var compiler = new PostgresCompiler(mockParamIdentifier, mockCommonCompiler);
        var result = compiler.FormatParameterName(2);

        Assert.Equal("$3", result);
        mockParamIdentifier.Received(1).FormatParameterName(2);
    }

    #endregion

    #region SqlServerCompiler Tests

    [Fact]
    public void SqlServerCompiler_Constructor_NullParamIdentifier_ThrowsArgumentNullException()
    {
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();
        Assert.Throws<ArgumentNullException>(() => new SqlServerCompiler(null!, mockCommonCompiler));
    }

    [Fact]
    public void SqlServerCompiler_Constructor_NullCommonCompiler_ThrowsArgumentNullException()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        Assert.Throws<ArgumentNullException>(() => new SqlServerCompiler(mockParamIdentifier, null!));
    }

    [Fact]
    public void SqlServerCompiler_Compile_NullQuery_ThrowsArgumentNullException()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();
        var compiler = new SqlServerCompiler(mockParamIdentifier, mockCommonCompiler);

        Assert.Throws<ArgumentNullException>(() => compiler.Compile(null!));
    }

    [Fact]
    public void SqlServerCompiler_FormatParameterName_Calls_ParamIdentifier()
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        mockParamIdentifier.FormatParameterName(1).Returns("@p1");
        var mockCommonCompiler = Substitute.For<ISqlCommonCompiler>();

        var compiler = new SqlServerCompiler(mockParamIdentifier, mockCommonCompiler);
        var result = compiler.FormatParameterName(1);

        Assert.Equal("@p1", result);
        mockParamIdentifier.Received(1).FormatParameterName(1);
    }

    #endregion

    #region SqlFromBuilder Tests

    [Fact]
    public void SqlFromBuilder_Constructor_NullParamIdentifier_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new SqlFromBuilder(null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SqlFromBuilder_Build_NullOrWhiteSpaceTable_ThrowsArgumentException(string? tableName)
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        var builder = new SqlFromBuilder(mockParamIdentifier);

        Assert.Throws<ArgumentException>(() => builder.Build(tableName));
    }

    #endregion

    #region SqlExpressionOperator Tests

    [Fact]
    public void SqlExpressionOperator_GetSymbol_InvalidOperatorType_ThrowsArgumentOutOfRangeException()
    {
        var op = new SqlExpressionOperator();
        var invalidType = (ExpressionOperatorType)999;

        Assert.Throws<ArgumentOutOfRangeException>(() => op.GetSymbol(invalidType));
    }

    #endregion
}