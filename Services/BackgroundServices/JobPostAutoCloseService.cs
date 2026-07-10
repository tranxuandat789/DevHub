using DevHub.Data;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.BackgroundServices
{
    // Periodically closes APPROVED job posts whose application deadline has already passed
    public class JobPostAutoCloseService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<JobPostAutoCloseService> _logger;

        // How often to scan for expired posts. Deadlines are date, just need to check every hourly tick.
        private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

        public JobPostAutoCloseService(IServiceScopeFactory scopeFactory, ILogger<JobPostAutoCloseService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        //Use periodicTimer to implement service every hour
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);
            while (true)
            {
                // Run once immediately at startup, then on every tick (1 hour).
                await CloseExpiredSafelyAsync(stoppingToken);
                try
                {
                    if (!await timer.WaitForNextTickAsync(stoppingToken)) break;
                }
                catch (OperationCanceledException)
                {
                    break; // app is shutting down
                }
            }
        }

        private async Task CloseExpiredSafelyAsync(CancellationToken token)
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.Today);

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ItrecruitmentDbContext>();

                // Update only active(approved) posts past their deadline are closed.
                var affected = await db.JobPosts
                    .Where(j => j.Deadline != null
                                && j.Deadline < today
                                && j.Status != null
                                && j.Status.ToUpper() == "APPROVED")
                    .ExecuteUpdateAsync(s => s.SetProperty(j => j.Status, "CLOSED"), token);

                if (affected > 0 && !token.IsCancellationRequested)
                    _logger.LogInformation("JobPostAutoClose: closed {Count} expired job post(s).", affected);
            }
            catch (Exception ex)
            {
                if (token.IsCancellationRequested) return; // Ignore errors (and logging) during shutdown.
                // Never let a failed run crash the background loop.
                try
                {
                    _logger.LogError(ex, "JobPostAutoClose: error while closing expired job posts.");
                }
                catch
                {
                    // Ignore if logger is already disposed
                }
            }
        }
    }
}
