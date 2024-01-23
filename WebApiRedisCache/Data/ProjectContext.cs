
using Microsoft.EntityFrameworkCore;
using WebApiRedisCache.Models;

namespace WebApiRedisCache.Data
{
    public class ProjectContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {
            
        }
    }
}
