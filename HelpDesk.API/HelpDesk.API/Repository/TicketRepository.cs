using HelpDesk.API.Data;
using HelpDesk.API.Models;
using HelpDesk.API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _db;

        public TicketRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _db.Tickets
                .Include(t => t.Department)
                .AsNoTracking()
                .OrderByDescending(t => t.Id)
                .ToListAsync();
        }

        public async Task<Ticket> GetAsync(int id)
        {
            return await _db.Tickets
                .Include(t => t.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Tickets.AnyAsync(t => t.Id == id);
        }

        public async Task CreateAsync(Ticket ticket)
        {
            await _db.Tickets.AddAsync(ticket);
        }

        public Task UpdateAsync(Ticket ticket)
        {
            _db.Tickets.Update(ticket);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Ticket ticket)
        {
            _db.Tickets.Remove(ticket);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}