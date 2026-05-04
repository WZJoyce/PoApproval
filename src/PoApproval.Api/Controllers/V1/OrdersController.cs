using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PoApproval.Api.Contracts.V1;
using PoApproval.Domain.Enums;

namespace PoApproval.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
[Produces("application/json")]
public sealed class OrdersController : ControllerBase
{
    /// <summary>
    /// Lists purchase orders, optionally filtered by status.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PurchaseOrderSummary>), StatusCodes.Status200OK)]
    public IActionResult List([FromQuery] PurchaseOrderStatus? status, CancellationToken cancellationToken)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// Retrieves a single purchase order by id.
    /// </summary>
    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType(typeof(PurchaseOrderDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// Creates a new purchase order in Draft status.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseOrderDetails), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] CreatePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// Submits a draft purchase order for approval.
    /// </summary>
    [HttpPost("{id:int:min(1)}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Submit([FromRoute] int id, CancellationToken cancellationToken)
    {
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    /// <summary>
    /// Approves a submitted purchase order.
    /// </summary>
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

    /// <summary>
    /// Rejects a submitted purchase order.
    /// </summary>
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
