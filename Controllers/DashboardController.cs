using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Employee_Management_System.Data;
using Smart_Employee_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{

}
namespace Smart_Employee_Management_System.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            DashboardVM vm = new DashboardVM();

            // ==========================
            // Dashboard Summary
            // ==========================

            vm.TotalWorkers = _context.Workers.Count();
            vm.TotalCustomers = _context.Customers.Count();
            vm.TotalBookings = _context.Bookings.Count();
            vm.TotalPayments = _context.Payments.Count();
            vm.TotalReviews = _context.Reviews.Count();

            vm.TotalRevenue = _context.Payments
                .Where(p => p.PaymentStatus == "Paid")
                .Sum(p => (decimal?)p.Amount) ?? 0;

            // ==========================
            // Recent Bookings
            // ==========================

            vm.RecentBookings = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Worker)
                .OrderByDescending(b => b.BookingDate)
                .Take(5)
                .ToList();

            // ==========================
            // Recent Payments
            // ==========================

            vm.RecentPayments = _context.Payments
                .Include(p => p.Booking)
                .OrderByDescending(p => p.PaymentDate)
                .Take(5)
                .ToList();

            // ==========================
            // Recent Reviews
            // ==========================

            vm.RecentReviews = _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Worker)
                .OrderByDescending(r => r.ReviewDate)
                .Take(5)
                .ToList();

            // ==========================
            // Today's Analytics
            // ==========================

            vm.TodayBookings = _context.Bookings
                .Count(b => b.BookingDate.Date == DateTime.Today);

            vm.TodayRevenue = _context.Payments
                .Where(p => p.PaymentStatus == "Paid"
                         && p.PaymentDate.Date == DateTime.Today)
                .Sum(p => (decimal?)p.Amount) ?? 0;

            // ==========================
            // Top Worker
            // ==========================

            var topWorker = _context.Bookings
                .GroupBy(b => b.WorkerId)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    WorkerId = g.Key,
                    TotalBookings = g.Count()
                })
                .FirstOrDefault();

            if (topWorker != null)
            {
                vm.TopWorker = _context.Workers
                    .Where(w => w.Id == topWorker.WorkerId)
                    .Select(w => w.FullName)
                    .FirstOrDefault() ?? "N/A";
            }

            // ==========================
            // Top Category
            // ==========================

            var topCategory = _context.Workers
                .GroupBy(w => w.CategoryId)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    CategoryId = g.Key,
                    TotalWorkers = g.Count()
                })
                .FirstOrDefault();

            if (topCategory != null)
            {
                vm.TopCategory = _context.Categories
                    .Where(c => c.Id == topCategory.CategoryId)
                    .Select(c => c.Name)
                    .FirstOrDefault() ?? "N/A";
            }

            // ==========================
            // Monthly Charts
            // ==========================

            string[] months =
            {
                "Jan","Feb","Mar","Apr","May","Jun",
                "Jul","Aug","Sep","Oct","Nov","Dec"
            };

            vm.Months = months.ToList();

            for (int i = 1; i <= 12; i++)
            {
                decimal revenue = _context.Payments
                    .Where(p => p.PaymentStatus == "Paid"
                             && p.PaymentDate.Month == i)
                    .Sum(p => (decimal?)p.Amount) ?? 0;

                int bookings = _context.Bookings
                    .Count(b => b.BookingDate.Month == i);

                vm.MonthlyRevenue.Add(revenue);
                vm.MonthlyBookings.Add(bookings);
            }

            return View(vm);
        }
    }
}