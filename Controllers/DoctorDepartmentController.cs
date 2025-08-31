using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagement.Controllers
{
    [Authorize]
    public class DoctorDepartmentController : Controller
    {

        #region Constructor
        private readonly IConfiguration configuration;

        public DoctorDepartmentController(IConfiguration configuration) {
            this.configuration = configuration;
        }

        #endregion

        public IActionResult DoctorDepartmentAdd_Edit(int? id)
        {
            DoctorDepartmentModel model = new DoctorDepartmentModel();
            ViewBag.Doctors = GetDoctors();
            ViewBag.Departments = GetDepartments();
            if (id.HasValue)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_DoctorDepartment_SelectByPK";
                    command.Parameters.AddWithValue("@DoctorDepartmentID", id.Value);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.DoctorDepartmentID = reader["DoctorDepartmentID"] == DBNull.Value ? 0 : (int)reader["DoctorDepartmentID"];
                            model.DoctorID = reader["DoctorID"] == DBNull.Value ? 0 : (int)reader["DoctorID"];
                            model.DepartmentID = reader["DepartmentID"] == DBNull.Value ? 0 : (int)reader["DepartmentID"];
                            model.Created = reader["Created"] == DBNull.Value ? DateTime.Now : (DateTime)reader["Created"];
                            model.Modified = reader["Modified"] == DBNull.Value ? DateTime.Now : (DateTime)reader["Modified"];
                            model.UserID = reader["UserID"] == DBNull.Value ? 0 : (int)reader["UserID"];
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult DoctorDepartmentAdd_Edit(DoctorDepartmentModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    if (model.DoctorDepartmentID == 0)
                    {
                        command.CommandText = "PR_DoctorDepartment_Insert";
                        command.Parameters.AddWithValue("@DoctorID", model.DoctorID);
                        command.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                        command.Parameters.AddWithValue("@Created", model.Created);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.Parameters.AddWithValue("@UserID", model.UserID);
                        command.Parameters.Add("@NewDoctorDepartmentID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Doctor-Department assignment added successfully!";
                    }
                    else
                    {
                        command.CommandText = "PR_DoctorDepartment_UpdateByPK";
                        command.Parameters.AddWithValue("@DoctorDepartmentID", model.DoctorDepartmentID);
                        command.Parameters.AddWithValue("@DoctorID", model.DoctorID);
                        command.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.Parameters.AddWithValue("@UserID", model.UserID);
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Doctor-Department assignment updated successfully!";
                    }
                }
                return RedirectToAction("DoctorDepartmentList");
            }
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            ViewBag.Doctors = GetDoctors();
            ViewBag.Departments = GetDepartments();
            return View(model);
        }

        public IActionResult DoctorDepartmentList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_DoctorDepartment_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }

        public IActionResult DoctorDepartmentDelete(int id)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_DoctorDepartment_DeleteByPK";
                    command.Parameters.AddWithValue("@DoctorDepartmentID", id);
                    command.ExecuteNonQuery();
                }
                TempData["SuccessMessage"] = "Doctor-Department assignment deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting doctor-department assignment: " + ex.Message;
            }
            return RedirectToAction("DoctorDepartmentList");
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

        private List<DepartmentModel> GetDepartments()
        {
            var list = new List<DepartmentModel>();
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT DepartmentID, DepartmentName FROM Department WHERE IsActive=1";
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DepartmentModel { DepartmentID = (int)reader["DepartmentID"], DepartmentName = reader["DepartmentName"].ToString() });
                    }
                }
            }
            return list;
        }
    }
}
