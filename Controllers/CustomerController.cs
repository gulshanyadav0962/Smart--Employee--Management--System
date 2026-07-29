using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Employee_Management_System.Data;
using Smart_Employee_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Smart_Employee_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace Smart_Employee_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

    public CustomerController(
    ApplicationDbContext context,
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult FindWorker()
        {
            var workers = _context.Workers
                                  .Include(w => w.Category)
                                  .ToList();

            return View(workers);
        }

        // Display Customer List
        public IActionResult Index()
        {
            var customers = _context.Customers.ToList();
            return View(customers);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View(new CustomerCreateVM());
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check Email Already Exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            // Create Identity User
            IdentityUser user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign Customer Role
                await _userManager.AddToRoleAsync(user, "Customer");

                // Save Customer Table
                Customer customer = new Customer
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    Address = model.Address,
                    City = model.City,
                    IdentityUserId = user.Id
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // GET: Edit
        public IActionResult Edit(int id)
        {
            var customer = _context.Customers.Find(id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Update(customer);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.Find(id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var customer = _context.Customers.Find(id);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}