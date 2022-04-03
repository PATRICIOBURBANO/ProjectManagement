using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;


namespace ProjectManagement.Models

{

    public class SeedData
    {

        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


            if (!context.Project.Any())
            {

                List<Project> newProject = new List<Project>()
            {
            new Project { Name = "My first Project",
                          Content ="This a sample of my fist prject content",
                          DateBegin = DateTime.Now,
                          DateEnd = DateTime.Now,
                          Budget = 90000,
                          },
            new Project { Name = "My second Project",
                          Content ="This a sample of my fist prject content",
                          DateBegin = DateTime.Now,
                          DateEnd = DateTime.Now,
                          Budget = 5000,
            },
            new Project {Name = "My Third Project",
                          Content ="This a sample of my fist prject content",
                          DateBegin = DateTime.Now,
                          DateEnd = DateTime.Now,
                          Budget = 10000,
            },
            };

                context.Project.AddRange(newProject);
            }

            if (!context.Roles.Any())
            {
                List<string> newRoles = new List<string>()
        {
        "Manager",
        "Developer",


        };
                foreach (string role in newRoles)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

            }

            if (!context.Users.Any())
            {
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                ApplicationUser admin1 = new ApplicationUser
                {
                    Email = "admin1@mitt.ca",
                    NormalizedEmail = "ADMIN1@MITT.CA",
                    UserName = "admin1@mitt.ca",
                    NormalizedUserName = "ADMIN1@MITT.CA",
                    EmailConfirmed = true
                };
                ApplicationUser admin2 = new ApplicationUser
                {
                    Email = "admin2@mitt.ca",
                    NormalizedEmail = "ADMIN2@MITT.CA",
                    UserName = "admin2@mitt.ca",
                    NormalizedUserName = "ADMIN2@MITT.CA",
                    EmailConfirmed = true
                };
                ApplicationUser developer1 = new ApplicationUser
                {
                    Email = "developer1@mitt.ca",
                    NormalizedEmail = "DEVELOPER1@MITT.CA",
                    UserName = "developer1@mitt.ca",
                    NormalizedUserName = "DEVELOPER1@MITT.CA",
                    EmailConfirmed = true
                };
                ApplicationUser developer2 = new ApplicationUser
                {
                    Email = "developer2@mitt.ca",
                    NormalizedEmail = "DEVELOPER2@MITT.CA",
                    UserName = "developer2@mitt.ca",
                    NormalizedUserName = "DEVELOPER2@MITT.CA",
                    EmailConfirmed = true
                };
                ApplicationUser developer3 = new ApplicationUser
                {
                    Email = "developer3@mitt.ca",
                    NormalizedEmail = "DEVELOPER3@MITT.CA",
                    UserName = "developer3@mitt.ca",
                    NormalizedUserName = "DEVELOPER3@MITT.CA",
                    EmailConfirmed = true
                };
                var hashedPassword1 = passwordHasher.HashPassword(admin1, "P@ssword1");
                var hashedPassword2 = passwordHasher.HashPassword(admin2, "P@ssword1");
                var hashedPassword3 = passwordHasher.HashPassword(developer1, "P@ssword2");
                var hashedPassword4 = passwordHasher.HashPassword(developer2, "P@ssword2");
                var hashedPassword5 = passwordHasher.HashPassword(developer3, "P@ssword2");

                admin1.PasswordHash = hashedPassword1;
                admin2.PasswordHash = hashedPassword2;
                developer1.PasswordHash = hashedPassword3;
                developer2.PasswordHash = hashedPassword4;
                developer3.PasswordHash = hashedPassword5;


                await userManager.CreateAsync(admin1);
                await userManager.CreateAsync(admin2);
                await userManager.CreateAsync(developer1);
                await userManager.CreateAsync(developer2);
                await userManager.CreateAsync(developer3);


                await userManager.AddToRoleAsync(admin1, "Manager");
                await userManager.AddToRoleAsync(admin2, "Manager");
                await userManager.AddToRoleAsync(developer1, "Developer");
                await userManager.AddToRoleAsync(developer2, "Developer");
                await userManager.AddToRoleAsync(developer3, "Developer");

            }


            context.SaveChanges();
        }
    }
}





