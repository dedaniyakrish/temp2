using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagement.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {


        #region Constructor
        private readonly IConfiguration configuration;

        public DoctorController(IConfiguration _configuration) {
            configuration = _configuration;
        }
        #endregion

        [HttpGet]
        public IActionResult DoctorAdd_Edit(int? id)
        {
            DoctorModel model = new DoctorModel();
            if (id.HasValue)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Doctor_SelectByPK";
                    command.Parameters.AddWithValue("@DoctorID", id.Value);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.DoctorID = reader["DoctorID"] == DBNull.Value ? 0 : (int)reader["DoctorID"];
                            model.Name = reader["Name"] == DBNull.Value ? string.Empty : reader["Name"].ToString() ?? string.Empty;
                            model.Phone = reader["Phone"] == DBNull.Value ? string.Empty : reader["Phone"].ToString() ?? string.Empty;
                            model.Email = reader["Email"] == DBNull.Value ? string.Empty : reader["Email"].ToString() ?? string.Empty;
                            model.Qualification = reader["Qualification"] == DBNull.Value ? string.Empty : reader["Qualification"].ToString() ?? string.Empty;
                            model.Specialization = reader["Specialization"] == DBNull.Value ? string.Empty : reader["Specialization"].ToString() ?? string.Empty;
                            model.IsActive = reader["IsActive"] == DBNull.Value ? true : (bool)reader["IsActive"];
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
        public IActionResult DoctorAdd_Edit(DoctorModel doctorModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    if (doctorModel.DoctorID == 0)
                    {
                        command.CommandText = "PR_Doctor_Insert";
                        command.Parameters.AddWithValue("@Name", doctorModel.Name);
                        command.Parameters.AddWithValue("@Phone", doctorModel.Phone);
                        command.Parameters.AddWithValue("@Email", doctorModel.Email);
                        command.Parameters.AddWithValue("@Qualification", doctorModel.Qualification);
                        command.Parameters.AddWithValue("@Specialization", doctorModel.Specialization);
                        command.Parameters.AddWithValue("@IsActive", doctorModel.IsActive);
                        command.Parameters.AddWithValue("@Created", doctorModel.Created);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.Parameters.AddWithValue("@UserID", doctorModel.UserID);
                        command.Parameters.Add("@NewDoctorID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Doctor added successfully!";
                    }
                    else
                    {
                        command.CommandText = "PR_Doctor_UpdateByPK";
                        command.Parameters.AddWithValue("@DoctorID", doctorModel.DoctorID);
                        command.Parameters.AddWithValue("@Name", doctorModel.Name);
                        command.Parameters.AddWithValue("@Phone", doctorModel.Phone);
                        command.Parameters.AddWithValue("@Email", doctorModel.Email);
                        command.Parameters.AddWithValue("@Qualification", doctorModel.Qualification);
                        command.Parameters.AddWithValue("@Specialization", doctorModel.Specialization);
                        command.Parameters.AddWithValue("@IsActive", doctorModel.IsActive);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.Parameters.AddWithValue("@UserID", doctorModel.UserID);
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Doctor updated successfully!";
                    }
                }
                return RedirectToAction("DoctorList");
            }
            return View(doctorModel);
        }

        #region Doctor List
        public IActionResult DoctorList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Doctor_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion

        #region Doctor Delete
        public IActionResult DoctorDelete(int id)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Doctor_DeleteByPK";
                    command.Parameters.AddWithValue("@DoctorID", id);
                    command.ExecuteNonQuery();
                }
                TempData["SuccessMessage"] = "Doctor deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting doctor: " + ex.Message;
            }
            return RedirectToAction("DoctorList");
        }
        #endregion

        #region DoctorAdd
        public IActionResult DoctorAdd(DoctorModel doctorModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (doctorModel.DoctorID == null)
                {
                    command.CommandText = "PR_Doctor_Insert";
                }
                else
                {
                    command.CommandText = "PR_Doctor_UpdateByPK";
                    command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = doctorModel.DoctorID;
                }
                command.Parameters.AddWithValue("@Name", doctorModel.Name);
                command.Parameters.AddWithValue("@Phone", doctorModel.Phone);
                command.Parameters.AddWithValue("@Email", doctorModel.Email);
                command.Parameters.AddWithValue("@Qualification", doctorModel.Qualification);
                command.Parameters.AddWithValue("@Specialization", doctorModel.Specialization);
                command.Parameters.AddWithValue("@IsActive", doctorModel.IsActive);
                command.Parameters.AddWithValue("@Created", doctorModel.Created);
                command.Parameters.AddWithValue("@Modified", doctorModel.Created);
                command.Parameters.AddWithValue("@UserID", SqlDbType.Int).Value = 1;
                command.ExecuteNonQuery();
                TempData["SuccessMessage"] = "Doctor saved successfully!";
                return RedirectToAction("DoctorList");
            }
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            return View("DoctorAdd", doctorModel);
        }
        #endregion
    }
}
