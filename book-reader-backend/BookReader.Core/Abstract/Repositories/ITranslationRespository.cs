using BookReader.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Repositories
{
    public interface ITranslationRespository
    {
        public Task<bool> AddTranslationAsync(Translation translation, CancellationToken token);
        public Task<Translation?> GetAsync(string sourceLang, string targetLang, string input, CancellationToken token);
        public Task<ICollection<Translation>> GetAllAsNoTrackingAsync(CancellationToken token);
    }
}
