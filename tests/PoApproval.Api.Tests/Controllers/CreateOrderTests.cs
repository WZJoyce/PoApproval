using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PoApproval.Api.Contracts.V1;
using PoApproval.Api.Tests.Infrastructure;
using PoApproval.Domain.Enums;

namespace PoApproval.Api.Tests.Controllers;

public sealed class CreateOrderTests : IntegrationTestBase
{
    public CreateOrderTests(DatabaseFixture database) : base(database) { }

    [Fact]
    public async Task Create_WithValidRequest_Returns201AndPersistsDraft()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("User-Id", "alice");

        var request = new CreatePurchaseOrderRequest("PO-2026-001", 1500m);

        var response = await client.PostAsJsonAsync("/api/v1/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var details = await response.Content.ReadFromJsonAsync<PurchaseOrderDetails>();
        details.Should().NotBeNull();
        details!.OrderNo.Should().Be("PO-2026-001");
        details.Amount.Should().Be(1500m);
        details.Status.Should().Be(PurchaseOrderStatus.Draft);
        details.CreatedBy.Should().Be("alice");
        details.ReviewedBy.Should().BeNull();
        details.RejectionReason.Should().BeNull();
    }

    [Fact]
    public async Task Create_WithoutUserHeader_Returns400()
    {
        var client = Factory.CreateClient();
        // Intentionally not setting User-Id

        var request = new CreatePurchaseOrderRequest("PO-2026-002", 500m);

        var response = await client.PostAsJsonAsync("/api/v1/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithDuplicateOrderNo_Returns409()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("User-Id", "alice");

        var request = new CreatePurchaseOrderRequest("PO-2026-003", 100m);

        var first = await client.PostAsJsonAsync("/api/v1/orders", request);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await client.PostAsJsonAsync("/api/v1/orders", request);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Create_WithInvalidOrderNo_Returns400()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("User-Id", "alice");

        var request = new CreatePurchaseOrderRequest("AB", 100m);  // < 3 chars, fails [StringLength]

        var response = await client.PostAsJsonAsync("/api/v1/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithNegativeAmount_Returns400()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("User-Id", "alice");

        var request = new CreatePurchaseOrderRequest("PO-2026-005", -10m);  // fails [Range]

        var response = await client.PostAsJsonAsync("/api/v1/orders", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
