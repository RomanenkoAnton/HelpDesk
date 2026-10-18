using HelpDesk.API.Models;
using Task = System.Threading.Tasks.Task;

namespace HelpDesk.API.Repository.IRepository
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetAllAsync();
        Task<Category> GetAsync(int id);
        Task<Category> GetAsync(string departmentTitle);
        Task CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task RemoveAsync(Category category);
        Task SaveAsync();
    }
}
