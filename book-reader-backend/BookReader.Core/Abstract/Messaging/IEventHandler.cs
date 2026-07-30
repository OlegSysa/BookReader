using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Events
{
    public interface IEventHandler<TEvent> where TEvent : IBusinessEvent
    {
        Task HandleAsync(TEvent e, CancellationToken token);
    }
}
