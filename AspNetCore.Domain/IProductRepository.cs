using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Domain
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(string name, string description, string imgUri, decimal price, CancellationToken cancellationToken = default);

        IQueryable<Product> GetAll();
        Task<Product> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Product> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
