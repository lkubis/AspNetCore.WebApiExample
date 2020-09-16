using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Domain
{
    public interface IProductManager
    {
        Task<Product> CreateAsync(string name, string description, string imgUri, decimal price, CancellationToken cancellationToken = default);
        Task<Product> UpdateNameAsync(int id, string name, CancellationToken cancellationToken = default);

        IQueryable<Product> FindAll();
        Task<PaginatedList<Product>> FindAllPagedAsync(int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<Product> FindByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
