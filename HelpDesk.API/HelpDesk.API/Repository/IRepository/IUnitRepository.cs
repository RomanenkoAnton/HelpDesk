using HelpDesk.API.Models;

namespace HelpDesk.API.Repository.IRepository
{
    public interface IUnitRepository
    {
        Task<List<Unit>> GetAllAsync();
        Task<Unit> GetAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task CreateAsync(Unit unit);
        Task UpdateAsync(Unit unit);
        Task RemoveAsync(Unit unit);
        Task SaveAsync();
    }
}
