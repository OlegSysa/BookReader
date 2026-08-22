using BookReader.Core.Abstract.Events;
using BookReader.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Events
{
   public sealed record BookNotificationEvent(int UserId, int BookId, BookStatus Status, string? Message = null) : IBusinessEvent;
}
