using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagement.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IConfiguration configuration;

        public DepartmentController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult DepartmentAdd_Edit(int? id)
        {
            DepartmentModel model = new DepartmentModel();
            if (id.HasValue)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Department_SelectByPK";
                    command.Parameters.AddWithValue("@DepartmentID", id.Value);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.DepartmentID = reader["DepartmentID"] == DBNull.Value ? 0 : (int)reader["DepartmentID"];
                            model.DepartmentName = reader["DepartmentName"] == DBNull.Value ? string.Empty : reader["DepartmentName"].ToString() ?? string.Empty;
                            model.Description = reader["Description"] == DBNull.Value ? string.Empty : reader["Description"].ToString() ?? string.Empty;
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
        public IActionResult DepartmentAdd_Edit(DepartmentModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    if (model.DepartmentID == 0)
                    {
                        command.CommandText = "PR_Department_Insert";
                        command.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);
                        command.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IsActive", model.IsActive);
                        command.Parameters.AddWithValue("@Created", model.Created);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.Parameters.AddWithValue("@UserID", model.UserID);
                        command.Parameters.Add("@NewDepartmentID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Department added successfully!";
                    }
                    else
                    {
                        command.CommandText = "PR_Department_UpdateByPK";
                        command.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                        command.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);
                        command.Parameters.AddWithValue("@Description", (object?)model.Description ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IsActive", model.IsActive);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.Parameters.AddWithValue("@UserID", model.UserID);
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Department updated successfully!";
                    }
                }
                return RedirectToAction("DepartmentList");
            }
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            return View(model);
        }

        public IActionResult DepartmentList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Department_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }

        public IActionResult DepartmentDelete(int id)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Department_DeleteByPK";
                    command.Parameters.AddWithValue("@DepartmentID", id);
                    command.ExecuteNonQuery();
                }
                TempData["SuccessMessage"] = "Department deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting department: " + ex.Message;
            }
            return RedirectToAction("DepartmentList");
        }
    }
}
