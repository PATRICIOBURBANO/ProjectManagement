using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    public class NotificationController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public NotificationController(ApplicationDbContext Db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = Db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: NotificationController
        public ActionResult Index()
        {
            return View();
        }

        // GET: NotificationController/Details/5
        public IActionResult Details(int projectId)
        {
            List<Notification> notification = _db.Notification.Include(n => n.Project).Include(n => n.User).Where(n => n.ProjectId == projectId).ToList();
            foreach(var noti in notification)
            {
                noti.Status = true;
            }
            _db.SaveChanges();
            return View(notification);
        }

        // GET: NotificationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NotificationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NotificationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NotificationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
