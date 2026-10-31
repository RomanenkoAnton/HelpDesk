using HelpDesk.API.Models;

namespace HelpDesk.API.Repository.IRepository
{
    public interface ITicketRepository
    {
        Task<List<Ticket>> GetAllAsync();
        Task<Ticket> GetAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task CreateAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task RemoveAsync(Ticket ticket);
        Task SaveAsync();
    }
}