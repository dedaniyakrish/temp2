using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class UserModel
    {
        public int UserID { get; set; } = 0;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string MobileNo { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;

    }

    public class UserDropDownModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
