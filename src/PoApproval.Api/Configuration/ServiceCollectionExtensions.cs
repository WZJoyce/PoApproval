using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PoApproval.Api.Services;
using PoApproval.Domain.Advisory;
using PoApproval.Domain.Configuration;
using PoApproval.Domain.Repositories;
using PoApproval.Domain.Services;
using PoApproval.Infrastructure.Advisory;
using PoApproval.Infrastructure.Persistence;
using PoApproval.Infrastructure.Persistence.Repositories;



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

        //Domain services
        services.AddScoped<IApprovalService, ApprovalService>();

        //Application services       
        services.AddScoped<IOrderCreationService, OrderCreationService>();
        services.AddScoped<IOrderQueryService, OrderQueryService>();
        services.AddScoped<IOrderTransitionService, OrderTransitionService>();


        return services;
    }

    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Connection string 'Default' not found.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            });
        });

        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

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

    public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>() ?? ["http://localhost:5173"];

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    public static IServiceCollection AddAdvisoryServices(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.Configure<AzureOpenAIOptions>(
            configuration.GetSection(AzureOpenAIOptions.SectionName));

        services.AddScoped<IApprovalAdvisor, AzureOpenAIApprovalAdvisor>();
        services.AddScoped<OrderRecommendationService>();

        return services;
    }

}
