using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Smart_Employee_Management_System.Data;
using Smart_Employee_Management_System.Models;

namespace Smart_Employee_Management_System.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ReviewController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyReviews()
        {
            var identityUserId = _userManager.GetUserId(User);

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.IdentityUserId == identityUserId);

            if (customer == null)
                return NotFound();

            var reviews = await _context.Reviews
                .Include(r => r.Worker)
                .Where(r => r.CustomerId == customer.Id)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            return View(reviews);
        }

        //================================================
        // Customer  Reviews
        //================================================
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Worker)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return NotFound();

            var identityUserId = _userManager.GetUserId(User);

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.IdentityUserId == identityUserId);

            if (customer == null)
                return NotFound();

            var review = new Review
            {
                CustomerId = customer.Id,
                WorkerId = booking.WorkerId
            };

            return View(review);
        }

        // =========================
        // Review List
        // =========================
        public IActionResult Index()
        {
            var reviews = _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Worker)
                .ToList();

            return View(reviews);
        }

        // =========================
        // GET: Create
        // =========================
        //public IActionResult  AdminCreate()
        //{
        //    ViewBag.Customers = new SelectList(_context.Customers, "Id", "FullName");
        //    ViewBag.Workers = new SelectList(_context.Workers, "Id", "FullName");

        //    return View();
        //}

        // =========================
        // POST: Create
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(Review review)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return Content(string.Join(" | ", errors));
            }
            

            var identityUserId = _userManager.GetUserId(User);

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.IdentityUserId == identityUserId);

            if (customer == null)
                return NotFound();

            review.CustomerId = customer.Id;

            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.CustomerId == review.CustomerId &&
                               r.WorkerId == review.WorkerId);

            if (alreadyReviewed)
            {
                TempData["Error"] = "You have already reviewed this worker.";
                return RedirectToAction("MyBookings", "Booking");
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review submitted successfully.";

            return RedirectToAction("MyBookings", "Booking");
        }

        // =========================
        // GET: Edit
        // =========================
        public IActionResult Edit(int id)
        {
            var review = _context.Reviews.Find(id);

            if (review == null)
                return NotFound();

            ViewBag.Customers = new SelectList(_context.Customers, "Id", "FullName", review.CustomerId);
            ViewBag.Workers = new SelectList(_context.Workers, "Id", "FullName", review.WorkerId);

            return View(review);
        }

        // =========================
        // POST: Edit
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Reviews.Update(review);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Customers = new SelectList(_context.Customers, "Id", "FullName", review.CustomerId);
            ViewBag.Workers = new SelectList(_context.Workers, "Id", "FullName", review.WorkerId);

            return View(review);
        }

        // =========================
        // GET: Delete
        // =========================
        public IActionResult Delete(int id)
        {
            var review = _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Worker)
                .FirstOrDefault(r => r.Id == id);

            if (review == null)
                return NotFound();

            return View(review);
        }

        // =========================
        // POST: Delete
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var review = _context.Reviews.Find(id);

            if (review != null)
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}