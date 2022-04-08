using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagement.Data;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{

    [Authorize(Roles = "Manager")]
    public class UserManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagerController(ApplicationDbContext context,UserManager<ApplicationUser> um,RoleManager<IdentityRole> rm)
        {
            _context = context;
            
            _userManager = um;
            
            _roleManager = rm;
        }


        
        public async Task<IActionResult> Index()
        {
            try
            {
                var userAndRoles = new Dictionary<ApplicationUser, IList<string>>();
                var users = _context.Users;
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userAndRoles.Add(user, roles);
                }
                return View(userAndRoles);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        public async Task<IActionResult> AssignRole(string userId)
        {
            try
            {
                var user = _context.ApplicationUser.First(u => u.Id == userId);
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _context.Roles.ToList();
                ViewBag.AllRoles = allRoles;
                var oldRoles = allRoles.Where(r => !userRoles.Contains(r.ToString())).ToList();

                ViewBag.User = _context.ApplicationUser.First(u => u.Id == userId);
                ViewBag.UserRoles = await _userManager.GetRolesAsync(user);
                var RolesList = new SelectList(oldRoles, "Name", "Name");
                return View(RolesList);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(string role, string userId)
        {
            try
            {
                var user = _context.Users.First(u => u.Id == userId);
                await _userManager.AddToRoleAsync(user, role);
                await _userManager.UpdateAsync(user);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CreateRole()
        {
            try
            {
                var allroles = _context.Roles.ToList();
                return View(allroles);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(string name)
        {
            try
            {
                await _roleManager.RoleExistsAsync(name);
                await _roleManager.CreateAsync(new IdentityRole(name));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return RedirectToAction("CreateRole");
        }

        public async Task<IActionResult> DeleteRole()
        {
            try
            {
                ViewBag.AllRoles = _context.Roles.ToList();
                var allroles = _context.Roles;
                var roleList = new SelectList(allroles, "Name", "Name");
                return View(roleList);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string role)
        {
            try
            {
                var roleToDelete = _context.Roles.First(r => r.Name == role);
                _context.Roles.Remove(roleToDelete);
                _context.SaveChanges();
                return RedirectToAction("DeleteRole");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> DeleteRoleUser(string userid)
        {
            try
            {
                var user = _context.ApplicationUser.First(u => u.Id == userid);
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _context.Roles.ToList();


                ViewBag.User = _context.ApplicationUser.First(u => u.Id == userid);
                ViewBag.UserRoles = await _userManager.GetRolesAsync(user);
                var selectlistOfRoles = new SelectList(userRoles);
                return View(selectlistOfRoles);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRoleUser(string role, string userid)
        {
            try
            {
                //var user = _context.Users.First(u => u.Id == userid);
                var roleId = _context.Roles.First(r => r.Name == role).Id;
                var userRole = _context.UserRoles.First(ur => ur.UserId == userid && ur.RoleId == roleId);
                _context.UserRoles.Remove(userRole);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> DeleteUser()
        {
            try
            {
                ViewBag.AllUsers = _context.ApplicationUser;
                var Alluser = _context.ApplicationUser;
                var userList = new SelectList(Alluser, "Email", "Email");
                return View(userList);
                var users = _context.ApplicationUser;
                var users1 = _context.ApplicationUser.ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(string username)
        {
            try
            {
                var user = _context.ApplicationUser.First(u => u.Email == username);
                var tasksOfUser = _context.Task.Where(t => t.UserId == user.Id).ToList();
                var notification = _context.Notification.Where(t => t.UserName == user.UserName).ToList();
                foreach (var n in notification)
                {
                    n.Id = default;
                }
                foreach (var task in tasksOfUser)
                {
                    task.UserId = null;
                }
                _context.ApplicationUser.Remove(user);
                _context.SaveChanges();
                return RedirectToAction("DeleteUser");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

       


        [HttpPost]
        public IActionResult Payment(string id, int amount)
        {
            var user = _context.ApplicationUser.First(u => u.Id == id);
            user.PayDay = amount;
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
            
        }



        [Authorize(Roles = "Admin")]
        public IActionResult CreateRoles()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoles(string newRoleName)
        {
            await _roleManager.CreateAsync(new IdentityRole(newRoleName));

            _context.SaveChanges();
            string currentUserName = User.Identity.Name;
            var getuserTask = _userManager.FindByNameAsync(currentUserName);
            ApplicationUser awaitedUser = await getuserTask;
            if (await _roleManager.RoleExistsAsync(newRoleName))
            {
                if (!await _userManager.IsInRoleAsync(awaitedUser, newRoleName))
                {
                    await _userManager.AddToRoleAsync(awaitedUser, newRoleName);
                    _context.SaveChanges();
                }
            }

            return View();
        }

    }
}
