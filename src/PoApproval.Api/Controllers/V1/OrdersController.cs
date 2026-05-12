using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PoApproval.Api.Contracts.V1;
using PoApproval.Domain.Entities;
using PoApproval.Domain.Enums;
using PoApproval.Domain.Services;

namespace PoApproval.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
[Produces("application/json")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderCreationService _creation;
    private readonly IOrderQueryService _query;
    private readonly IOrderTransitionService _transition;

    public OrdersController(
        IOrderCreationService creation,
        IOrderQueryService query,
        IOrderTransitionService transition)
    {
        _creation = creation;
        _query = query;
        _transition = transition;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PurchaseOrderSummary>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<PurchaseOrderSummary>>> List(
        [FromQuery] PurchaseOrderStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await _query.ListAsync(status, page, pageSize, cancellationToken);

        var summaries = result.Items.Select(ToSummary).ToList();

        return Ok(new PagedResponse<PurchaseOrderSummary>(
            summaries,
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages,
            result.HasMore));
    }

    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(typeof(PurchaseOrderDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseOrderDetails>> GetById(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var order = await _query.GetByIdAsync(id, cancellationToken);
        return Ok(ToDetails(order));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderDetails), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PurchaseOrderDetails>> Create(
        [FromBody] CreatePurchaseOrderRequest request,
        [FromHeader(Name = "User-Id")] string userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Missing user identity",
                Detail = "User-Id header is required.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var order = await _creation.CreateDraftAsync(
            request.OrderNo,
            request.Amount,
            userId,
            cancellationToken);

        var details = ToDetails(order);

        return CreatedAtAction(nameof(GetById), new { id = order.Id, version = "1.0" }, details);
    }

    [HttpPost("{id:int:min(1)}/submit")]
    [ProducesResponseType(typeof(PurchaseOrderDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PurchaseOrderDetails>> Submit(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var order = await _transition.SubmitAsync(id, cancellationToken);
        return Ok(ToDetails(order));
    }

    [HttpPost("{id:int:min(1)}/approve")]
    [ProducesResponseType(typeof(PurchaseOrderDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PurchaseOrderDetails>> Approve(
        [FromRoute] int id,
        [FromBody] ApprovePurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _transition.ApproveAsync(id, request.Approver, cancellationToken);
        return Ok(ToDetails(order));
    }

    [HttpPost("{id:int:min(1)}/reject")]
    [ProducesResponseType(typeof(PurchaseOrderDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PurchaseOrderDetails>> Reject(
        [FromRoute] int id,
        [FromBody] RejectPurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await _transition.RejectAsync(id, request.Approver, request.Reason, cancellationToken);
        return Ok(ToDetails(order));
    }

    private static PurchaseOrderSummary ToSummary(PurchaseOrder order)
        => new(order.Id, order.OrderNo, order.Amount, order.Status, order.CreatedBy, order.CreatedAt);

    private static PurchaseOrderDetails ToDetails(PurchaseOrder order)
        => new(
            order.Id,
            order.OrderNo,
            order.Amount,
            order.Status,
            order.CreatedBy,
            order.CreatedAt,
            order.ReviewedBy,
            order.ReviewedAt,
            order.RejectionReason);
}
