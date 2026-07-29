using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Employee_Management_System.Data;
using Smart_Employee_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Smart_Employee_Management_System.ViewModels;

namespace Smart_Employee_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WorkerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public WorkerController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // Display Worker List
        public IActionResult Index()
        {
            var workers = _context.Workers
                                  .Include(w => w.Category)
                                  .ToList();

            return View(workers);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View(new WorkerCreateVM());
        }
        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkerCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                return View(model);
            }

            // Check duplicate email
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email already exists.");
                ViewBag.Categories = _context.Categories.ToList();
                return View(model);
            }

            // Create Identity User
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ViewBag.Categories = _context.Categories.ToList();
                return View(model);
            }

            // Assign Worker Role
            await _userManager.AddToRoleAsync(user, "Worker");

            // Save Worker
            Worker worker = new Worker
            {
                FullName = model.FullName,
                MobileNumber = model.MobileNumber,
                Email = model.Email,
                Address = model.Address,
                CategoryId = model.CategoryId,
                IdentityUserId = user.Id
            };

            _context.Workers.Add(worker);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public IActionResult Edit(int id)
        {
            var worker = _context.Workers.Find(id);

            if (worker == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(worker);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Worker worker)
        {
            if (ModelState.IsValid)
            {
                _context.Workers.Update(worker);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(worker);
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            var worker = _context.Workers
                                 .Include(w => w.Category)
                                 .FirstOrDefault(w => w.Id == id);

            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var worker = _context.Workers.Find(id);

            if (worker != null)
            {
                _context.Workers.Remove(worker);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}