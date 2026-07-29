using System;

namespace Smart_Employee_Management_System.Models
{
    public class Payment
    {
        public int Id { get; set; }

        // Booking
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        // Payment Details
        public decimal Amount { get; set; }

        // Cash / UPI / Debit Card / Credit Card / Net Banking
        public string PaymentMethod { get; set; } = string.Empty;

        // Pending / Paid / Failed
        public string PaymentStatus { get; set; } = "Pending";

        public string? TransactionId { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        // ==========================
        // UPI Payment
        // ==========================
        public string? UpiId { get; set; }

        // ==========================
        // Card Payment
        // ==========================
        public string? CardHolderName { get; set; }

        public string? CardNumber { get; set; }

        public string? ExpiryDate { get; set; }

        public string? CVV { get; set; }

        // ==========================
        // Net Banking
        // ==========================
        public string? BankName { get; set; }

        public string? AccountNumber { get; set; }

        public string? IFSCCode { get; set; }
    }
}