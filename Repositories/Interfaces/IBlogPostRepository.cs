using DevHub.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DevHub.Repositories.Interfaces
{
    public interface IBlogPostRepository
    {
        IQueryable<BlogPost> GetAllActive();
        Task<BlogPost?> GetByIdAsync(int id);
        Task AddAsync(BlogPost blogPost);
        Task UpdateAsync(BlogPost blogPost);
        Task DeleteAsync(BlogPost blogPost);
    }
}
