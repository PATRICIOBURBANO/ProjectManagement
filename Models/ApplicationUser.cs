using Microsoft.AspNetCore.Identity;

namespace ProjectManagement.Models
{
   
        public class ApplicationUser : IdentityUser
        {
            public virtual ICollection<Project> Projects { get; set; } = null!;
            public virtual ICollection<Task> Tasks { get; set; } = null!;
            public virtual ICollection<Notification> Notifications { get; set; } = null!;
            public int Reputation { get; set; } = 0;

            public ApplicationUser()
            {
                Projects = new HashSet<Project>();
                Tasks = new HashSet<Task>();
                Notifications = new HashSet<Notification>();
            }

        }


    }

