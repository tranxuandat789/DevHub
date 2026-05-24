using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DevHub.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevHub.Controllers.Recruiter
{
    [Route("recruiter/dashboard")]
    public class RecruiterDashboardController : Controller
    {
        public RecruiterDashboardController()
        {
        }

        public IActionResult Index()
        {
            // Sử dụng dữ liệu giả (mock data) để thiết kế UI mà không cần kết nối DB
            var viewModel = new RecruiterDashboard
            {
                TotalJobPosts = 12,
                TotalApplications = 145,
                TotalScheduledInterviews = 8,
                TotalCompletedInterviews = 32,
                JobPostApplicantCounts = new List<JobPostApplicantCount>
                {
                    new JobPostApplicantCount { JobId = 1, Title = "Senior UI/UX Designer", ApplicantCount = 45, Status = "Active", CreatedAt = DateTime.Now.AddDays(-2) },
                    new JobPostApplicantCount { JobId = 2, Title = "Frontend Developer (React/NextJS)", ApplicantCount = 38, Status = "Active", CreatedAt = DateTime.Now.AddDays(-5) },
                    new JobPostApplicantCount { JobId = 3, Title = "Backend Engineer (.NET Core)", ApplicantCount = 24, Status = "Closed", CreatedAt = DateTime.Now.AddDays(-15) },
                    new JobPostApplicantCount { JobId = 4, Title = "Marketing Specialist", ApplicantCount = 38, Status = "Active", CreatedAt = DateTime.Now.AddDays(-8) }
                },
                ScheduledInterviews = new List<Interview>
                {
                    new Interview { 
                        InterviewId = 1, 
                        ScheduledTime = DateTime.Now.AddDays(1).AddHours(2), 
                        Status = "Scheduled", 
                        Candidate = new DevHub.Models.Candidate { FullName = "Nguyễn Văn Hoàng" }, 
                        Application = new Application { Job = new JobPost { Title = "Senior UI/UX Designer" } }, 
                        MeetingLink = "https://meet.google.com/abc-xyz" 
                    },
                    new Interview { 
                        InterviewId = 2, 
                        ScheduledTime = DateTime.Now.AddDays(2).AddHours(5), 
                        Status = "Scheduled", 
                        Candidate = new DevHub.Models.Candidate { FullName = "Trần Thị Lan" }, 
                        Application = new Application { Job = new JobPost { Title = "Frontend Developer" } }, 
                        MeetingLink = "https://zoom.us/j/123456789" 
                    }
                },
                CompletedInterviews = new List<Interview>
                {
                    new Interview { 
                        InterviewId = 3, 
                        ScheduledTime = DateTime.Now.AddDays(-1).AddHours(-2), 
                        Status = "Completed", 
                        Candidate = new DevHub.Models.Candidate { FullName = "Lê Tuấn Anh" }, 
                        Application = new Application { Job = new JobPost { Title = "Backend Engineer" } } 
                    },
                    new Interview { 
                        InterviewId = 4, 
                        ScheduledTime = DateTime.Now.AddDays(-3).AddHours(-4), 
                        Status = "Completed", 
                        Candidate = new DevHub.Models.Candidate { FullName = "Phạm Mai Phương" }, 
                        Application = new Application { Job = new JobPost { Title = "Marketing Specialist" } } 
                    }
                }
            };

            return View("~/Views/Recruiter/RecruiterDashboard/Index.cshtml", viewModel);
        }
    }
}
