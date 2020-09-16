using AspNetCore.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspNetCore.WebApi.Controllers
{
    [ApiController]
    [Route("api/v2/products")]
    public class ProductV2Controller : ProductV1Controller
    {
        public ProductV2Controller(IProductManager productManager)
            : base(productManager)
        {
        }

        [HttpGet]
        [Route("{pageNumber:int:min(1)}/{pageSize:int:range(1,5000)}")]
        public async Task<IActionResult> FindAllAsync(int pageNumber = 1, int pageSize = 50)
            => Ok(await ProductManager.FindAllPagedAsync(pageNumber, pageSize));
    }
}
