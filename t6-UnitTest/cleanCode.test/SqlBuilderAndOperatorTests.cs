using System;
using CleanCode;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace cleanCode.test;

public class SqlBuilderAndOperatorTests
{
    [Fact]
    public void SqlFromBuilder_Constructor_NullParamIdentifier_ThrowsArgumentNullException()
    {
        Action act = () => new SqlFromBuilder(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SqlFromBuilder_Build_NullOrWhiteSpaceTable_ThrowsArgumentException(string? tableName)
    {
        var mockParamIdentifier = Substitute.For<IParameterIdentifier>();
        var sut = new SqlFromBuilder(mockParamIdentifier);

        Action act = () => sut.Build(tableName);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SqlExpressionOperator_GetSymbol_InvalidOperatorType_ThrowsArgumentOutOfRangeException()
    {
        var sut = new SqlExpressionOperator();
        var invalidType = (ExpressionOperatorType)999;

        Action act = () => sut.GetSymbol(invalidType);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}