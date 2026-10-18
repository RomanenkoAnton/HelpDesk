using HelpDesk.API.Models;
using Task = System.Threading.Tasks.Task;

namespace HelpDesk.API.Repository.IRepository
{
    public interface IDepartmentRepository
    {
        Task<ICollection<Department>> GetAllAsync();
        Task<Department> GetAsync(int id);
        Task<Department> GetAsync(string departmentTitle);
        Task CreateAsync(Department department);
        Task UpdateAsync(Department department);
        Task RemoveAsync(Department department);
        Task SaveAsync();
    }
}
