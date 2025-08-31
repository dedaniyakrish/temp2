using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class AppointmentModel
    {
        public int AppointmentID { get; set; }

        [Required(ErrorMessage = "Doctor is required")]
        public int DoctorID { get; set; }

        [Required(ErrorMessage = "Patient is required")]
        public int PatientID { get; set; }

        [Required(ErrorMessage = "Appointment Date is required")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Appointment Status is required")]
        public string AppointmentStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Special Remarks are required")]
        public string SpecialRemarks { get; set; } = string.Empty;

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "User is required")]
        public int UserID { get; set; }

        public decimal? TotalConsultedAmount { get; set; }
    }
}
