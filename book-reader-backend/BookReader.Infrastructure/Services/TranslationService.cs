using BookReader.Core.Abstract.Services;
using BookReader.Core.Business;
using BookReader.Core.DTOs.Models;
using DeepL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookReader.Infrastructure.Services
{
    public class TranslationService : BaseService<TranslationService>, ITranslationService
    {
        public TranslationService(IConfiguration config,
            ILogger<TranslationService> logger) : base(config, logger)
        {
        }
        public async Task<ServiceResult<string>> TranslateAsync(string input, CancellationToken token)
        {
            try
            {
                var apikey = _config["ApiKeys:DeplKey"]!;
                var client = new DeepLClient(apikey);

                var translatedText = await client.TranslateTextAsync(
                    input,
                    null,
                    LanguageCode.Ukrainian);
                return new ServiceResult<string>(translatedText.Text, string.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot translate input string. Error: {message}", e.Message);
                return new ServiceResult<string>(string.Empty, e.Message);
            }
        }
    }
}
