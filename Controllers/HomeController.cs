using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.Models;
using System.Diagnostics;

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

        //public IActionResult NewTask(int projectId)
        //{

        //    ViewBag.ProjectId = projectId;
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult NewTask(int projectId, string title, string content)
        //{
        //    string userName = User.Identity.Name;

        //    try
        //    {
        //        ApplicationUser user = _db.Users.First(u => u.Email == userName);
        //        if (user != null)
        //        {
        //            TaskProject newTask = new TaskProject
        //            {

        //                ProjectId = projectId,
        //                Title = title,
        //                Content = content,
        //                DateBegin = DateTime.Now,
        //                DateEnd = DateTime.Now,


        //            };
        //            _db.Task.Add(newTask);
        //            _db.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(ex.Message);
        //    }

        //    return RedirectToAction("ProjectTasks", new { questionId = projectId });
        //}

        [Authorize(Roles = "Manager")]
        public IActionResult AllProjects()
        {
            var allProjects = _db.Project.ToList();

            return View(allProjects);
        }
        ////[Authorize(Roles = "Manager")]
        //public IActionResult DeleteProject(int projectId)
        //{
        //    Project projectSelected = _db.Project.First(a => a.Id == projectId);
        //    var tasks = _db.Task.ToList();
        //    var tasksRelated = _db.Task.Where(a => a.ProjectId == projectId).ToList();

        //    if (tasksRelated != null)
        //    {
        //        _db.Project.Remove(projectSelected);
        //        _db.SaveChanges();
        //    }
        //    return RedirectToAction("AllProjects");




        //    return RedirectToAction("ProjectTasks", new { projectId = projectId });
        //}

        [Authorize(Roles = "Manager")]
        public IActionResult TasksProject(int projectId)
        {
            //var tasksList = _db.Project.Where(b => b.Id == projectId).Include(c => c.Tasks).ToList();
            var project = _db.Project.Include(c => c.Tasks).First(p => p.Id == projectId);
            return View(project);
        }


        [Authorize(Roles = "Developer")]
        public IActionResult TasksProjectDev()
        {
            string userName = User.Identity.Name;
            ApplicationUser user = _db.Users.First(u => u.Email == userName);
            var tasksList = _db.Project.Include(c => c.Tasks).Where(d => d.User == user).ToList();
            return View(tasksList);

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
    }
}