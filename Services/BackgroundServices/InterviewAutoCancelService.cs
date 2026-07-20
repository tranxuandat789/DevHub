using DevHub.Data;
using Microsoft.EntityFrameworkCore;
using DevHub.Models;

namespace DevHub.Services.BackgroundServices
{
    // Auto-cancels scheduled interviews that are not confirmed by the candidate 24h prior to the interview time.
    public class InterviewAutoCancelService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<InterviewAutoCancelService> _logger;

        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);

        public InterviewAutoCancelService(IServiceScopeFactory scopeFactory, ILogger<InterviewAutoCancelService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);
            while (true)
            {
                await CancelUnconfirmedInterviewsAsync(stoppingToken);
                try
                {
                    if (!await timer.WaitForNextTickAsync(stoppingToken)) break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task CancelUnconfirmedInterviewsAsync(CancellationToken token)
        {
            try
            {
                var threshold = DateTime.Now.AddHours(24);

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ItrecruitmentDbContext>();

                // Get interviews that are scheduled, not yet confirmed, and starting in less than 24 hours.
                var interviewsToCancel = await db.Interviews
                    .Include(i => i.Candidate)
                    .Include(i => i.Application).ThenInclude(a => a.Job)
                    .Where(i => i.Status == "scheduled" && i.ScheduledTime <= threshold)
                    .ToListAsync(token);

                if (!interviewsToCancel.Any()) return;

                foreach (var interview in interviewsToCancel)
                {
                    interview.Status = "cancelled";
                    interview.Notes = string.IsNullOrEmpty(interview.Notes)
                        ? "Hệ thống tự động hủy: Ứng viên không xác nhận trước 24h"
                        : $"{interview.Notes}\nHệ thống tự động hủy: Ứng viên không xác nhận trước 24h";
                    interview.UpdatedAt = DateTime.Now;

                    // Fetch Recruiter manually since Interview doesn't have a navigation property
                    var recruiter = await db.Recruiters
                        .Include(r => r.RecruiterNavigation)
                        .FirstOrDefaultAsync(r => r.RecruiterId == interview.RecruiterId, token);

                    // Notify Recruiter
                    if (recruiter?.RecruiterNavigation != null)
                    {
                        var n = new Notification
                        {
                            UserId = recruiter.RecruiterNavigation.UserId,
                            UserType = "RECRUITER",
                            Type = "INTERVIEW",
                            Title = "Lịch phỏng vấn bị hủy tự động",
                            Message = $"Hệ thống đã tự động hủy phỏng vấn cho {interview.Candidate?.FullName} (vị trí {interview.Application?.Job?.Title}) do không xác nhận tham gia trước 24h.",
                            ReferenceType = "Interview",
                            ReferenceId = interview.InterviewId,
                            SeverityLevel = "warning",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        };
                        
                        if (n.Message.Length > 500)
                            n.Message = n.Message.Substring(0, 497) + "...";

                        db.Notifications.Add(n);
                    }
                    
                    // Notify Candidate
                    if (interview.Candidate != null)
                    {
                        var n = new Notification
                        {
                            UserId = interview.CandidateId, // Note: CandidateId is the UserId
                            UserType = "CANDIDATE",
                            Type = "INTERVIEW",
                            Title = "Lịch phỏng vấn bị hủy",
                            Message = $"Lịch phỏng vấn vị trí {interview.Application?.Job?.Title} đã bị hủy do bạn không xác nhận tham gia trước 24h.",
                            ReferenceType = "Interview",
                            ReferenceId = interview.InterviewId,
                            SeverityLevel = "warning",
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        };
                        
                        if (n.Message.Length > 500)
                            n.Message = n.Message.Substring(0, 497) + "...";

                        db.Notifications.Add(n);
                    }
                }

                await db.SaveChangesAsync(token);
                _logger.LogInformation("InterviewAutoCancel: automatically cancelled {Count} unconfirmed interviews.", interviewsToCancel.Count);
            }
            catch (Exception ex)
            {
                if (token.IsCancellationRequested) return;
                try
                {
                    _logger.LogError(ex, "InterviewAutoCancel: error while cancelling unconfirmed interviews.");
                }
                catch {}
            }
        }
    }
}
