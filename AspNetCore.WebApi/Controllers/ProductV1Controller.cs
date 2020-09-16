using AspNetCore.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductV1Controller : ControllerBase
    {
        protected IProductManager ProductManager { get; }

        public ProductV1Controller(IProductManager productManager)
        {
            ProductManager = productManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> FindAllAsync()
            => Ok(await ProductManager.FindAll().AsNoTracking().ToListAsync());

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> FindByIdAsyn(int id, CancellationToken cancellationToken = default)
            => Ok(await ProductManager.FindByIdAsync(id, cancellationToken));

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateNameAsync(int id, [FromBody] string name, CancellationToken cancellationToken = default)
            => Ok(await ProductManager.UpdateNameAsync(id, name, cancellationToken));
    }
}
