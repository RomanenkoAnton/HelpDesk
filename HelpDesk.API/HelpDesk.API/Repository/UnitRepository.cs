using HelpDesk.API.Data;
using HelpDesk.API.Models;
using HelpDesk.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Repository
{
    public class UnitRepository : IUnitRepository
    {
        private readonly ApplicationDbContext _db;

        public UnitRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Unit>> GetAllAsync()
        {
            return await _db.Units
                .Include(u => u.Category)
                .Include(u => u.Department)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Unit> GetAsync(int id)
        {
            return await _db.Units
                .Include(u => u.Category)
                .Include(u => u.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Units.AnyAsync(u => u.Id == id);
        }

        public async Task CreateAsync(Unit unit)
        {
            await _db.Units.AddAsync(unit);
        }

        public Task UpdateAsync(Unit unit)
        {
            _db.Units.Update(unit);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Unit unit)
        {
            _db.Units.Remove(unit);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}