using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Employee_Management_System.Data;
using Smart_Employee_Management_System.Models;

namespace Smart_Employee_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult Create()
        {
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "FullName");
            ViewBag.Workers = new SelectList(_context.Workers, "Id", "FullName");

            return View();
        }

        // =========================
        // POST: Create
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Reviews.Add(review);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Customers = new SelectList(_context.Customers, "Id", "FullName", review.CustomerId);
            ViewBag.Workers = new SelectList(_context.Workers, "Id", "FullName", review.WorkerId);

            return View(review);
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