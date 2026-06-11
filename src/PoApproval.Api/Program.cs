using PoApproval.Api.Configuration;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddDomainServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApiVersioningServices();
builder.Services.AddOpenApiServices(builder.Configuration);
builder.Services.AddCorsServices(builder.Configuration);
builder.Services.AddAdvisoryServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<PoApproval.Api.Middleware.DomainExceptionMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "PoApproval API";
        options.Theme = ScalarTheme.BluePlanet;
    });
}

//app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();

public partial class Program;
