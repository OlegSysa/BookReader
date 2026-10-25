using BookReader.Core.Entities;

namespace BookReader.Core.Abstract.Repositories
{
    public interface ITranslationRepository : IRepository
    {
        public Task<bool> AddTranslationAsync(Translation translation, CancellationToken token);
        public Task<Translation?> GetAsync(string sourceLang, string targetLang, string input, CancellationToken token);
        public Task<ICollection<Translation>> GetAllAsNoTrackingAsync(CancellationToken token);
    }
}
