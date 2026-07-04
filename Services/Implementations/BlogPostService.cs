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

        public async Task<(IEnumerable<BlogPost> Blogs, int TotalPages, int TotalItems)> GetFilteredBlogsAsync(string keyword, string dateFrom, string status, string sortBy, int page, int pageSize, bool forModerator = false)
        {
            var query = _blogPostRepository.GetAllActive();

            if (forModerator)
            {
                // Moderators should only see Pending (3), Approved/Published (1), Rejected (4), Hidden (2)
                // Assuming they don't see Drafts (0) from Recruiters
                query = query.Where(b => b.Status != 0);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                string searchTrimmed = keyword.Trim();
                query = query.Where(b => b.Title != null && b.Title.Contains(searchTrimmed));
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
                    Status       = b.Status,
                    CreatedAt    = b.CreatedAt,
                    Publisher    = b.Publisher
                })
                .ToListAsync();

            return (blogs, totalPages, totalItems);
        }

        public async Task CreatePostAsync(BlogPostCreateViewModel model, int? publisherId)
        {
            var blog = new BlogPost
            {
                Title = model.Title,
                Content = model.Content,
                ThumbnailUrl = model.ThumbnailUrl,
                PublisherId = publisherId,
                Tag = model.Tags ?? "",
                CreatedAt = DateTime.Now,
                Slug = Regex.Replace(model.Title.ToLower(), @"[^a-z0-9\s-]", "").Replace(" ", "-") + "-" + DateTime.Now.Ticks
            };

            if (model.actionType == "draft")
            {
                blog.Status = 0; // Draft
            }
            else
            {
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
            if (blog == null)
                return null;
            return blog;
        }

        public async Task<bool> EditPostAsync(int id, BlogPostEditViewModel model)
        {
            var blog = await _blogPostRepository.GetByIdAsync(id);
            if (blog == null)
                return false;

            blog.Title = model.Title;
            blog.Content = model.Content;
            blog.ThumbnailUrl = model.ThumbnailUrl;
            blog.Tag = model.Tags ?? "";
            
            blog.Slug = Regex.Replace(model.Title.ToLower(), @"[^a-z0-9\s-]", "").Replace(" ", "-") + "-" + DateTime.Now.Ticks;

            if (model.actionType == "draft")
            {
                blog.Status = 0; // Draft
            }
            else
            {
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
            if (blog == null) return false;

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
            if (blog == null) return false;

            await _blogPostRepository.DeleteAsync(blog);
            return true;
        }
    }
}
