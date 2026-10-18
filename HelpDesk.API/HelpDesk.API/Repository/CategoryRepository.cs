using HelpDesk.API.Data;
using HelpDesk.API.Models;
using HelpDesk.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task CreateAsync(Category category)
        {
            _db.Add(category);
            
        }

        public async Task<ICollection<Category>> GetAllAsync()
        {
           return await _db.Categories.ToListAsync();
        }

        public async Task<Category> GetAsync(int id)
        {
            return await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Category> GetAsync(string categoryTitle)
        {
            return await _db.Categories.FirstOrDefaultAsync(x => x.Title.ToLower() == categoryTitle.ToLower());
        }

        public async Task RemoveAsync(Category category)
        {
            _db.Categories.Remove(category);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _db.Categories.Update(category);
        }
    }
}
