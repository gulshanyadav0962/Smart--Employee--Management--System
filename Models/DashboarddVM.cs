using Smart_Employee_Management_System.Models;

namespace Smart_Employee_Management_System.ViewModels
{
    public class DashboardVM
    {
        // Dashboard Cards
        public int TotalWorkers { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalBookings { get; set; }
        public int TotalPayments { get; set; }
        public int TotalReviews { get; set; }

        public decimal TotalRevenue { get; set; }

        // Today's Analytics
        public int TodayBookings { get; set; }

        public decimal TodayRevenue { get; set; }

        public string TopWorker { get; set; } = "N/A";

        public string TopCategory { get; set; } = "N/A";

        // Recent Data
        public List<Booking> RecentBookings { get; set; } = new();

        public List<Payment> RecentPayments { get; set; } = new();

        public List<Review> RecentReviews { get; set; } = new();

        // Charts
        public List<string> Months { get; set; } = new();

        public List<decimal> MonthlyRevenue { get; set; } = new();

        public List<int> MonthlyBookings { get; set; } = new();
    }
}