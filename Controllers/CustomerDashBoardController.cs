using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Employee_Management_System.Data;

namespace Smart_Employee_Management_System.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FindWorker()
        {
            var workers = _context.Workers
                                  .Include(w => w.Category)
                                  .ToList();

            return View(workers);
        }
    }
}