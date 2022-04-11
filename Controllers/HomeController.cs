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

        public async Task<IActionResult> Index1(

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
        public IActionResult AllProjects()
        {
            var allProjects = _db.Project.ToList();
            

            return View(allProjects);
        }

        [Authorize(Roles = "Manager")]
        public IActionResult TasksProject(int projectId)
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


        [Authorize(Roles = "Manager")]
        public IActionResult Dashboard()
        {

            var project = _db.Project.Include(c => c.Tasks);
            return View(project);
        }

        public IActionResult GroupedByProject()
        {
            var groupedItems = _db.Task.OrderBy(x => x.ProjectId).GroupBy(y => y.ProjectId);
            return View(groupedItems);
        }
        

        //public IActionResult OrderCompletionPriority()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult OrderCompletionPriority(Priority taskPriority, int? number)
        //{

        //    var grouped = from s in _db.Task
        //                  where s. == (int)taskPriority
        //                  orderby (number == 0) ? s.TaskPriority : s.CompletedPercentage
        //                  select s;
        //    var groupedBrand = grouped;
        //    if (number == 0)
        //    {
        //        groupedBrand = grouped.OrderByDescending(x => x.YearMake);
        //    }
        //    else
        //    {
        //        groupedBrand = grouped.OrderByDescending(x => x.Price);
        //    }
        //    ViewBag.Number = number;
        //    ViewBag.Priority = ;
        //    ViewBag.Option = (number == 0) ? "Year" : "Price";
        //    ViewBag.GroupedBrand = groupedBrand;

        //    return View(groupedBrand);

        //}





        [Authorize(Roles = "Developer")]
        public async Task<IActionResult> AllProjectsDev()
        {

            string userName = User.Identity.Name;

            //Project project = _db.Project.First();
            //var projectUser = project.UserName;

            //var allProjects = _db.Project.Include(b => b.Tasks).Include(b => b.User).Where(a =>a.UserName == User.Identity.Name).ToList();

            List<Project> projectsByDev = new List<Project>();

            int numNotification = 0;


            foreach(var project in _db.Project.Include(p => p.Notifications).Include(p => p.Tasks).ThenInclude(t => t.User))
            {
                if(project.Tasks.Any(p => p.UserName == User.Identity.Name))
                {
                    projectsByDev.Add(project);
                    project.CreateNotification(project);
                    numNotification += project.Notifications.Count(n => n.Status == false && (n.Task != null && n.Task.UserName == userName));
                }
            }
            await _db.SaveChangesAsync();

            ViewBag.NumNoti = numNotification;
            return View(projectsByDev);
        }
        //public IActionResult AllTasks(string? orderMethod)
        //{
        //    ViewBag.OrderOptions = new List<SelectListItem>
        //    {
        //        new SelectListItem("Order By Completed", "Completed"),
        //        new SelectListItem("Order By Priority", "Priority"),
        //    };
        //    List<Project> allTasks = _db.Project.Include(r => r.Tasks).ToList();
        //    if (orderMethod != null)
        //    {

        //        if (orderMethod == "Completed")
        //        {
        //           allTasks = allTasks.OrderBy(c => c.
        //        }
        //        else if (orderMethod == "Priority")
        //        {
        //            allTasks = _db.Project.Include(c => c.Tasks).ToList();
        //        }

        //    }

        //    return View(allTasks);
        //}


        [Authorize(Roles = "Developer")]
        public IActionResult TasksProjectDev(int projectId)
        {
            //var tasksList = _db.Project.Where(b => b.Id == projectId).Include(c => c.Tasks).ToList();
            var project = _db.Project.Include(c => c.Tasks).ThenInclude(c => c.User).First(p => p.Id == projectId);
            //project.CreateNotification(project);

            ViewBag.UserLogged = User.Identity.Name;
            return View(project);
        }

        //public IActionResult AllProjectByPriority(string taskPriority)    
        //{
        //    var projectsByTag = _db.Project.Where(a => a.Topic == priorityTag).ToList();

        //    return View(projectsByTag);
        //}



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