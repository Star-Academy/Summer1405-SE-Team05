using ASP.integration.fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ASP.integration.fixtures;

public class PostgresWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgresContainerFixture _fixture;

    public PostgresWebApplicationFactory(PostgresContainerFixture fixture)
    {
        _fixture = fixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgresConnection"] = _fixture.ConnectionString
            });
        });
    }
}