using System;
using CleanCode;
using FluentAssertions;
using Xunit;

namespace cleanCode.test.Builders;

public class SqlExpressionOperatorTests
{
    private readonly SqlExpressionOperator _sut;

    public SqlExpressionOperatorTests()
    {
        _sut = new SqlExpressionOperator();
    }

    [Fact]
    public void GetSymbol_ShouldThrowArgumentOutOfRangeException_WhenOperatorTypeIsInvalid()
    {
        // Arrange
        var invalidType = (ExpressionOperatorType)999;

        // Act
        Action act = () => _sut.GetSymbol(invalidType);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(ExpressionOperatorType.Equals, "=")]
    [InlineData(ExpressionOperatorType.NotEquals, "<>")]
    [InlineData(ExpressionOperatorType.GreaterThan, ">")]
    [InlineData(ExpressionOperatorType.GreaterThanOrEqual, ">=")]
    [InlineData(ExpressionOperatorType.LessThan, "<")]
    [InlineData(ExpressionOperatorType.LessThanOrEqual, "<=")]
    [InlineData(ExpressionOperatorType.Like, "LIKE")]
    public void GetSymbol_ShouldReturnCorrectSqlSymbol_WhenOperatorTypeIsValid(
        ExpressionOperatorType operatorType, 
        string expectedSymbol)
    {
        // Act
        var result = _sut.GetSymbol(operatorType);

        // Assert
        result.Should().Be(expectedSymbol);
    }
}