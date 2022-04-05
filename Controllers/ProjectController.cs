using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Data;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    public class ProjectController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public ProjectController(ApplicationDbContext Db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = Db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: ProjectController
        public ActionResult Index()
        {

            return View();
        }

        // GET: ProjectController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProjectController/AddProject
        public IActionResult AddProject()
        {
           
            return View();
        }

        // POST: ProjectController/AddProject
        [HttpPost]
        public IActionResult AddProject(string name, string content, int budget, PriorityP priority)
        {
            string userName = User.Identity.Name;

            try
            {
                ApplicationUser user = _db.Users.First(u => u.Email == userName);

                if (user != null)
                {
                    Project newProject = new Project
                    {
                        Name = name,
                        Content = content,
                        Budget = budget,
                        CompletedPercentage = 0,
                        DateBegin = DateTime.Now,
                        User = user,
                        UserId = user.Id,
                        ProjectPriority = priority
                        
                    };
                    _db.Project.Add(newProject);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            return RedirectToAction("AllProjects","Home");
        }

     
    }
}
