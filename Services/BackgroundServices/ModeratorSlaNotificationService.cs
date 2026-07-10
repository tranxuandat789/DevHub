using DevHub.Data;
using DevHub.Helpers;
using DevHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevHub.Services.BackgroundServices
{
    public class ModeratorSlaNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ModeratorSlaNotificationService> _logger;

        public ModeratorSlaNotificationService(IServiceScopeFactory scopeFactory, ILogger<ModeratorSlaNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                // Lập lịch chạy vào 8h sáng mỗi ngày
                var nextRun = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
                if (now > nextRun)
                {
                    nextRun = nextRun.AddDays(1);
                }

                var delay = nextRun - now;
                _logger.LogInformation($"ModeratorSlaNotificationService will run next at: {nextRun}");

                await Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested)
                    break;

                await CheckOverdueTasksAsync(stoppingToken);
            }
        }

        private async Task CheckOverdueTasksAsync(CancellationToken token)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ItrecruitmentDbContext>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var emailHelper = scope.ServiceProvider.GetRequiredService<EmailHelper>();

                var overdueTime = DateTime.Now.AddDays(-1);

                // Lấy tất cả Moderator đang active
                var activeMods = await db.Admins
                    .Include(a => a.AdminNavigation)
                    .Where(a => a.AdminNavigation.IsActive == true && (a.Role == "MODERATOR" || a.Role == "ADMIN"))
                    .ToListAsync(token);

                int modsNotified = 0;

                foreach (var mod in activeMods)
                {
                    // Đếm Job Posts quá 24h
                    int overdueJobs = await db.JobPosts
                        .CountAsync(j => j.Status == "PENDING" && j.ModeratorId == mod.AdminId && j.CreatedAt < overdueTime, token);

                    // Đếm Reviews quá 24h
                    int overdueReviews = await db.ReviewCompanies
                        .CountAsync(r => r.Status == "PENDING" && r.ModeratorId == mod.AdminId && r.CreatedAt < overdueTime, token);

                    // Đếm Companies quá 24h (Do Company không có CreatedAt, ta có thể link qua Recruiter.UserAccount hoặc CompanyInvitations, 
                    // tạm thời ở đây sử dụng Recruiter's UserAccount CreatedAt như một xấp xỉ hợp lý)
                    int overdueCompanies = await db.Companies
                        .CountAsync(c => c.Status == "PENDING" && c.ModeratorId == mod.AdminId 
                                      && c.Recruiters.Any(r => r.RecruiterNavigation != null && r.RecruiterNavigation.CreatedAt < overdueTime), token);

                    int totalOverdue = overdueJobs + overdueReviews + overdueCompanies;

                    if (totalOverdue > 0)
                    {
                        // 1. In-app notification
                        await notificationService.SendNotificationAsync(
                            userId: mod.AdminId,
                            userType: "ADMIN",
                            title: "Cảnh báo Task quá hạn",
                            message: $"Bạn đang có {totalOverdue} task quá 24h chưa được xử lý, vui lòng kiểm tra.",
                            type: "SLA_WARNING",
                            severity: "warning",
                            referenceId: null,
                            referenceType: "SLA"
                        );

                        // 2. Email notification
                        if (mod.AdminNavigation != null && !string.IsNullOrEmpty(mod.AdminNavigation.Email))
                        {
                            string emailBody = $@"
                                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #eee; border-radius: 8px;'>
                                    <h2 style='color: #d9534f; text-align: center;'>Cảnh báo Task Quá Hạn</h2>
                                    <p>Chào <strong>{mod.FullName ?? mod.Username}</strong>,</p>
                                    <p>Nhắc nhở: Có <strong>{totalOverdue}</strong> yêu cầu đang chờ bạn xử lý quá hạn (hơn 24h) trên hệ thống DevHub.</p>
                                    <ul style='background-color: #f9f9f9; padding: 15px 30px; border-radius: 5px;'>
                                        <li><strong>Công ty:</strong> {overdueCompanies} yêu cầu</li>
                                        <li><strong>Tin tuyển dụng:</strong> {overdueJobs} yêu cầu</li>
                                        <li><strong>Đánh giá:</strong> {overdueReviews} yêu cầu</li>
                                    </ul>
                                    <p>Vui lòng đăng nhập vào hệ thống quản trị để kiểm tra và xử lý các yêu cầu này sớm nhất có thể.</p>
                                    <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;' />
                                    <p style='color: #888; font-size: 12px; text-align: center;'>Đây là email tự động từ hệ thống DevHub. Vui lòng không trả lời email này.</p>
                                </div>";

                            await emailHelper.SendEmailAsync(mod.AdminNavigation.Email, $"Nhắc nhở: Có {totalOverdue} yêu cầu đang chờ bạn xử lý quá hạn", emailBody);
                        }

                        modsNotified++;
                    }
                }

                _logger.LogInformation($"ModeratorSlaNotificationService completed checking. Notified {modsNotified} moderators.");
            }
            catch (Exception ex)
            {
                if (token.IsCancellationRequested) return;
                _logger.LogError(ex, "ModeratorSlaNotificationService: Error while checking for overdue tasks.");
            }
        }
    }
}
