using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.Models;
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
            var project = _db.Project.Include(c => c.Tasks).First(p => p.Id == projectId);
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
        public IActionResult AllProjectsDev()
        {
           
           
            var allProjects = _db.Project.Include(b => b.Tasks).Where(a =>a.UserName == User.Identity.Name).ToList();


            return View(allProjects);
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
            var project = _db.Project.Include(c => c.Tasks).First(p => p.Id == projectId);
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