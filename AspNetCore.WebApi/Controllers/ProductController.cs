using AspNetCore.Domain;
using AspNetCore.WebApi.DTOs;
using AspNetCore.WebApi.Extensions;
using AspNetCore.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private readonly IProductManager _productManager;

        public ProductController(IProductManager productManager)
        {
            _productManager = productManager;
        }

        /// <summary>
        /// Finds all products.
        /// </summary>
        /// <remarks>Returns all products.</remarks>
        /// <response code="200">Successful operation.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("", Name = "FindAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProductDTO>>> FindAllAsync()
            => Ok((await _productManager.FindAll().AsNoTracking().ToListAsync()).Select(x => x.ToDTO()));


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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDTO>> FindByIdAsync(int id, CancellationToken cancellationToken = default)
            => Ok((await _productManager.FindByIdAsync(id, cancellationToken)).ToDTO());

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
        [HttpPatch("{id}/name", Name = "UpdateProductName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ProductDTO>> UpdateNameAsync(int id, [FromBody][Required] string name, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok((await _productManager.UpdateNameAsync(id, name, cancellationToken)).ToDTO());
        }

        /// <summary>
        /// Updates description of product by ID.
        /// </summary>
        /// <param name="id">ID of product to update.</param>
        /// <param name="description">New description of product.</param>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <response code="200">Product description was updated.</response>
        /// <response code="404">The product was not found.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpPatch("{id}/description", Name = "UpdateProductDescription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDTO>> UpdateDescriptionAsync(int id, [FromBody] string description, CancellationToken cancellationToken = default)
            => Ok((await _productManager.UpdateDescriptionAsync(id, description, cancellationToken)).ToDTO());

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseHeader("X-Pagination-Metadata", StatusCodes.Status200OK, Type = nameof(PaginationMetadata))]
        [HttpGet("{pageNumber:int:min(1)}/{pageSize:int:range(1,5000)}", Name = "FindAllProductsPaged")]
        public async Task<ActionResult<List<ProductDTO>>> FindAllAsync(int pageNumber, int pageSize)
        {
            var products = await _productManager.FindAllPagedAsync(pageNumber, pageSize);

            var paginationMetadata = new PaginationMetadata()
            {
                TotalCount = products.TotalCount,
                TotalPages = products.TotalPages,
                PageNumber = products.PageNumber,
                PageSize = products.PageSize,
                HasNextPage = products.HasNextPage,
                HasPreviousPage = products.HasPreviousPage
            };
            Response.Headers.Add("X-Pagination-Metadata", JsonConvert.SerializeObject(paginationMetadata));

            return Ok(products.Select(x => x.ToDTO()));
        }
    }
}
