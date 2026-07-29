using System.ComponentModel.DataAnnotations;

namespace Smart_Employee_Management_System.ViewModels
{
    public class BookingCreateVM
    {
        public int WorkerId { get; set; }

        public string WorkerName { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime WorkDate { get; set; }

        [Required]
        public string Address { get; set; } = "";

        public string? Description { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }
    }
}


