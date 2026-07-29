using System;

namespace Smart_Employee_Management_System.Models
{
    public class Review
    {
        public int Id { get; set; }

        // Customer
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        // Worker
        public int WorkerId { get; set; }
        public Worker? Worker { get; set; }

        // Rating (1 to 5)
        public int Rating { get; set; }

        // Review Message
        public string Comment { get; set; } = string.Empty;

        // Review Date
        public DateTime ReviewDate { get; set; } = DateTime.Now;
    }
}