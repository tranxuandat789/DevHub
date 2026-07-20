using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DevHub.Data;

var options = new DbContextOptionsBuilder<ItrecruitmentDbContext>()
    .UseSqlServer("Server=localhost;Database=IT_Recruitment_DevHub;Trusted_Connection=True;TrustServerCertificate=True;")
    .Options;

using (var context = new ItrecruitmentDbContext(options))
{
    var interview = context.Interviews
        .Include(i => i.Application).ThenInclude(a => a.Job)
        .Include(i => i.Candidate).ThenInclude(c => c.CandidateNavigation)
        .FirstOrDefault(i => i.InterviewId == 26);

    if (interview != null)
    {
        Console.WriteLine("Found interview! Status: " + interview.Status);
        
        try {
            interview.Status = "confirmed";
            interview.UpdatedAt = DateTime.Now;
            context.SaveChanges();
            Console.WriteLine("Interview saved!");
            
            var recruiter = context.Recruiters
                .Include(r => r.RecruiterNavigation)
                .FirstOrDefault(r => r.RecruiterId == interview.RecruiterId);
                
            var n = new DevHub.Models.Notification
            {
                UserId = recruiter.RecruiterNavigation.UserId,
                UserType = "RECRUITER",
                Type = "INTERVIEW",
                Title = "?ng vien xac nh?n tham gia ph?ng v?n",
                Message = $"?ng vien {interview.Candidate?.FullName} ?a xac nh?n tham gia bu?i ph?ng v?n cho v? tri {interview.Application?.Job?.Title}.",
                ReferenceType = "Interview",
                ReferenceId = interview.InterviewId,
                SeverityLevel = "info",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            context.Notifications.Add(n);
            context.SaveChanges();
            Console.WriteLine("Notification saved!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex.Message);
            if (ex.InnerException != null) {
                Console.WriteLine("INNER: " + ex.InnerException.Message);
            }
        }
    }
}
