using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Employee_Management_System.Data;
using Smart_Employee_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Smart_Employee_Management_System.ViewModels;
using System.Security.Claims;

namespace Smart_Employee_Management_System.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public BookingController(
     ApplicationDbContext context,
     UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // Admin Booking List
        // =======================
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var bookings = _context.Bookings
                                   .Include(b => b.Worker)
                                   .Include(b => b.Customer)
                                   .ToList();

            return View(bookings);
        }

        // =======================
        // Customer Book Worker
        // =======================
        [Authorize(Roles = "Customer")]
        public IActionResult Create(int workerId)
        {
            var worker = _context.Workers.FirstOrDefault(w => w.Id == workerId);

            if (worker == null)
            {
                return NotFound();
            }

            var model = new BookingCreateVM
            {
                WorkerId = worker.Id,
                WorkerName = worker.FullName
            };

            return View(model);
        }

        // =======================
        // Save Booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(BookingCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var identityUserId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(identityUserId))
            {
                return Unauthorized();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.IdentityUserId == identityUserId);

            if (customer == null)
            {
                return Content("Customer NOT Found");
            }

            Booking booking = new Booking
            {
                CustomerId = customer.Id,
                WorkerId = model.WorkerId,
                BookingDate = DateTime.Now,
                WorkDate = model.WorkDate,
                Address = model.Address,
                Description = model.Description,
                TotalAmount = model.TotalAmount,
                Status = "Pending"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking created successfully.";

            return RedirectToAction("Index", "CustomerDashboard");
        }

        // =======================
        // Edit Booking (Admin)
        // =======================
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var booking = _context.Bookings.Find(id);

            if (booking == null)
                return NotFound();

            ViewBag.Workers = new SelectList(_context.Workers, "Id", "FullName", booking.WorkerId);
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "FullName", booking.CustomerId);

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Bookings.Update(booking);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Workers = new SelectList(_context.Workers, "Id", "FullName", booking.WorkerId);
            ViewBag.Customers = new SelectList(_context.Customers, "Id", "FullName", booking.CustomerId);

            return View(booking);
        }

        // =======================
        // Delete Booking (Admin)
        // =======================
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var booking = _context.Bookings
                                  .Include(b => b.Worker)
                                  .Include(b => b.Customer)
                                  .FirstOrDefault(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }
        //========================================
        // POst Method [Delete]
        //=======================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var booking = _context.Bookings.Find(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        // =====================================================
        // My Booking 
        //=================================================================
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyBookings()
        {
            var identityUserId = _userManager.GetUserId(User);

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.IdentityUserId == identityUserId);

            if (customer == null)
            {
                return Content("Customer record not found.");
            }

            var bookings = await _context.Bookings
                .Include(b => b.Worker)
                .Include(b => b.Payment)   // <-- Add this line
                .Where(b => b.CustomerId == customer.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

          var paidBookingIds = await _context.Payments
            .Where(p => p.PaymentStatus == "Paid")
            .Select(p => p.BookingId)
            .ToListAsync();

            ViewBag.PaidBookings = paidBookingIds;

            return View(bookings);
        }
    }
}
