using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SplitBillsapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });
            services.AddMvc();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CheckRole",
                    policy => policy
                    .RequireClaim("Role", "SignInUser"));
            });
            //services.AddSwaggerGen(c =>
            //{
            //    //c.SwaggerDoc("v1.0", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Split Bills api", Version = "v1.0" });
            //    c.SwaggerDoc("v1.0",new Swashbuckle.AspNetCore.Swagger.Info { Title = "Split Bills api"});
            //});
            //services.AddSwaggerGen();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddMediatR();
            // Configure versions 
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });
            services.AddSwaggerGen(options =>
            {
                // Specify two versions 
                options.SwaggerDoc("v1",
                    new Info()
                    {
                        Version = "v1",
                        Title = "v1 API",
                        Description = "v1 API Description",
                        TermsOfService = "Terms of usage v1"
                    });

                options.SwaggerDoc("v2",
                    new Info()
                    {
                        Version = "v2",
                        Title = "v2 API",
                        Description = "v2 API Description",
                        TermsOfService = "Terms of usage v2"
                    });

                // This call remove version from parameter, without it we will have version as parameter 
                // for all endpoints in swagger UI
                options.OperationFilter<RemoveVersionFromParameter>();

                // This make replacement of v{version:apiVersion} to real version of corresponding swagger doc.
                options.DocumentFilter<ReplaceVersionWithExactValueInPath>();

                // This on used to exclude endpoint mapped to not specified in swagger version.
                // In this particular example we exclude 'GET /api/v2/Values/otherget/three' endpoint,
                // because it was mapped to v3 with attribute: MapToApiVersion("3")
                options.DocInclusionPredicate((version, desc) =>
                {
                    var versions = desc.ControllerAttributes()
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    var maps = desc.ActionAttributes()
                        .OfType<MapToApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions)
                        .ToArray();

                    return versions.Any(v => $"v{v.ToString()}" == version) && (maps.Length == 0 || maps.Any(v => $"v{v.ToString()}" == version));
                });

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "v1");
                c.SwaggerEndpoint($"/swagger/v2/swagger.json", $"v2");
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"v1");
            });
        }
    }
    public class RemoveVersionFromParameter : IOperationFilter
    {
        public void Apply(Swashbuckle.AspNetCore.Swagger.Operation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.Single(p => p.Name == "version");
            operation.Parameters.Remove(versionParameter);
        }
    }

    public class ReplaceVersionWithExactValueInPath : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Paths = swaggerDoc.Paths
                .ToDictionary(
                    path => path.Key.Replace("v{version}", swaggerDoc.Info.Version),
                    path => path.Value
                );
        }
    }
}
