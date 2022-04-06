using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult DeleteProject(int projectId)
        {
            try
            {
                Project project = _db.Project.First(p => p.Id == projectId);
                if (project != null)
                {
                    _db.Project.Remove(project);
                    _db.SaveChanges();
                }
            }
            catch (Exception Ex)
            {
                return NotFound(Ex.Message);
            }
            return RedirectToAction("AllProjects", "Home");
        }

        public IActionResult AddTask(int projectId)
        {
            ViewBag.ProjectId = projectId;
            var developers = _db.Users.ToList();
            ViewBag.SelectList = new SelectList(developers, "Id", "UserName");
            return View();
        }

        [HttpPost]
        public IActionResult AddTask(string title, string content, string userId, Priority priorityValue, int projectId)
        {
            string userName = User.Identity.Name;

            try
            {
                ApplicationUser user = _db.Users.First(u => u.Email == userName);
                ApplicationUser assignedUser = _db.Users.First(u => u.Id == userId);
                Project project = _db.Project.First(p => p.Id == projectId);

                if (user != null)
                {
                    TaskProject newTask = new TaskProject
                    {
                        Title = title,
                        Content = content,
                        //Project = project,
                        //ProjectId = project.Id,
                        DateBegin = DateTime.Now,
                        Project = project,
                        ProjectId = project.Id,
                        User = assignedUser,
                        UserId = assignedUser.Id,
                        TaskPriority = priorityValue
                    };
                    _db.Task.Add(newTask);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            return RedirectToAction("TasksProject", "Home");
        }

        public IActionResult DeleteTask(int taskId)
        {
            try
            {
                TaskProject task = _db.Task.First(t => t.Id == taskId);
                if (task != null)
                {
                    _db.Task.Remove(task);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            return RedirectToAction("AllProjects", "Home");
        }

    }
}
