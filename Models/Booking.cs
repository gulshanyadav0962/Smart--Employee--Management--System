using System;

namespace Smart_Employee_Management_System.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // Customer
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        // Worker
        public int WorkerId { get; set; }
        public Worker? Worker { get; set; }

        // Booking Details
        public DateTime BookingDate { get; set; }

        public DateTime WorkDate { get; set; }

        public string Address { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        // Payment ===
        public Payment? Payment { get; set; }
        public decimal TotalAmount { get; set; }


        public string? Description { get; set; }
    }
}