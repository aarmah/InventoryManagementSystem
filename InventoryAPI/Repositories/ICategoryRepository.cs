using InventoryAPI.Models;

namespace InventoryAPI.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
        Task<Category?> GetCategoryWithProductsAsync(int id);
    }
}