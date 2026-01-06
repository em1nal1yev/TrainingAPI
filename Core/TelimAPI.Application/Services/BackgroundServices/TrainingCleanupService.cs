//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TelimAPI.Application.Repositories;

//namespace TelimAPI.Persistence.Services.BackgroundServices
//{
//    public class TrainingCleanupService : BackgroundService
//    {
//        private readonly ILogger<TrainingCleanupService> _logger;
//        private readonly IServiceProvider _serviceProvider;
//        private readonly TimeSpan _period = TimeSpan.FromMinutes(10);

//        public TrainingCleanupService(ILogger<TrainingCleanupService> logger, IServiceProvider serviceProvider)
//        {
//            _logger = logger;
//            _serviceProvider = serviceProvider;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            using var timer = new PeriodicTimer(_period);

//            while (!stoppingToken.IsCancellationRequested)
//            {
//                await DoWorkAsync();

//                try
//                {
//                    await timer.WaitForNextTickAsync(stoppingToken);
//                }
//                catch (OperationCanceledException)
//                {
//                    break;
//                }
//            }
//        }
//        private async Task DoWorkAsync()
//        {
            
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var trainingRepository = scope.ServiceProvider.GetRequiredService<ITrainingRepository>();

//                try
//                {
//                    var cleanupTime = DateTime.UtcNow.AddHours(-24);
//                    var allTrainings = await trainingRepository.GetAllAsync();

//                    var trainingsToDelete = allTrainings
//                        .Where(t => t.Date < cleanupTime) 
//                        .ToList();

//                    foreach (var training in trainingsToDelete)
//                    {
//                        await trainingRepository.DeleteAsync(training.Id);
//                        _logger.LogInformation($"Training deleted: {training.Id}");
//                    }
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error during Training Cleanup.");
//                }
//            }
//        }
//    }
//}
