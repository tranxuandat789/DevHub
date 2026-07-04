using DevHub.Data;
using DevHub.Models;
using DevHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DevHub.Repositories.Implementations
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ItrecruitmentDbContext _context;

        public BlogPostRepository(ItrecruitmentDbContext context)
        {
            _context = context;
        }

        public IQueryable<BlogPost> GetAllActive()
        {
            return _context.BlogPosts
                .Include(b => b.Publisher)
                .AsQueryable();
        }

        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            return await _context.BlogPosts
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.BlogId == id);
        }

        public async Task AddAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Update(blogPost);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BlogPost blogPost)
        {
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
        }
    }
}
