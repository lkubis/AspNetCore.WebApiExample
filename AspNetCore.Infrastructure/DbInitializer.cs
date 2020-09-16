using AspNetCore.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.Infrastructure
{
    public class DbInitializer
    {
        public void Initialize(IServiceProvider services)
        {
            var productManager = services.GetService<IProductManager>();

            // Debug data
            var random = new Random();
            for (int i = 0; i < 100; i++)
                productManager.CreateAsync($"Name_{i}", $"Description_{i}", $"/api/resources/{i}", (decimal)random.NextDouble() * 1000).GetAwaiter().GetResult();

            productManager.SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
