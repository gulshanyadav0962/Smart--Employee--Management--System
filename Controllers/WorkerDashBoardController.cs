using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Employee_Management_System.Data;

namespace Smart_Employee_Management_System.Controllers
{
    [Authorize(Roles = "Worker")]
    public class WorkerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkerDashboardController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var identityUserId = _userManager.GetUserId(User);

            var worker = await _context.Workers
                .FirstOrDefaultAsync(w => w.IdentityUserId == identityUserId);

            if (worker == null)
            {
                return Content("Worker record not found.");
            }

            var bookings = await _context.Bookings
                .Include(b => b.Customer)
                .Where(b => b.WorkerId == worker.Id)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }
    
    [HttpGet]
        public async Task<IActionResult> Accept(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound();

            booking.Status = "Accepted";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Reject(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound();

            booking.Status = "Rejected";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Complete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound();

            booking.Status = "Completed";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}