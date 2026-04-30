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
    public async Task WeatherForecast_Returns200_AndJsonArray()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/weatherforecast");// Call the /weatherforecast endpoint

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);// Check if the status code is 200 OK
        response.Content.Headers.ContentType?.MediaType
            .Should().Be("application/json");// Check if the content type is application/json

        var body = await response.Content.ReadAsStringAsync();
        body.Should().StartWith("[");  // JSON Array starts with '['
    }

    [Fact]
    public async Task UnknownPath_Returns404()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/this-route-does-not-exist");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}