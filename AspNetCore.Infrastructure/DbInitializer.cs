using AspNetCore.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AspNetCore.Infrastructure
{
    public class DbInitializer
    {
        public void Initialize(IServiceProvider services)
        {
            var dataContext = services.GetService<DataContext>();

            // Debug data
            var hasProducts = dataContext.Products.Any();
            if (!hasProducts)
            {
                var random = new Random();
                for (int i = 0; i < 100; i++)
                {
                    dataContext.Products.Add(new Product()
                    {
                        Name = $"Name_{i}",
                        Description = $"Description_{i}",
                        ImgUri = $"/api/resources/{i}",
                        Price = (decimal)random.NextDouble() * 1000
                    });
                }

                dataContext.SaveChanges();
            }
        }
    }
}
