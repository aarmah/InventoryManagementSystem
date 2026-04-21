using InventoryAPI.Models;

namespace InventoryAPI.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithCategoryAsync();
        Task<Product?> GetProductWithCategoryAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<(IEnumerable<Product> Products, int TotalCount)> GetProductsPagedAsync(int pageNumber, int pageSize);
    }
}