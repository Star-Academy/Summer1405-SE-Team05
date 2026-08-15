using System.IO;
using System.Net;
using System.Text.Json;
using ASP.middlewares;
using ASP.models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Asp.test;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldReturn400BadRequest_WhenArgumentExceptionIsThrown()
    {
        // Arrange
        var exceptionMessage = "Invalid database type: 'Oracle'.";
        RequestDelegate next = (ctx) => throw new ArgumentException(exceptionMessage);
        
        var middleware = new ExceptionHandlingMiddleware(next);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<string>>(responseBody, jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Message.Should().Be(exceptionMessage);
    }

    [Fact]
    public async Task InvokeAsync_ShouldPassThrough_WhenNoExceptionIsThrown()
    {
        // Arrange
        var wasNextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            wasNextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ExceptionHandlingMiddleware(next);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        wasNextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task InvokeAsync_ShouldReturn500InternalServerError_WhenGenericExceptionIsThrown()
    {
        // Arrange
        RequestDelegate next = (ctx) => throw new Exception("Database fail");
        var middleware = new ExceptionHandlingMiddleware(next);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }
}