using AspNetCore.Domain;
using AspNetCore.WebApi.DTOs;
using AspNetCore.WebApi.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/products")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        protected IProductManager ProductManager { get; }

        public ProductController(IProductManager productManager)
        {
            ProductManager = productManager;
        }

        /// <summary>
        /// Finds all products.
        /// </summary>
        /// <remarks>Returns all products.</remarks>
        /// <response code="200">Successful operation.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("", Name = "FindAllProducts")]
        [ProducesResponseType(typeof(List<ProductDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindAllAsync()
            => Ok((await ProductManager.FindAll().AsNoTracking().ToListAsync()).Select(x => x.ToDTO()));


        /// <summary>
        /// Finds product by ID.
        /// </summary>
        /// <remarks>Returns a single product.</remarks>
        /// <param name="id">ID of product to return.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <response code="200">Successful operation.</response>
        /// <response code="404">The product was not found.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("{id}", Name = "FindProductById")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindByIdAsyn(int id, CancellationToken cancellationToken = default)
            => Ok((await ProductManager.FindByIdAsync(id, cancellationToken)).ToDTO());

        /// <summary>
        /// Updates name of product by ID.
        /// </summary>
        /// <param name="id">ID of product to update.</param>
        /// <param name="name">New name of product.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <response code="200">Product name was updated.</response>
        /// <response code="404">The product was not found.</response>
        /// <response code="409">The product with the same name already exists.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpPut("{id}", Name = "UpdateProduct")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateNameAsync(int id, [FromBody] string name, CancellationToken cancellationToken = default)
            => Ok((await ProductManager.UpdateNameAsync(id, name, cancellationToken)).ToDTO());

        /// <summary>
        /// Finds all products.
        /// </summary>
        /// <remarks>Returns all products.</remarks>
        /// <param name="pageNumber">The number of page to return.</param>
        /// <param name="pageSize">The numbers of items to return in a single page.</param>
        /// <response code="200">Successful operation.</response>
        /// <response code="500">Internal server error occurred.</response>
        [MapToApiVersion("2.0")]
        [ApiExplorerSettings(GroupName = "v2.0")]
        [ProducesResponseType(typeof(List<ProductDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{pageNumber:int:min(1)}/{pageSize:int:range(1,5000)}", Name = "FindAllProductsPaged")]
        public async Task<IActionResult> FindAllAsync(int pageNumber = 1, int pageSize = 50)
            => Ok((await ProductManager.FindAllPagedAsync(pageNumber, pageSize)).Select(x => x.ToDTO()));
    }
}
