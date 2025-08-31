using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class DoctorDepartmentModel
    {
        public int DoctorDepartmentID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;

        [Required]
        public int UserID { get; set; }
    }
}
