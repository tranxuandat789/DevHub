using DevHub.Models;
using DevHub.Repositories.Interfaces;
using DevHub.Services.Interfaces;

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
                // Moderators manage blogs, they need to see their Drafts (0) too
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


        public async Task<BlogPost?> GetPostByIdAsync(int id)
        {
            var blog = await _blogPostRepository.GetByIdAsync(id);
            if (blog == null)
                return null;
            return blog;
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

        public async Task<BlogPost> CreateBlogPostAsync(int publisherId, string title, string content, string? thumbnailUrl, string? tag, bool isPublished, string? authorName)
        {
            var slug = title.ToLower().Replace(" ", "-").Trim();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", ""); 
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = $"{slug}-{Guid.NewGuid().ToString().Substring(0, 8)}";

            var newBlog = new BlogPost
            {
                PublisherId = publisherId,
                Title = title,
                Content = content,
                ThumbnailUrl = thumbnailUrl,
                Tag = tag ?? "Chung",
                Slug = slug,
                AuthorName = authorName,
                IsPublished = isPublished,
                Status = isPublished ? 1 : 0,
                CreatedAt = DateTime.Now,
                PublishedAt = isPublished ? DateTime.Now : null
            };

            await _blogPostRepository.AddAsync(newBlog);
            return newBlog;
        }

        public async Task<bool> UpdateBlogPostAsync(int id, string title, string content, string? thumbnailUrl, string? tag, bool isPublished, string? authorName)
        {
            var blog = await _blogPostRepository.GetByIdAsync(id);
            if (blog == null) return false;

            blog.Title = title;
            blog.Content = content;
            if (thumbnailUrl != null)
            {
                blog.ThumbnailUrl = thumbnailUrl;
            }
            blog.Tag = tag ?? "Chung";
            blog.AuthorName = authorName;

            if (isPublished && blog.Status == 0)
            {
                blog.IsPublished = true;
                blog.Status = 1;
                blog.PublishedAt = DateTime.Now;
            }
            else if (!isPublished && blog.Status == 1)
            {
                blog.IsPublished = false;
                blog.Status = 0;
            }

            await _blogPostRepository.UpdateAsync(blog);
            return true;
        }
    }
}
