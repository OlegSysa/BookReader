using BookReader.Core.DTOs.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Services
{
    public interface ITranslationService
    {
        Task<ServiceResult<string>> TranslateAsync(string input, CancellationToken token);
        Task<ServiceResult<string>> TranslateSentenceAsync(int sentenceId, string value, CancellationToken token);
    }
}
