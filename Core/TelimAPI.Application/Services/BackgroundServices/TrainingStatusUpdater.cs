using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Repositories;
using TelimAPI.Domain.Enums;

namespace TelimAPI.Persistence.Services.BackgroundServices
{
    public class TrainingStatusUpdater: BackgroundService
    {
        private readonly IServiceProvider _provider;

        public TrainingStatusUpdater(IServiceProvider provider)
        {
            _provider = provider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _provider.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<ITrainingRepository>();
                    await UpdateStatuses(repo);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
        private async Task UpdateStatuses(ITrainingRepository repo)
        {
            var now = DateTime.Now;

            var trainings = await repo.GetAllAsync();

            foreach (var t in trainings)
            {
                if (t.Status == TrainingStatus.Pending && t.StartDate <= now)
                    t.Status = TrainingStatus.OnGoing;

                if (t.Status == TrainingStatus.OnGoing && t.EndDate < now)
                    t.Status = TrainingStatus.Completed;

                if ((t.Status == TrainingStatus.Draft || t.Status == TrainingStatus.Pending) && t.EndDate < now)
                    t.Status = TrainingStatus.Completed;
            }

            await repo.SaveAsync();
        }
    }
}
