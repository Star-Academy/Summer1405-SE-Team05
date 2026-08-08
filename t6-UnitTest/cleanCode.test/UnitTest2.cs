using CleanCode;
using NSubstitute;

namespace cleanCode.test;

public class UnitTest2
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

        var compiler = new SqlServerCompiler(
            sqlServerParameterIdentifier,
            commonCompiler
        );

        var query = new Query()
            .From("Student")
            .Select("StudentNumber", "FirstName")
            .Where("Grade", ExpressionOperatorType.GreaterThanOrEqual, 16.0);

        // Act
        var result = compiler.Compile(query);

        // Assert
        Assert.Equal("SELECT [StudentNumber], [FirstName] FROM [Student] WHERE [Grade] >= @p0",
            result.QueryString);
        Assert.Single(result.Bindings);
        Assert.Equal(16.0, result.Bindings[0]);
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

        var compiler = new SqlServerCompiler(
            substituteParamIdentifier,
            substituteCommonCompiler
        );

        // Act
        var result = compiler.Compile(query);

        // Assert
        Assert.Equal(expectedResult.QueryString, result.QueryString);

        substituteCommonCompiler
            .Received(1)
            .Compile(query);
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

        var compiler = new SqlServerCompiler(
            sqlServerParameterIdentifier,
            commonCompiler);

        // Act
        var result = compiler.Compile(query);

        // Assert
        Assert.Equal(expectedSql, result.QueryString);
        Assert.Equal(expectedBindings, result.Bindings);
    }
}