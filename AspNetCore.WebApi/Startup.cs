using AspNetCore.Domain;
using AspNetCore.Infrastructure;
using AspNetCore.Infrastructure.Managers;
using AspNetCore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddControllers(options => options.Filters.Add(new HttpResponseExceptionFilter()));

            services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase("DB"));
            services.AddTransient<DbInitializer>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProductManager, ProductManager>();
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
