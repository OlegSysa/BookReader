using BookReader.Core.Entities;
using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Repositories
{
    public interface IOutboxMessageRepository : IRepository
    {
        Task AddAsync(OutboxMessage message, bool saveChanges = true);
        Task<IEnumerable<OutboxMessage>> GetMessagesForProcessingAsync(CancellationToken token, OutboxMessageType? type = null);
    }
}
