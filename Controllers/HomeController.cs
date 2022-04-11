using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.Models;
using ProjectManagementPagination;
using System.Diagnostics;
using System.Security.Claims;

namespace ProjectManagement.Controllers
{


    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = context;

        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AllProjects(

            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["TasksSortParm"] = sortOrder == "Task" ? "Task_desc" : "Task";
            ViewData["BudgetSortParm"] = sortOrder == "Budget" ? "Budget_desc" : "Budget";
            ViewData["PrioritySortParm"] = sortOrder == "Priority" ? "Priority_desc" : "Priority";
            ViewData["CompletedParm"] = sortOrder == "Completed" ? "Completed_desc" : "Completed";




            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var projects = from s in _db.Project.Include(c => c.Tasks)
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                projects = _db.Project.Include(c => c.Tasks).Where(s => s.Name.Contains(searchString) || s.Content.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Date":
                    projects = projects.OrderBy(s => s.DateBegin);
                    break;
                case "date_desc":
                    projects = projects.OrderByDescending(s => s.DateBegin);
                    break;
                case "Task_desc":
                    projects = projects.OrderByDescending(s => s.Tasks.Count);
                    break;
                case "Task":
                    projects = projects.OrderBy(s => s.Tasks.Count);
                    break;
                case "Budget_desc":
                    projects = projects.OrderByDescending(s => s.Budget);
                    break;
                case "Budget":
                    projects = projects.OrderBy(s => s.Budget);
                    break;
                case "Priority_desc":
                    projects = projects.OrderByDescending(s => s.ProjectPriority);
                    break;
                case "Priority":
                    projects = projects.OrderBy(s => s.ProjectPriority);
                    break;
                case "Completed_desc":
                    projects = projects.OrderByDescending(s => s.CompletedPercentage);
                    break;
                case "Completed":
                    projects = projects.OrderBy(s => s.CompletedPercentage);
                    break;

                default:
                    projects = projects.OrderBy(s => s.DateBegin);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Project>.CreateAsync(projects.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public IActionResult Index()
        {

            return View();
        }


        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> TasksProject(string sortOrder, int projectId)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["UserSortParm"] = sortOrder == "User" ? "User_desc" : "User";
            ViewData["PrioritySortParm"] = sortOrder == "Priority" ? "Priority_desc" : "Priority";
            ViewData["CompletedParm"] = sortOrder == "Completed" ? "Completed_desc" : "Completed";



            var tasks = _db.Project.Include(c => c.Tasks).Include(p => p.Notifications).Include(p => p.User).First(p => p.Id == projectId);
            var allTasksRelated = _db.Task.Where(a => a.ProjectId == projectId);
            int sumTasks = allTasksRelated.Sum(a => a.CompletedPercentage);
            int averageCompletion = 0;
            if (allTasksRelated.Any())
            {
                averageCompletion = sumTasks / allTasksRelated.Count();
            }

            tasks.CompletedPercentage = averageCompletion;
            tasks.CreateNotification(tasks);
            _db.SaveChanges();

            var projects = from s in _db.Task.Where(c => c.ProjectId == projectId)
                           select s;



            switch (sortOrder)
            {
                case "Date":
                    projects = projects.OrderBy(s => s.DateBegin);
                    break;
                case "date_desc":
                    projects = projects.OrderByDescending(s => s.DateBegin);
                    break;
                case "User_desc":
                    projects = projects.OrderByDescending(s => s.UserName);
                    break;
                case "User":
                    projects = projects.OrderBy(s => s.UserName);
                    break;
                case "Priority_desc":
                    projects = projects.OrderByDescending(s => s.TaskPriority);
                    break;
                case "Priority":
                    projects = projects.OrderBy(s => s.TaskPriority);
                    break;
                case "Completed_desc":
                    projects = projects.OrderByDescending(s => s.CompletedPercentage);
                    break;
                case "Completed":
                    projects = projects.OrderBy(s => s.CompletedPercentage);
                    break;

                default:
                    projects = projects.OrderBy(s => s.DateBegin);
                    break;
            }
            ViewBag.Projects = projects;

            return View(tasks);
        }





        [Authorize(Roles = "Developer")]
        public IActionResult AllProjectsDev()
        {

            //string userName = User.Identity.Name;

            //Project project = _db.Project.First();
            //var projectUser = project.UserName;

            //var allProjects = _db.Project.Include(b => b.Tasks).Include(b => b.User).Where(a =>a.UserName == User.Identity.Name).ToList();

            List<Project> projectsByDev = new List<Project>();

            foreach(var project in _db.Project.Include(p => p.Tasks).ThenInclude(t => t.User))
            {
                if(project.Tasks.Any(p => p.UserName == User.Identity.Name))
                {
                    projectsByDev.Add(project);
                }
            }

            return View(projectsByDev);
        }


        [Authorize(Roles = "Developer")]
      
        public IActionResult TasksProjectDev(int projectId)
        {

            //var tasksList = _db.Project.Where(b => b.Id == projectId).Include(c => c.Tasks).ToList();
            var project = _db.Project.Include(c => c.Tasks).Include(p => p.Notifications).Include(p => p.User).First(p => p.Id == projectId);
            var allTasksRelated = _db.Task.Where(a => a.ProjectId == projectId);
            int sumTasks = allTasksRelated.Sum(a => a.CompletedPercentage);
            int averageCompletion = 0;
            if (allTasksRelated.Any())
            {
                averageCompletion = sumTasks / allTasksRelated.Count();
            }

            project.CompletedPercentage = averageCompletion;
            project.CreateNotification(project);
            _db.SaveChanges();
            return View(project);

        }
       

        public IActionResult AllUsers()
        {
            string userName = User.Identity.Name;
            ApplicationUser userFromDb = _db.Users.First(u => u.UserName == userName);

            List<ApplicationUser> users = _db.Users.ToList();

            return View(users);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult ListTasks()

        {

            var userName = User.Identity.Name;
            ViewBag.UserName = userName;
            var user = _db.ApplicationUser.First(u => u.UserName == userName);
            var userId = user.Id;
            var applicationDbContext = _db.Task.Include(a => a.User)
                                                       .Include(a => a.Project)
                                                       .Where(t => t.UserId == userId);

            var today = DateTime.Now;
            int numberOfNotice = 0;

            //foreach (var task in applicationDbContext)
            //{
            //    numberOfNotice += task.Notifications.Where(n => n.Isopen == false).Count();
            //}
            ViewBag.NumberOfNotice = numberOfNotice;
            ViewBag.UserId = userId;
            return View(applicationDbContext.ToList());
        }
    }


}