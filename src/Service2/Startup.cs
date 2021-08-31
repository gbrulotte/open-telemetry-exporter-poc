using System.Diagnostics;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Genetec.Service2;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class Startup
{
    public static readonly ActivitySource ActivitySource = new("Genetec.Service2", "1.0");

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status406NotAcceptable));
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
            options.Filters.Add(new ProducesDefaultResponseTypeAttribute());
            options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.AllowTrailingCommas = true;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Service2", Version = "v1" });

            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

            c.UseInlineDefinitionsForEnums();
        });

        services.AddHealthChecks();

        services.AddDbContext<LocationContext>(options =>
                options.UseSqlServer(Configuration.GetValue<string>("ConnectionString")));

        services.AddOpenTelemetryTracing(builder =>
            builder
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Service2"))
                .AddSource("Genetec.Service2")
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = (httpContext) =>
                    {
                        // Filter out requests on health endpoint.
                        return !httpContext.Request.Path.Equals("/health");
                    };
                })
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(Configuration.GetValue<string>("OTEL_EXPORTER_OTLP_ENDPOINT"));
                }));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Service v1");
            options.RoutePrefix = string.Empty;
        });

        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
