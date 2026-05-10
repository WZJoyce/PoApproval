using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PoApproval.Api.Contracts.V1;
using PoApproval.Domain.Enums;
using PoApproval.Domain.Exceptions;
using PoApproval.Domain.Services;

namespace PoApproval.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
[Produces("application/json")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderCreationService _orderCreationService;

    public OrdersController(IOrderCreationService orderCreationService)
    {
        _orderCreationService = orderCreationService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PurchaseOrderSummary>), StatusCodes.Status200OK)]
    public IActionResult List([FromQuery] PurchaseOrderStatus? status, CancellationToken cancellationToken)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(typeof(PurchaseOrderDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderDetails), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePurchaseOrderRequest request,
        [FromHeader(Name = "User-Id")] string userId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Missing user identity",
                Detail = "X-User-Id header is required.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            var order = await _orderCreationService.CreateDraftAsync(
                request.OrderNo,
                request.Amount,
                userId,
                cancellationToken);

            var details = new PurchaseOrderDetails(
                order.Id,
                order.OrderNo,
                order.Amount,
                order.Status,
                order.CreatedBy,
                order.CreatedAt,
                order.ReviewedBy,
                order.ReviewedAt,
                order.RejectionReason);

            return CreatedAtAction(nameof(GetById), new { id = order.Id, version = "1.0" }, details);
        }
        catch (BusinessRuleViolationException ex) when (ex.RuleCode == "ORDER_NO_DUPLICATE")
        {
            return Conflict(new ProblemDetails
            {
                Title = "Duplicate order",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict,
                Extensions = { ["ruleCode"] = ex.RuleCode }
            });
        }
    }

    [HttpPost("{id:int:min(1)}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Submit([FromRoute] int id, CancellationToken cancellationToken)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPost("{id:int:min(1)}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Approve(
        [FromRoute] int id,
        [FromBody] ApprovePurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPost("{id:int:min(1)}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Reject(
        [FromRoute] int id,
        [FromBody] RejectPurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}
