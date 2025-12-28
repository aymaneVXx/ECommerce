using ECommerce.Infrastructure.DependencyInjection;
using ECommerce.Application.DependencyInjection;
using System.Text.Json.Serialization;
using Serilog;
using Serilog.Events;

try
{
    // Configuration de Serilog
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "Logs/log-.txt",
            rollingInterval: RollingInterval.Day)
        .CreateLogger();

    Log.Information("Application is building");

    var builder = WebApplication.CreateBuilder(args);

    // Brancher Serilog au Host
    builder.Host.UseSerilog();

    // Brancher CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy
                .SetIsOriginAllowed(_ => true) 
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    // Dependency Injection 
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddApplicationServices();

    builder.Services.AddControllers()
        .AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

    // Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Pipeline 
    app.UseInfrastructureService(); // middleware d’exception global
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors("CorsPolicy");
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Application is running");

    app.Run();
}
catch (Exception ex)
{
    Log.Error(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}
