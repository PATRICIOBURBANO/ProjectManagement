using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult AddProject(string name, string content, int budget, PriorityP priority, double days)
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
                        DateEnd = DateTime.Now.AddDays(days),
                        User = user,
                        UserId = user.Id,
                        UserName = userName,
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

    
        public async Task<IActionResult> UpdateProject(int? id, ApplicationUser user)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _db.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProject(int id,[Bind("Id,Name,Content,Budget,ProjectPriority, DateBegin, DateEnd")] Project project)
        {

            string userName = User.Identity.Name;
            ApplicationUser user = _db.Users.First(u => u.Email == userName);
            if (id != project.Id)
            {
                return NotFound();
            }


            if (!ModelState.IsValid)
            {
                try
                {
                    project.User = user;
                    project.UserId = user.Id;
                    project.UserName = user.UserName;
                    _db.Update(project);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                   
                }
                return RedirectToAction("AllProjects","Home");
            }
            return View(project);
        }
        private bool ProjectExists(int projectId)
        {
            return _db.Project.Any(e => e.Id == projectId);
        }




        public IActionResult AddTask(int projectId)
        {
            ViewBag.ProjectId = projectId;
            var developers = _db.Users.ToList();
            ViewBag.SelectList = new SelectList(developers, "Id", "UserName");
            var allTasksRelated = _db.Task.Where(a => a.ProjectId == projectId);
            int sumTasks = allTasksRelated.Sum(a => a.CompletedPercentage);
            int averageCompletion = 0;
            Project project = _db.Project.FirstOrDefault(a => a.Id == projectId);
            if (allTasksRelated.Any())
            {
                averageCompletion = sumTasks / allTasksRelated.Count();
            }

            project.CompletedPercentage= averageCompletion;
           
            _db.SaveChanges();
            return View();
        }

        [HttpPost]
        public IActionResult AddTask(string title, string content, string userId,Priority priorityValue, int projectId, double days)
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
                        DateEnd = DateTime.Now.AddDays(days),
                        Project = project,
                        ProjectId = project.Id,
                        User = assignedUser,
                        UserId = assignedUser.Id,
                        UserName = assignedUser.UserName,
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
            // redirect to tasksProject new projectId
            return RedirectToAction("TasksProject", "Home", new { projectId = projectId });
        }

        public async Task<IActionResult> UpdateTask(int? id, ApplicationUser user)
        {
            if(id == null)
            {
                return NotFound();
            }

            var task = await _db.Task.FindAsync(id);

            if(task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTask(int id, [Bind("Id,Title,Content,DateBegin,DateEnd,TaskPriority, ProjectId")] TaskProject task)
        {
            string userName = User.Identity.Name;
            ApplicationUser user = _db.Users.First(u => u.Email == userName);
            Project project = _db.Project.First(p => p.Id == task.ProjectId);

            if (id != task.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    task.User = user;
                    task.UserId = user.Id;
                    task.UserName = user.UserName;
                    task.Project = project;
                    task.ProjectId = project.Id;
                    _db.Update(task);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("TasksProject", "Home", new { projectId = task.ProjectId });
            }
            return View(task);
        }

        private bool TaskExists(int taskId)
        {
            return _db.Task.Any(e => e.Id == taskId);
        }
        public IActionResult DeleteTask(int taskId)
        {
            TaskProject task = _db.Task.First(t => t.Id == taskId);
            try
            {
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
            //projectid
            return RedirectToAction("TasksProject", "Home", new { projectId = task.ProjectId });
        }
        
        public IActionResult TaskCompleted(int taskId, int projectId)
        {
            TaskProject taskSelected = _db.Task.First(a => a.Id == taskId);

            if (taskSelected.IsFinished == true)
            {
                taskSelected.IsFinished = false;
                _db.SaveChanges();


            }
            else if (_db.Task.Where(a => a.ProjectId == projectId).Any(b => b.IsFinished == true))
            {
                return RedirectToAction("TasksProject", new { projectId = projectId });
            }
            else
            {
                taskSelected.IsFinished = true;
                _db.SaveChanges();
            }
            return RedirectToAction("TasksProject", new { projectId = projectId });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IncreaseAdvance(int taskId, int projectId, int valuePercentage)
        {

            TaskProject taskSelected = _db.Task.First(a => a.Id == taskId);
        

            taskSelected.CompletedPercentage = valuePercentage;
            if(taskSelected.CompletedPercentage == 100)
            {
                taskSelected.IsFinished = true;
            }
        

            _db.SaveChanges();

            return RedirectToAction("TasksProjectDev","Home",new { projectId = projectId });
        }

        [HttpPost]
        public IActionResult EnterComment(int taskId, int projectId, string finalComment)
        {

            TaskProject taskSelected = _db.Task.First(a => a.Id == taskId);
        

           
            if (taskSelected.Comment != null)
            {
                taskSelected.Comment = "No final comments";
            }
            else
            {
                taskSelected.Comment = finalComment;
            }
            


            _db.SaveChanges();

            return RedirectToAction("TasksProjectDev", "Home", new { projectId = projectId });
        }


    }
}
