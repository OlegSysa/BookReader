using BookReader.Core.Abstract.Repositories;
using BookReader.Core.Abstract.Services;
using BookReader.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookReader.Core.Business
{
    public class WarmupService : BaseService<WarmupService>, IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public WarmupService(IServiceScopeFactory scopeFactory,
            IConfiguration config,
            ILogger<WarmupService>logger,
            IHttpContextAccessor httpContextAccessor) : base(config, logger, httpContextAccessor)
        {
            _scopeFactory = scopeFactory;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
            var translationRepository = scope.ServiceProvider.GetRequiredService<ITranslationRespository>();
            var allTranslations = await translationRepository.GetAllAsNoTrackingAsync(cancellationToken);
            var translationDict = allTranslations
                .ToDictionary(t => 
                t.SourceWord.BuildChacheTranslationKey(t.SourceLang, t.TargetLang),
                t => t.TranslatedWord);

           
            await cacheService.SetBatchAsync(translationDict);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
