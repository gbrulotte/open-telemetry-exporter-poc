using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Genetec.Enrollment.Management.Api;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class Startup
{
    public static readonly ActivitySource ActivitySource = new("Genetec.Service2", "1.0");

    public static readonly Meter Meter = new("Genetec.Service2", "v1.0");
#pragma warning disable SA1306 // Field names should begin with lower-case letter
    //private static readonly MeterProvider? Provider;
#pragma warning restore SA1306 // Field names should begin with lower-case letter

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
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(Configuration.GetValue<string>("OTEL_EXPORTER_OTLP_ENDPOINT"));
                }));

        // Adding the OtlpExporter creates a GrpcChannel.
        // This switch must be set before creating a GrpcChannel/HttpClient when calling an insecure gRPC service.
        // See: https://docs.microsoft.com/aspnet/core/grpc/troubleshoot#call-insecure-grpc-services-with-net-core-client
        //        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        //#pragma warning disable S2696 // Instance members should not write to "static" fields
        //        Provider = OpenTelemetry.Sdk.CreateMeterProviderBuilder()
        //#pragma warning restore S2696 // Instance members should not write to "static" fields
        //            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Genetec.Enrollment.Management.Service"))
        //            .AddSource("Genetec.Enrollment.Management.Service")
        //            .AddAspNetCoreInstrumentation()
        //            .AddHttpClientInstrumentation()
        //            .AddConsoleExporter()
        //.AddOtlpExporter(o =>
        //{
        //    o.Endpoint = new Uri("http://host.docker.internal:4317");
        ////})
        //.Build();
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
