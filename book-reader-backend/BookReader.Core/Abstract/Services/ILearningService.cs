using BookReader.Core.DTOs.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Abstract.Services
{
    public interface ILearningService
    {
        Task<ServiceResult<bool>> LearnWord(int userId, string word, CancellationToken token);
        Task<ServiceResult<List<LearningWordModel>>> GetUserVocabulary(int userId, CancellationToken token);
    }
}
