using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Business
{
    public class LearningService : BaseService<LearningService>, ILearningService
    {
        private readonly ILearningRepository _learningRepository;
        private readonly ITranslationRespository _translationRespository;
        private readonly ICacheService _cacheService;
        public LearningService(ICacheService cacheService,
            ITranslationRespository translationRespository,
            ILearningRepository learningRepository,
            IConfiguration config,
            ILogger<LearningService> logger,
            IHttpContextAccessor httpContextAccessor) : base(config, logger, httpContextAccessor)
        {   
            _cacheService = cacheService;
            _translationRespository = translationRespository;
            _learningRepository = learningRepository;
        }

        public async Task<ServiceResult<List<LearningWordModel>>> GetUserVocabulary(int userId, CancellationToken token)
        {
            var data = await _learningRepository.GetLearningWordsWithValues(userId, token);
            return new ServiceResult<List<LearningWordModel>>(data, null);
        }

        public async Task<ServiceResult<bool>> LearnWord(int userId, string word, CancellationToken token)
        {
            var sourceLang = _config["Translation:SourceLang"]!;
            var targetLang = _config["Translation:TargetLang"]!;
            var translationEntity = await _translationRespository.GetAsync(sourceLang, targetLang, word, token);
            if (translationEntity == null) 
                return new ServiceResult<bool>(false, $"Translation for word '{word}' not found.");

            var savedWords = await _learningRepository.GetLearningWords(userId, token, translationEntity.Id);
            if(!savedWords.IsNullOrEmpty())
                return new ServiceResult<bool>(false, $"This word already in learning from this user.");

            var learnWord = new LearningWord
            {
                UserId = userId,
                TranslationId = translationEntity.Id,
            };
            await _learningRepository.AddWordAsync(learnWord, token);
            return new ServiceResult<bool>(true, null);
        }
    }
}
