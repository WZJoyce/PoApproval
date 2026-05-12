using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PoApproval.Api.Contracts.V1;
using PoApproval.Api.Tests.Infrastructure;

namespace PoApproval.Api.Tests.Controllers;

public sealed class GetOrderByIdTests : IntegrationTestBase
{
    public GetOrderByIdTests(DatabaseFixture database) : base(database) { }

    [Fact]
    public async Task GetById_ExistingOrder_Returns200()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("User-Id", "alice");

        var created = await client.PostAsJsonAsync("/api/v1/orders", new CreatePurchaseOrderRequest("PO-100", 250m));
        var createdDetails = await created.Content.ReadFromJsonAsync<PurchaseOrderDetails>();

        var response = await client.GetAsync($"/api/v1/orders/{createdDetails!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var details = await response.Content.ReadFromJsonAsync<PurchaseOrderDetails>();
        details!.OrderNo.Should().Be("PO-100");
    }

    [Fact]
    public async Task GetById_NonExistent_Returns404WithProblemDetails()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/api/v1/orders/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType
            .Should().Be("application/problem+json");
    }
}
