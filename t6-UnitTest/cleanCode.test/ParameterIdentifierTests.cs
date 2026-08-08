using CleanCode;
using Xunit;

namespace cleanCode.test;

public class ParameterIdentifierTests
{
    [Fact]
    public void PostgresParameterIdentifier_FormatParameterName_Should_Return_Empty_String()
    {
        var sut = new PostgresParameterIdentifier();

        var result = sut.FormatParameterName(0);

        Assert.Equal("", result);
    }

    [Fact]
    public void SqlServerParameterIdentifier_FormatParameterName_Should_Return_Formatted_Name()
    {
        var sut = new SqlServerParameterIdentifier();

        var result = sut.FormatParameterName(0);

        Assert.Equal("@p0", result);
    }
}