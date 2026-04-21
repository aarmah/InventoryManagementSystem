using Microsoft.EntityFrameworkCore;
using InventoryAPI.Models;
using InventoryAPI.Data;

namespace InventoryAPI.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsWithCategoryAsync()
        {
            //return await _context.Products
            //    .Include(p => p.Category)
            //    .ToListAsync();

            return await _context.Products
                .AsNoTracking() // Improves performance for read-only queries
                .Include(p => p.Category)
                .OrderBy(p => p.Id) // Add ordering
                .ToListAsync();

        }

        public async Task<Product?> GetProductWithCategoryAsync(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                 .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetProductsPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category);

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }
    }
}