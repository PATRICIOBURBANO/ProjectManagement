using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProjectManagement.Models.Project> Project { get; set; }
        public DbSet<ProjectManagement.Models.Task> Task { get; set; }
        public DbSet<ProjectManagement.Models.Notification> Notification { get; set; }

    }
}