using ASP.services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace Asp.test;

public class DbContextManagerTests
{
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbContextManagerTests()
    {
        _config = Substitute.For<IConfiguration>();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        _config.GetConnectionString("SqlServerConnection").Returns("Server=localhost;Database=TestDb;");
        _config.GetConnectionString("PostgresConnection").Returns("Host=localhost;Database=TestDb;");
    }

    [Fact]
    public void CurrentDatabase_ShouldReturnDefaultPostgresql_WhenHeaderIsMissing()
    {
        // Arrange
        var context = new DefaultHttpContext();
        _httpContextAccessor.HttpContext.Returns(context);

        var manager = new DbContextManager(_config, _httpContextAccessor);

        // Act
        // Assert
        manager.CurrentDatabase.Should().Be("Postgresql");
    }

    [Fact]
    public void CurrentDatabase_ShouldReturnHeaderValue_WhenHeaderIsPresent()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Database-Type"] = "SqlServer";
        _httpContextAccessor.HttpContext.Returns(context);

        var manager = new DbContextManager(_config, _httpContextAccessor);

        // Act
        // Assert
        manager.CurrentDatabase.Should().Be("SqlServer");
    }

    [Fact]
    public void GetQueryFactory_ShouldReturnQueryFactory_WhenDatabaseTypeIsSqlServer()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Database-Type"] = "SqlServer";
        _httpContextAccessor.HttpContext.Returns(context);

        var manager = new DbContextManager(_config, _httpContextAccessor);

        // Act
        var factory = manager.GetQueryFactory();

        // Assert
        factory.Should().NotBeNull();
        factory.Compiler.GetType().Name.Should().Be("SqlServerCompiler");
    }

    [Fact]
    public void GetQueryFactory_ShouldReturnQueryFactory_WhenDatabaseTypeIsPostgresql()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Database-Type"] = "Postgresql";
        _httpContextAccessor.HttpContext.Returns(context);

        var manager = new DbContextManager(_config, _httpContextAccessor);

        // Act
        var factory = manager.GetQueryFactory();

        // Assert
        factory.Should().NotBeNull();
        factory.Compiler.GetType().Name.Should().Be("PostgresCompiler");
    }

    [Fact]
    public void GetQueryFactory_ShouldThrowArgumentException_WhenDatabaseTypeIsInvalid()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Database-Type"] = "MongoDB";
        _httpContextAccessor.HttpContext.Returns(context);

        var manager = new DbContextManager(_config, _httpContextAccessor);

        // Act
        Action act = () => manager.GetQueryFactory();

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Invalid database type: 'MongoDB'*");
    }
}