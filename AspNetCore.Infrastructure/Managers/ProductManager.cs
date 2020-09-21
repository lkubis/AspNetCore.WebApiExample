using AspNetCore.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Infrastructure.Managers
{
    public class ProductManager : IProductManager
    {
        private readonly DataContext _dataContext;
        private readonly IProductRepository _productRepository;

        public ProductManager(DataContext dataContext, IProductRepository productRepository)
        {
            _dataContext = dataContext;
            _productRepository = productRepository;
        }

        public async Task<Product> CreateAsync(string name, string description, string imgUri, decimal price, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (string.IsNullOrWhiteSpace(imgUri))
                throw new ArgumentNullException(nameof(imgUri));

            var existingProduct = await _productRepository.FindByNameAsync(name, cancellationToken);
            if (existingProduct != null)
                throw new BusinessException(BusinessExceptions.ProductAlreadyExists, $"Product '{name}' already exists!");

            return await _productRepository.CreateAsync(name, description, imgUri, price, cancellationToken);
        }

        public IQueryable<Product> FindAll()
            => _productRepository.GetAll();

        public async Task<PaginatedList<Product>> FindAllPagedAsync(int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            var source = _productRepository.GetAll();

            var totalCount = await source.CountAsync(cancellationToken);
            var result = new PaginatedList<Product>()
            {
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            result.AddRange(source
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize));

            return result;
        }

        public async Task<Product> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.FindByIdAsync(id, cancellationToken);
            if (product is null)
                throw new BusinessException(BusinessExceptions.ProductDoesNotExist, $"Product '{id}' does not exist!");

            return product;
        }

        public async Task<Product> UpdateNameAsync(int id, string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var product = await FindByIdAsync(id, cancellationToken);

            var existingProduct = await _productRepository.FindByNameAsync(name, cancellationToken);
            if (existingProduct != null)
                throw new BusinessException(BusinessExceptions.ProductAlreadyExists, $"Product '{name}' already exists!");

            product.Name = name;

            await SaveChangesAsync(cancellationToken);
            return product;
        }

        public async Task<Product> UpdateDescriptionAsync(int id, string description, CancellationToken cancellationToken = default)
        {
            var product = await FindByIdAsync(id, cancellationToken);
            product.Description = description;

            await SaveChangesAsync(cancellationToken);
            return product;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _dataContext.SaveChangesAsync(cancellationToken);
    }
}
