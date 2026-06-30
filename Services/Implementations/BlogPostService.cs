using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;
using DevHub.ViewModels.Recruiter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevHub.Services.Implementations
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IConfiguration _config;

        public BlogPostService(IBlogPostRepository blogPostRepository, IConfiguration config)
        {
            _blogPostRepository = blogPostRepository;
            _config = config;
        }

        public async Task<(IEnumerable<BlogPost> Blogs, int TotalPages, int TotalItems)> GetFilteredBlogsAsync(string keyword, string dateFrom, string status, string sortBy, int page, int pageSize, int? authorId = null, bool forModerator = false)
        {
            var query = _blogPostRepository.GetAllActive();

            if (authorId.HasValue)
            {
                query = query.Where(b => b.AuthorId == authorId.Value);
            }
            
            if (forModerator)
            {
                // Moderators should only see Pending (3), Approved/Published (1), Rejected (4), Hidden (2)
                // Assuming they don't see Drafts (0) from Recruiters
                query = query.Where(b => b.Status != 0);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                string searchTrimmed = keyword.Trim();
                query = query.Where(b => b.Title.Contains(searchTrimmed));
            }

            if (!string.IsNullOrEmpty(dateFrom) && DateTime.TryParse(dateFrom, out DateTime parsedDate))
            {
                query = query.Where(b => b.CreatedAt != null && b.CreatedAt.Value.Date == parsedDate.Date);
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (int.TryParse(status, out int parsedStatus))
                {
                    if (parsedStatus == 2)
                    {
                        query = query.Where(b => b.Status == 2 || b.Status == 5);
                    }
                    else
                    {
                        query = query.Where(b => b.Status == parsedStatus);
                    }
                }
            }

            if (sortBy == "oldest")
            {
                query = query.OrderBy(b => b.CreatedAt);
            }
            else
            {
                query = query.OrderByDescending(b => b.CreatedAt);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            var blogs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BlogPost
                {
                    BlogId       = b.BlogId,
                    Title        = b.Title,
                    ThumbnailUrl = b.ThumbnailUrl,
                    Author       = b.Author,
                    AuthorId     = b.AuthorId,
                    Status       = b.Status,
                    CreatedAt    = b.CreatedAt,
                    AuthorRecruiter = b.AuthorRecruiter
                })
                .ToListAsync();

            return (blogs, totalPages, totalItems);
        }

        public async Task CreatePostAsync(BlogPostCreateViewModel model, int? publisherId, int? authorId)
        {
            var blog = new BlogPost
            {
                Title = model.Title,
                Content = model.Content,
                ThumbnailUrl = model.ThumbnailUrl,
                PublisherId = publisherId,
                AuthorId = authorId,
                Author = model.Author,
                Tags = model.Tags,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                Slug = Regex.Replace(model.Title.ToLower(), @"[^a-z0-9\s-]", "").Replace(" ", "-") + "-" + DateTime.Now.Ticks
            };

            if (model.actionType == "draft")
            {
                blog.Status = 0; // Draft
            }
            else
            {
                // If created by recruiter without publisher, it's pending (3).
                // If created by moderator, it's published (1).
                if (publisherId.HasValue)
                {
                    blog.Status = 1; // Published
                    blog.PublishedAt = DateTime.Now;
                }
                else
                {
                    blog.Status = 3; // Pending Approval
                }
            }

            await _blogPostRepository.AddAsync(blog);
        }

        public async Task<BlogPost?> GetPostByIdAsync(int id)
        {
            var blog = await _blogPostRepository.GetByIdAsync(id);
            if (blog == null || blog.IsDeleted == true)
                return null;
            return blog;
        }

        public async Task<bool> EditPostAsync(int id, BlogPostEditViewModel model)
        {
            var blog = await _blogPostRepository.GetByIdAsync(id);
            if (blog == null || blog.IsDeleted == true)
                return false;

            blog.Title = model.Title;
            blog.Content = model.Content;
            blog.ThumbnailUrl = model.ThumbnailUrl;
            blog.Author = model.Author;
            blog.Tags = model.Tags;
            
            blog.Slug = Regex.Replace(model.Title.ToLower(), @"[^a-z0-9\s-]", "").Replace(" ", "-") + "-" + DateTime.Now.Ticks;

            if (model.actionType == "draft")
            {
                blog.Status = 0; // Draft
            }
            else
            {
                // If the user editing is a recruiter (publisherId not involved here, but generally if they edit, it should become pending if it wasn't already published? Or maybe they just can't edit published without re-approval? Let's say if action is publish, it goes to Pending Approval unless they are Moderator)
                // Wait, we need to know who is editing. Let's just say if they click publish:
                if (blog.PublisherId != null) 
                {
                    blog.Status = 1; // Published
                    if (blog.PublishedAt == null)
                    {
                        blog.PublishedAt = DateTime.Now;
                    }
                } 
                else 
                {
                    blog.Status = 3; // Pending Approval
                }
            }

            await _blogPostRepository.UpdateAsync(blog);
            return true;
        }

        public async Task<bool> ToggleVisibilityAsync(int id)
        {
            var blog = await _blogPostRepository.GetByIdAsync(id);
            if (blog == null || blog.IsDeleted == true) return false;

            if (blog.Status == 1)
            {
                blog.Status = 2; // Hide
            }
            else
            {
                blog.Status = 1; // Publish
                blog.PublishedAt = DateTime.Now;
            }

            await _blogPostRepository.UpdateAsync(blog);
            return true;
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var blog = await _blogPostRepository.GetByIdAsync(id);
            if (blog == null || blog.IsDeleted == true) return false;

            blog.IsDeleted = true;
            await _blogPostRepository.UpdateAsync(blog);
            return true;
        }

        public async Task<bool> ApprovePostAsync(int id, int approverId)
        {
            var blog = await _blogPostRepository.GetByIdAsync(id);
            if (blog == null || blog.IsDeleted == true) return false;

            blog.Status = 1; // Published
            blog.ApproverId = approverId;
            blog.ApprovedAt = DateTime.Now;
            if (blog.PublishedAt == null)
            {
                blog.PublishedAt = DateTime.Now;
            }

            await _blogPostRepository.UpdateAsync(blog);

            // Gửi email thông báo cho recruiter
            if (blog.AuthorRecruiter?.RecruiterNavigation?.Email != null)
            {
                var emailHelper = new DevHub.Helpers.EmailHelper(_config);
                string subject = "DevHub - Bài viết của bạn đã được duyệt";
                string content = $@"
                    <p>Chào <b>{blog.Author ?? blog.AuthorRecruiter.RecruiterNavigation.Email}</b>,</p>
                    <p>Bài viết <b>{blog.Title}</b> của bạn đã được moderator duyệt và xuất bản thành công trên nền tảng DevHub.</p>
                    <p style='margin-top:20px;'>
                        <a href='https://devhub.vn/Blog/{blog.Slug}'
                           style='background:#4640DE;color:#fff;padding:10px 20px;border-radius:6px;text-decoration:none;font-weight:bold;'>
                            Xem bài viết
                        </a>
                    </p>";
                string body = DevHub.Helpers.EmailHelper.GetBaseTemplate("🎉 Bài viết đã được duyệt!", content);
                try
                {
                    await emailHelper.SendEmailAsync(blog.AuthorRecruiter.RecruiterNavigation.Email, subject, body);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error sending approval email: " + ex.Message);
                }
            }

            return true;
        }

        public async Task<bool> RejectPostAsync(int id, int approverId, string reason)
        {
            var blog = await _blogPostRepository.GetByIdAsync(id);
            if (blog == null || blog.IsDeleted == true) return false;

            blog.Status = 4; // Rejected
            blog.ApproverId = approverId;
            blog.RejectedAt = DateTime.Now;
            blog.RejectReason = reason;

            await _blogPostRepository.UpdateAsync(blog);

            if (blog.AuthorRecruiter?.RecruiterNavigation?.Email != null)
            {
                var emailHelper = new DevHub.Helpers.EmailHelper(_config);
                string subject = "DevHub - Thông báo từ chối bài viết blog";
                string content = $@"
                    <p>Chào bạn,</p>
                    <p>Bài viết <b>{blog.Title}</b> của bạn đã bị từ chối với lý do:</p>
                    <div style='background:#fff3cd;border-left:4px solid #ffc107;padding:12px 16px;border-radius:4px;margin:16px 0;'>
                        <p style='margin:0;color:#856404;'><i>{reason}</i></p>
                    </div>
                    <p>Vui lòng đăng nhập vào hệ thống để chỉnh sửa và gửi lại bài viết.</p>";
                string body = DevHub.Helpers.EmailHelper.GetBaseTemplate("Thông báo từ chối bài viết", content);
                try 
                {
                    await emailHelper.SendEmailAsync(blog.AuthorRecruiter.RecruiterNavigation.Email, subject, body);
                } 
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error sending rejection email: " + ex.Message);
                }
            }

            return true;
        }
    }
}
