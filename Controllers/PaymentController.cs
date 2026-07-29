using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Employee_Management_System.Data;
using Smart_Employee_Management_System.Models;

namespace Smart_Employee_Management_System.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================
        // Payment List
        // ======================
        public IActionResult Index()
        {
            var payments = _context.Payments
                                   .Include(p => p.Booking)
                                   .ToList();

            return View(payments);
        }

        // ======================
        // GET: Create
        // ======================
        [Authorize(Roles = "Customer")]
        public IActionResult Create(int bookingId)
        {
            var booking = _context.Bookings
                .FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
                return NotFound();

            Payment payment = new Payment
            {
                BookingId = booking.Id,
                Amount = booking.TotalAmount,
                PaymentStatus = "Pending"
            };

            return View(payment);
        }

        // ======================
        // POST: Create
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public IActionResult Create(Payment payment)
        {
            if (!ModelState.IsValid)
                return View(payment);

            payment.PaymentDate = DateTime.Now;
            payment.PaymentStatus = "Paid";
            payment.TransactionId = Guid.NewGuid().ToString();

            _context.Payments.Add(payment);
            _context.SaveChanges();

            TempData["Success"] = "Payment Successful.";

            return RedirectToAction("MyBookings", "Booking");
        }

        // ======================
        // GET: Edit
        // ======================
        public IActionResult Edit(int id)
        {
            var payment = _context.Payments.Find(id);

            if (payment == null)
                return NotFound();

            ViewBag.Bookings = new SelectList(_context.Bookings, "Id", "Id", payment.BookingId);

            return View(payment);
        }

        // ======================
        // POST: Edit
        // ======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Payment payment)
        {
            if (ModelState.IsValid)
            {
                _context.Payments.Update(payment);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Bookings = new SelectList(_context.Bookings, "Id", "Id", payment.BookingId);

            return View(payment);
        }

        // ======================
        // GET: Delete
        // ======================
        public IActionResult Delete(int id)
        {
            var payment = _context.Payments
                                  .Include(p => p.Booking)
                                  .FirstOrDefault(p => p.Id == id);

            if (payment == null)
                return NotFound();

            return View(payment);
        }

        // ======================
        // POST: Delete
        // ======================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var payment = _context.Payments.Find(id);

            if (payment != null)
            {
                _context.Payments.Remove(payment);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}