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
}