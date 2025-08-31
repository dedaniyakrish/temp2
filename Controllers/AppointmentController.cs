using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagement.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IConfiguration configuration;

        public AppointmentController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IActionResult AppointmentAdd_Edit(int? id)
        {
            AppointmentModel model = new AppointmentModel();
            ViewBag.Doctors = GetDoctors();
            ViewBag.Patients = GetPatients();
            if (id.HasValue)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Appointment_SelectByPK";
                    command.Parameters.AddWithValue("@AppointmentID", id.Value);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.AppointmentID = reader["AppointmentID"] == DBNull.Value ? 0 : (int)reader["AppointmentID"];
                            model.DoctorID = reader["DoctorID"] == DBNull.Value ? 0 : (int)reader["DoctorID"];
                            model.PatientID = reader["PatientID"] == DBNull.Value ? 0 : (int)reader["PatientID"];
                            model.AppointmentDate = reader["AppointmentDate"] == DBNull.Value ? DateTime.Now : (DateTime)reader["AppointmentDate"];
                            model.AppointmentStatus = reader["AppointmentStatus"] == DBNull.Value ? string.Empty : reader["AppointmentStatus"].ToString() ?? string.Empty;
                            model.Description = reader["Description"] == DBNull.Value ? string.Empty : reader["Description"].ToString() ?? string.Empty;
                            model.SpecialRemarks = reader["SpecialRemarks"] == DBNull.Value ? string.Empty : reader["SpecialRemarks"].ToString() ?? string.Empty;
                            model.TotalConsultedAmount = reader["TotalConsultedAmount"] == DBNull.Value ? null : (decimal?)reader["TotalConsultedAmount"];
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult AppointmentAdd_Edit(AppointmentModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    if (model.AppointmentID == 0)
                    {
                        command.CommandText = "PR_Appointment_Insert";
                        command.Parameters.AddWithValue("@DoctorID", model.DoctorID);
                        command.Parameters.AddWithValue("@PatientID", model.PatientID);
                        command.Parameters.AddWithValue("@AppointmentDate", model.AppointmentDate);
                        command.Parameters.AddWithValue("@AppointmentStatus", model.AppointmentStatus);
                        command.Parameters.AddWithValue("@Description", model.Description);
                        command.Parameters.AddWithValue("@SpecialRemarks", model.SpecialRemarks);
                        command.Parameters.AddWithValue("@Created", model.Created);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.Parameters.AddWithValue("@UserID", model.UserID);
                        command.Parameters.AddWithValue("@TotalConsultedAmount", (object?)model.TotalConsultedAmount ?? DBNull.Value);
                        command.Parameters.Add("@NewAppointmentID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Appointment added successfully!";
                    }
                    else
                    {
                        command.CommandText = "PR_Appointment_UpdateByPK";
                        command.Parameters.AddWithValue("@AppointmentID", model.AppointmentID);
                        command.Parameters.AddWithValue("@DoctorID", model.DoctorID);
                        command.Parameters.AddWithValue("@PatientID", model.PatientID);
                        command.Parameters.AddWithValue("@AppointmentDate", model.AppointmentDate);
                        command.Parameters.AddWithValue("@AppointmentStatus", model.AppointmentStatus);
                        command.Parameters.AddWithValue("@Description", model.Description);
                        command.Parameters.AddWithValue("@SpecialRemarks", model.SpecialRemarks);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.Parameters.AddWithValue("@UserID", model.UserID);
                        command.Parameters.AddWithValue("@TotalConsultedAmount", (object?)model.TotalConsultedAmount ?? DBNull.Value);
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Appointment updated successfully!";
                    }
                }
                return RedirectToAction("AppointmentList");
            }
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            ViewBag.Doctors = GetDoctors();
            ViewBag.Patients = GetPatients();
            return View(model);
        }

        public IActionResult AppointmentList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Appointment_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult AppointmentDelete(int id) 
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Appointment_DeleteByPK";
                    command.Parameters.AddWithValue("@AppointmentID", id);
                    command.ExecuteNonQuery();
                }
                TempData["SuccessMessage"] = "Appointment deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting appointment: " + ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("AppointmentList");
        }

        private List<DoctorModel> GetDoctors()
        {
            var list = new List<DoctorModel>();
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT DoctorID, Name FROM Doctor WHERE IsActive=1";
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DoctorModel { DoctorID = (int)reader["DoctorID"], Name = reader["Name"].ToString() });
                    }
                }
            }
            return list;
        }

        private List<PatientModel> GetPatients()
        {
            var list = new List<PatientModel>();
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT PatientID, Name FROM Patient WHERE IsActive=1";
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PatientModel { PatientID = (int)reader["PatientID"], Name = reader["Name"].ToString() });
                    }
                }
            }
            return list;
        }
    }
}
