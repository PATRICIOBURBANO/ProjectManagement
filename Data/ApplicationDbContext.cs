using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;

namespace ProjectManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProjectManagement.Models.Project> Project { get; set; }
        public DbSet<ProjectManagement.Models.TaskProject> Task { get; set; }
        public DbSet<ProjectManagement.Models.Notification> Notification { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

    }
}