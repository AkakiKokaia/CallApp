using CallApp.API.Middlewares;
using CallApp.Application;
using CallApp.Infrastructure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Globalization;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            builder.WebHost.UseKestrel(options =>
            {
                options.Limits.MaxRequestLineSize = 1048576;
                options.Limits.MaxRequestBufferSize = 1048576;
                options.Limits.MaxRequestBodySize = 100 * 1024 * 1024;
            })
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var env = hostingContext.HostingEnvironment;
                        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                        config.AddEnvironmentVariables();
                    })
                    //.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    //    .ReadFrom.Configuration(hostingContext.Configuration))
                    .UseIISIntegration();

            IConfiguration configuration = builder.Configuration;

            builder.Services.AddControllers()
                            .AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining(typeof(Program));
                fv.ValidatorOptions.LanguageManager.Enabled = true;
                fv.LocalizationEnabled = true;
            });
            builder.Services.AddLocalization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authentication with JWT Token",
                    Type = SecuritySchemeType.Http
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new List<string>()
                        }
                    });
            });
            var queueName = "default";
            if (builder.Environment.IsDevelopment())
            {
                queueName = "development";
            }
            // Registering Databases
            builder.Services.AddInfrastructureLayer(configuration);

            // Authentication Services
            builder.Services.AddAuthenticationServices(configuration);

            builder.Services.AddApplicationLayer();

            builder.Services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            var origins = builder.Configuration.GetValue<string>("OriginsToAllow");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultAuth",
                    builder =>
                    {
                        builder
                        .WithOrigins(origins.Split(";"))
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });
        }

        var app = builder.Build();
        {

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            var supportedCultures = new List<CultureInfo>
                {
                    CultureInfo.InvariantCulture,
                    new CultureInfo("en-US"),
                    // Add other supported cultures as needed
                };
            //var supportedCultures = new List<CultureInfo>
            //    {
            //        new CultureInfo("ka-GE"),
            //        new CultureInfo("en-US"),
            //        //new CultureInfo("ru-RU"),
            //        //new CultureInfo("uk-UA")
            //    };

            //var options = new RequestLocalizationOptions
            //{
            //    DefaultRequestCulture = new RequestCulture("ka-GE"),
            //    SupportedCultures = supportedCultures,
            //    SupportedUICultures = supportedCultures
            //};

            var serviceProvider = builder.Services.BuildServiceProvider();

            var context = serviceProvider.GetService<CallAppDBContext>();

            DBInitializer.InitializeDatabase(app.Services, context);

            //app.UseRequestLocalization(options);

            app.MapControllers();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseCors("DefaultAuth");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}