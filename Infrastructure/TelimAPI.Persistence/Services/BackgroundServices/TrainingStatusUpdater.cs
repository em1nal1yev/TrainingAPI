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
        private readonly ITrainingRepository _repo;

        public TrainingStatusUpdater(ITrainingRepository repo)
        {
            _repo = repo;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateStatuses();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken); 
            }
        }
        private async Task UpdateStatuses()
        {
            var now = DateTime.UtcNow;

            var trainings = await _repo.GetAllAsync();

            foreach (var t in trainings)
            {
                if (t.Status == TrainingStatus.Pending && t.StartDate <= now)
                    t.Status = TrainingStatus.OnGoing;

                if (t.Status == TrainingStatus.OnGoing && t.EndDate < now)
                    t.Status = TrainingStatus.Completed;

                if ((t.Status == TrainingStatus.Draft || t.Status == TrainingStatus.Pending) && t.EndDate < now)
                    t.Status = TrainingStatus.Completed;
            }

            await _repo.SaveAsync();
        }
    }
}
