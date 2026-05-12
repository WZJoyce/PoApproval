using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PoApproval.Api.Contracts.V1;
using PoApproval.Api.Tests.Infrastructure;
using PoApproval.Domain.Enums;

namespace PoApproval.Api.Tests.Controllers;

public sealed class ListOrdersTests : IntegrationTestBase
{
    public ListOrdersTests(DatabaseFixture database) : base(database) { }

    [Fact]
    public async Task List_OnEmptyDatabase_ReturnsEmptyResult()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/api/v1/orders");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var paged = await response.Content.ReadFromJsonAsync<PagedResponse<PurchaseOrderSummary>>();
        paged.Should().NotBeNull();
        paged!.Items.Should().BeEmpty();
        paged.TotalCount.Should().Be(0);
        paged.HasMore.Should().BeFalse();
    }

    [Fact]
    public async Task List_WithMultipleOrders_ReturnsNewestFirst()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("User-Id", "alice");

        await client.PostAsJsonAsync("/api/v1/orders", new CreatePurchaseOrderRequest("PO-A", 100m));
        await Task.Delay(10);  // ensure CreatedAt differs
        await client.PostAsJsonAsync("/api/v1/orders", new CreatePurchaseOrderRequest("PO-B", 200m));

        var response = await client.GetAsync("/api/v1/orders");
        var paged = await response.Content.ReadFromJsonAsync<PagedResponse<PurchaseOrderSummary>>();

        paged!.Items.Should().HaveCount(2);
        paged.Items[0].OrderNo.Should().Be("PO-B");  // newest first
        paged.Items[1].OrderNo.Should().Be("PO-A");
    }

    [Fact]
    public async Task List_WithStatusFilter_OnlyReturnsMatching()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("User-Id", "alice");

        await client.PostAsJsonAsync("/api/v1/orders", new CreatePurchaseOrderRequest("PO-DRAFT", 100m));
        var submitted = await client.PostAsJsonAsync("/api/v1/orders", new CreatePurchaseOrderRequest("PO-SUB", 200m));
        var submittedDetails = await submitted.Content.ReadFromJsonAsync<PurchaseOrderDetails>();
        await client.PostAsync($"/api/v1/orders/{submittedDetails!.Id}/submit", null);

        var response = await client.GetAsync($"/api/v1/orders?status={(int)PurchaseOrderStatus.Submitted}");
        var paged = await response.Content.ReadFromJsonAsync<PagedResponse<PurchaseOrderSummary>>();

        paged!.Items.Should().HaveCount(1);
        paged.Items[0].OrderNo.Should().Be("PO-SUB");
    }

    [Fact]
    public async Task List_WithExcessivePageSize_ClampsToMax()
    {
        var client = Factory.CreateClient();

        var response = await client.GetAsync("/api/v1/orders?pageSize=10000");
        var paged = await response.Content.ReadFromJsonAsync<PagedResponse<PurchaseOrderSummary>>();

        paged!.PageSize.Should().Be(100);  // clamped to max
    }
}
