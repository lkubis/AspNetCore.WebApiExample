using AspNetCore.Domain;
using AspNetCore.WebApi.DTOs;

namespace AspNetCore.WebApi.Extensions
{
    public static class ProductExtensions
    {
        public static ProductDTO ToDTO(this Product product)
        {
            return new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImgUri = product.ImgUri,
                Price = product.Price
            };
        }
    }
}
