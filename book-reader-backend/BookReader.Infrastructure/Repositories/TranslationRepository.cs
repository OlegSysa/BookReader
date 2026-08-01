using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Entities;
using BookReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Infrastructure.Repositories
{
    public class TranslationRepository : BaseRepository, ITranslationRespository
    {
        public TranslationRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<bool> AddTranslationAsync(Translation translation, CancellationToken token)
        {
            _context.Translations.Add(translation);
            var res = await _context.SaveChangesAsync();
            return res > 0;
        }

        public async Task<Translation?> GetAsync(string sourceLang, string targetLang, string input)
        {
            return await _context.Translations.FirstOrDefaultAsync(t => 
                t.SourceLang == sourceLang &&
                t.TargetLang == targetLang &&
                t.SourceWord == input);
        }
    }
}
