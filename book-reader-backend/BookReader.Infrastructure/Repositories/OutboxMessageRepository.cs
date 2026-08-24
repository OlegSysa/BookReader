using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Entities;
using BookReader.Core.Enums;
using BookReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookReader.Infrastructure.Repositories
{
    public class OutboxMessageRepository : BaseRepository, IOutboxMessageRepository
    {
        public OutboxMessageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task AddAsync(OutboxMessage message, bool saveChanges = true)
        {
            _context.OutboxMessages.Add(message);
            if (saveChanges)
            {
                await SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<OutboxMessage>> GetMessagesForProcessingAsync(CancellationToken token, OutboxMessageType? type = null)
        {
            return await _context.OutboxMessages
                .Where(m =>(type == null || m.EventType == type) && m.ProcessedAtUtc == null && m.RetryCount < 10)
                .OrderBy(m => m.CreatedAtUtc)
                .Take(100)
                .ToListAsync(token);
        }
    }
}
