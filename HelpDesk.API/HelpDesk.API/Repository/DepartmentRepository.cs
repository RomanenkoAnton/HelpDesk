using HelpDesk.API.Data;
using HelpDesk.API.Models;
using HelpDesk.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _db;
        public DepartmentRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task CreateAsync(Department department)
        {
            _db.Add(department);
            
        }

        public async Task<ICollection<Department>> GetAllAsync()
        {
           return await _db.Departments.ToListAsync();
        }

        public async Task<Department> GetAsync(int id)
        {
            return await _db.Departments.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Department> GetAsync(string departmentTitle)
        {
            return await _db.Departments.FirstOrDefaultAsync(x => x.Title.ToLower() == departmentTitle.ToLower());
        }

        public async Task RemoveAsync(Department department)
        {
            _db.Departments.Remove(department);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            _db.Departments.Update(department);
        }
    }
}
