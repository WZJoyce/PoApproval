using Asp.Versioning;
using PoApproval.Domain.Configuration;
using PoApproval.Domain.Services;

namespace PoApproval.Api.Configuration;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers domain-layer services and their configuration bindings.
    /// </summary>
    public static IServiceCollection AddDomainServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<ApprovalSettings>()
            .Bind(configuration.GetSection(ApprovalSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IClock, SystemClock>();

        return services;
    }

    /// <summary>
    /// Configures URL-segment based API versioning with sensible defaults.
    /// </summary>
    public static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}