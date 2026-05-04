using Asp.Versioning;
using Microsoft.OpenApi.Models;
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

    public static IServiceCollection AddOpenApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<OpenApiOptions>()
            .Bind(configuration.GetSection(OpenApiOptions.SectionName));

        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, context, _) =>
            {
                var settings = configuration
                    .GetSection(OpenApiOptions.SectionName)
                    .Get<OpenApiOptions>() ?? new OpenApiOptions();

                document.Info = new OpenApiInfo
                {
                    Title = settings.Title,
                    Version = "v1",
                    Description = settings.Description,
                    Contact = new OpenApiContact
                    {
                        Name = settings.Contact.Name,
                        Url = string.IsNullOrWhiteSpace(settings.Contact.Url)
                            ? null
                            : new Uri(settings.Contact.Url)
                    }
                };

                return Task.CompletedTask;
            });
        });

        return services;
    }

}
