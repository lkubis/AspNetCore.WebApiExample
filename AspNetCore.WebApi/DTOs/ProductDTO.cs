using System.ComponentModel.DataAnnotations;

namespace AspNetCore.WebApi.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string ImgUri { get; set; }

        public decimal Price { get; set; }
    }
}
