using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Enums
{
    public enum BookStatus
    {
        SavedToStorage = 0,
        CreatedMetadata = 1,
        ProcessingStarted = 2,
        Parsed = 3,
        Ready = 4,
        Failed = 5,
       
    }
}
