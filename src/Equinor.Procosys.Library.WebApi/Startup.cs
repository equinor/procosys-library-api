using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Equinor.Procosys.Library.WebApi.DIModules;
using Equinor.Procosys.Library.WebApi.Middleware;
using Equinor.Procosys.Library.WebApi.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Equinor.Procosys.Library.WebApi
{
    public class Startup
    {
        private const string AllowAllOriginsCorsPolicy = "AllowAllOrigins";
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            _environment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //if (_environment.IsDevelopment())
            //{
            //    if (Configuration.GetValue<bool>("MigrateDatabase"))
            //    {
            //        services.AddHostedService<DatabaseMigrator>();
            //    }
            //}

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    Configuration.Bind("API", options);
                });

            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllOriginsCorsPolicy,
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    return new ValidationFailedResult(actionContext.ModelState);
                };
            })
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            if (Configuration.GetValue<bool>("UseAzureAppConfiguration"))
            {
                services.AddAzureAppConfiguration();
            }

            services.AddControllers();

            var scopes = Configuration.GetSection("Swagger:Scopes")?.Get<Dictionary<string, string>>() ?? new Dictionary<string, string>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProCoSys Library API", Version = "v1" });

                //Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(Configuration["Swagger:AuthorizationUrl"]),
                            Scopes = scopes
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        scopes.Keys.ToArray()
                    }
                });
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            services.AddMediatrModules();
            services.AddApplicationModules(Configuration);
            services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = Configuration["ApplicationInsights:ConnectionString"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Configuration.GetValue<bool>("UseAzureAppConfiguration"))
            {
                app.UseAzureAppConfiguration();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.AddGlobalExceptionHandling();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProCoSys Library API V1");
                c.DocExpansion(DocExpansion.List);
                c.DisplayRequestDuration();

                c.OAuthClientId(Configuration["Swagger:ClientId"]);
                c.OAuthAppName("ProCoSys Library API V1");
                c.OAuthScopeSeparator(" ");
                c.OAuthAdditionalQueryStringParams(new Dictionary<string, string> { { "resource", Configuration["API:Audience"] } });
            });

            app.UseCors(AllowAllOriginsCorsPolicy);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseResponseCompression();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
