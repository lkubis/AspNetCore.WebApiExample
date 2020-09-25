using AspNetCore.Domain;
using AspNetCore.Infrastructure;
using AspNetCore.Infrastructure.Managers;
using AspNetCore.Infrastructure.Repositories;
using AspNetCore.WebApi.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Net;

namespace AspNetCore.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.Filters.Add(new HttpResponseExceptionFilter());
                    options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status500InternalServerError));
                    options.RespectBrowserAcceptHeader = false;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("DB"));
            services.AddTransient<DbInitializer>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProductManager, ProductManager>();

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1.0", new OpenApiInfo()
                {
                    Title = "Products API",
                    Version = "v1.0",
                    Description = "Products API Example"
                });
                x.SwaggerDoc("v2.0", new OpenApiInfo()
                {
                    Title = "Products API",
                    Version = "v2.0",
                    Description = "Products API Example"
                });

                x.OperationFilter<RemoveVersionParameterFilter>();
                x.OperationFilter<ResponseHeadersFilter>();
                x.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
                x.EnableAnnotations();

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "AspNetCore.WebApi.xml");
                x.IncludeXmlComments(filePath);
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Products API v1.0");
                x.SwaggerEndpoint("/swagger/v2.0/swagger.json", "Products API v2.0");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var initializer = scope.ServiceProvider.GetService<DbInitializer>();
                if (initializer != null)
                    initializer.Initialize(scope.ServiceProvider);
            }


        }
    }
}
