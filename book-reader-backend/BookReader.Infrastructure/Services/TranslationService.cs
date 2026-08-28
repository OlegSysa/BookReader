using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.DTOs.Models;
using BookReader.Core.Entities;
using BookReader.Core.Extensions;
using DeepL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BookReader.Infrastructure.Services
{
    public class TranslationService : BaseService<TranslationService>, ITranslationService
    {
        private readonly ICacheService _cacheService;
        private readonly ITranslationRespository _translationRespository;
        public TranslationService(ICacheService cacheService,
            ITranslationRespository translationRespository,
            IConfiguration config,
            ILogger<TranslationService> logger,
            IHttpContextAccessor httpContextAccessor) : base(config, logger, httpContextAccessor)
        {
            _cacheService = cacheService;
            _translationRespository = translationRespository;
        }
        public async Task<ServiceResult<string>> TranslateAsync(string input, CancellationToken token)
        {
            try
            {
                var sourceLang = _config["Translation:SourceLang"]!;
                var targetLang = _config["Translation:TargetLang"]!;
                var translationKey = input.BuildChacheTranslationKey(sourceLang, targetLang);
                var cachedTranslation = await _cacheService.GetAsync<string>(translationKey);
                if (!string.IsNullOrEmpty(cachedTranslation))
                    return new ServiceResult<string>(cachedTranslation, string.Empty);

                var result = await ExecuteTraslation(input, token);
                var translationEntity = await _translationRespository.GetAsync(sourceLang, targetLang, input, token);
                if (translationEntity == null)
                {
                    translationEntity = new Translation()
                        {
                            SourceLang = sourceLang,
                            TargetLang = targetLang,
                            SourceWord = input,
                            TranslatedWord = result
                        };
                    await _translationRespository.AddTranslationAsync(translationEntity, token);
                }
                
                await _cacheService.SetAsync<string>(translationKey, result);
                return new ServiceResult<string>(result, string.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot translate input string. Error: {message}", e.Message);
                return new ServiceResult<string>(string.Empty, e.Message);
            }
        }

        public async Task<ServiceResult<string>> TranslateSentenceAsync(int sentenceId, string value, CancellationToken token)
        {
            try
            {
               
                //var translationKey = input.BuildChacheKey(sourceLang, targetLang);
                //var cachedTranslation = await _cacheService.GetAsync<string>(translationKey);
                //if (!string.IsNullOrEmpty(cachedTranslation))
                    //return new ServiceResult<string>(cachedTranslation, string.Empty);

                var result = await ExecuteTraslation(value, token);
                //var translationEntity = await _translationRespository.GetAsync(sourceLang, targetLang, input, token);
                //if (translationEntity == null)
                //{
                //    translationEntity = new Translation()
                //    {
                //        SourceLang = sourceLang,
                //        TargetLang = targetLang,
                //        SourceWord = input,
                //        TranslatedWord = result
                //    };
                //    await _translationRespository.AddTranslationAsync(translationEntity, token);
                //}

               // await _cacheService.SetAsync<string>(translationKey, result);
                return new ServiceResult<string>(result, string.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot translate input string. Error: {message}", e.Message);
                return new ServiceResult<string>(string.Empty, e.Message);
            }
        }

        private async Task<string> ExecuteTraslation(string input, CancellationToken token)
        {
            var apikey = _config["ApiKeys:DeplKey"]!;
            var client = new DeepLClient(apikey);

            var translatedText = await client.TranslateTextAsync(
                input,
                null,
                LanguageCode.Ukrainian, cancellationToken: token);
            return translatedText.Text;
        }
    }
}
