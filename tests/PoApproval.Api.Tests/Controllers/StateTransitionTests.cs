using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PoApproval.Api.Contracts.V1;
using PoApproval.Api.Tests.Infrastructure;
using PoApproval.Domain.Enums;

namespace PoApproval.Api.Tests.Controllers;

public sealed class StateTransitionTests : IntegrationTestBase
{
    public StateTransitionTests(DatabaseFixture database) : base(database) { }

    private async Task<int> CreateDraftAsync(HttpClient client, string orderNo, string user = "alice")
    {
        client.DefaultRequestHeaders.Remove("User-Id");
        client.DefaultRequestHeaders.Add("User-Id", user);

        var response = await client.PostAsJsonAsync("/api/v1/orders", new CreatePurchaseOrderRequest(orderNo, 100m));

        var details = await response.Content.ReadFromJsonAsync<PurchaseOrderDetails>();
        return details!.Id;
    }

    [Fact]
    public async Task Submit_Draft_TransitionsToSubmitted()
    {
        var client = Factory.CreateClient();
        var id = await CreateDraftAsync(client, "PO-S1");

        var response = await client.PostAsync($"/api/v1/orders/{id}/submit", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var details = await response.Content.ReadFromJsonAsync<PurchaseOrderDetails>();
        details!.Status.Should().Be(PurchaseOrderStatus.Submitted);
    }

    [Fact]
    public async Task Submit_AlreadySubmitted_Returns409()
    {
        var client = Factory.CreateClient();
        var id = await CreateDraftAsync(client, "PO-S2");
        await client.PostAsync($"/api/v1/orders/{id}/submit", null);

        var response = await client.PostAsync($"/api/v1/orders/{id}/submit", null);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Approve_SubmittedByDifferentUser_TransitionsToApproved()
    {
        var client = Factory.CreateClient();
        var id = await CreateDraftAsync(client, "PO-A1", user: "alice");
        await client.PostAsync($"/api/v1/orders/{id}/submit", null);

        var response = await client.PostAsJsonAsync(
            $"/api/v1/orders/{id}/approve",
            new ApprovePurchaseOrderRequest("bob"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var details = await response.Content.ReadFromJsonAsync<PurchaseOrderDetails>();
        details!.Status.Should().Be(PurchaseOrderStatus.Approved);
        details.ReviewedBy.Should().Be("bob");
    }

    [Fact]
    public async Task Approve_BySameUser_Returns422()
    {
        var client = Factory.CreateClient();
        var id = await CreateDraftAsync(client, "PO-A2", user: "alice");
        await client.PostAsync($"/api/v1/orders/{id}/submit", null);

        var response = await client.PostAsJsonAsync(
            $"/api/v1/orders/{id}/approve",
            new ApprovePurchaseOrderRequest("alice"));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Reject_Submitted_TransitionsToRejected()
    {
        var client = Factory.CreateClient();
        var id = await CreateDraftAsync(client, "PO-R1");
        await client.PostAsync($"/api/v1/orders/{id}/submit", null);

        var response = await client.PostAsJsonAsync(
            $"/api/v1/orders/{id}/reject",
            new RejectPurchaseOrderRequest("bob", "Budget exceeded for the month."));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var details = await response.Content.ReadFromJsonAsync<PurchaseOrderDetails>();
        details!.Status.Should().Be(PurchaseOrderStatus.Rejected);
        details.RejectionReason.Should().Be("Budget exceeded for the month.");
    }

    [Fact]
    public async Task Submit_NonExistent_Returns404()
    {
        var client = Factory.CreateClient();

        var response = await client.PostAsync("/api/v1/orders/9999/submit", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
