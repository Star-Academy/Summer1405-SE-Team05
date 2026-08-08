using System;
using CleanCode;
using NSubstitute;
using Xunit;

namespace cleanCode.test;

public class SqlBuilderAndOperatorTests
{
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
        var sut = new SqlFromBuilder(mockParamIdentifier);

        Assert.Throws<ArgumentException>(() => sut.Build(tableName));
    }

    [Fact]
    public void SqlExpressionOperator_GetSymbol_InvalidOperatorType_ThrowsArgumentOutOfRangeException()
    {
        var sut = new SqlExpressionOperator();
        var invalidType = (ExpressionOperatorType)999;

        Assert.Throws<ArgumentOutOfRangeException>(() => sut.GetSymbol(invalidType));
    }
}