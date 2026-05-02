using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PoApproval.Api.Tests;

public class SmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task UnknownPath_ReturnsNotFound()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/unknown-path");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task OrdersEndpoint_ReturnsNotImplemented()
    {
        var client = _factory.CreateClient();           

        var response = await client.GetAsync("/api/v1/orders");                     

        response.StatusCode.Should().Be(HttpStatusCode.NotImplemented);
    }
    
    [Fact]
    public async Task GetOrderbyId_WithInvalidIdContraint_ReturnsNotFound()
    {
        var client = _factory.CreateClient();           

        var response = await client.GetAsync("/api/v1/orders/0");                     

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

}