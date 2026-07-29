using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Enums
{
    public enum BookStatus
    {
        Uploading = 0,
        SavedToStorage = 1,
        ParseProcessing = 2,
        Ready = 3,
        Failed = 4
    }
}
