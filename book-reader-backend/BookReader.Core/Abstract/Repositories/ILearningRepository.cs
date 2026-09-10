using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Repositories
{
    public interface ILearningRepository : IRepository
    {
        Task AddWordAsync(LearningWord word, CancellationToken token);
        Task<List<LearningWord>> GetLearningWords(int userId, CancellationToken token, int ? translationId = null);
        Task<List<LearningWordModel>> GetLearningWordsWithValues(int userId, CancellationToken token, int ? translationId = null);
    }
}
