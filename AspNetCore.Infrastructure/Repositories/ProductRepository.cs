using AspNetCore.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _dataContext;

        public ProductRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task<Product> CreateAsync(string name, string description, string imgUri, decimal price, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var product = new Product()
            {
                Name = name,
                Description = description,
                ImgUri = imgUri,
                Price = price
            };
            _dataContext.Products.Add(product);

            return Task.FromResult(product);
        }

        public Task<Product> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _dataContext.Products.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public Task<Product> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _dataContext.Products.SingleOrDefaultAsync(x => x.Name == name, cancellationToken);
        }

        public IQueryable<Product> GetAll()
            => _dataContext.Products;
    }
}
