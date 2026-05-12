using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PoApproval.Domain.Exceptions;

namespace PoApproval.Api.Middleware;

/// <summary>
/// Translates domain-layer exceptions into ProblemDetails responses.
/// </summary>
public sealed class DomainExceptionMiddleware
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<DomainExceptionMiddleware> _logger;

    public DomainExceptionMiddleware(
        RequestDelegate next,
        ILogger<DomainExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogInformation(
                "Entity not found: {EntityName} key={Key}",
                ex.EntityName, ex.Key);

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status404NotFound,
                "Resource not found",
                ex.Message);
        }
        catch (InvalidStateTransitionException ex)
        {
            _logger.LogInformation(
                "Invalid state transition from {CurrentStatus} to {AttemptedTransition}",
                ex.CurrentStatus, ex.AttemptedTransition);

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status409Conflict,
                "Invalid state transition",
                ex.Message,
                extensions: new Dictionary<string, object?>
                {
                    ["currentStatus"] = ex.CurrentStatus.ToString(),
                    ["attemptedTransition"] = ex.AttemptedTransition.ToString()
                });
        }
        catch (BusinessRuleViolationException ex)
        {
            _logger.LogInformation(
                "Business rule violated: {RuleCode}",
                ex.RuleCode);

            var statusCode = MapRuleCodeToStatusCode(ex.RuleCode);

            await WriteProblemDetailsAsync(
                context,
                statusCode,
                "Business rule violation",
                ex.Message,
                extensions: new Dictionary<string, object?>
                {
                    ["ruleCode"] = ex.RuleCode
                });
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail,
        IDictionary<string, object?>? extensions = null)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (extensions is not null)
        {
            foreach (var (key, value) in extensions)
            {
                problem.Extensions[key] = value;
            }
        }

        var json = JsonSerializer.Serialize(problem, _jsonOptions);
        await context.Response.WriteAsync(json);
    }

    private static int MapRuleCodeToStatusCode(string ruleCode)
    {
        switch (ruleCode)
        {
            case "ORDER_NO_DUPLICATE":
                return StatusCodes.Status409Conflict;

            default:
                return StatusCodes.Status422UnprocessableEntity;
        }
    }
}
