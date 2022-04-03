﻿using Microsoft.AspNetCore.Identity;

namespace ProjectManagement.Models
{
   
        public class ApplicationUser : IdentityUser
        {
            public virtual ICollection<Project> Projects { get; set; } = null!;

            public virtual ICollection<TaskProject> Tasks { get; set; } = null!;

            public virtual ICollection<Notification> Notifications { get; set; } = null!;

      

            public ApplicationUser()
            {
                Projects = new HashSet<Project>();

                Tasks = new HashSet<TaskProject>();

                Notifications = new HashSet<Notification>();
            }

        }


    }

