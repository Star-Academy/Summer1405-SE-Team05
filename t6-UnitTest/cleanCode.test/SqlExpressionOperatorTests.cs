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
}