using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagement.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        #region Constructor
        private readonly IConfiguration configuration;

        public PatientController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        [HttpGet]
        public IActionResult PatientAddEdit(int? id)
        {
            PatientModel model = new PatientModel();
            if (id.HasValue)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Patient_SelectByPK";
                    command.Parameters.AddWithValue("@PatientID", id.Value);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.PatientID = reader["PatientID"] == DBNull.Value ? 0 : (int)reader["PatientID"];
                            model.Name = reader["Name"] == DBNull.Value ? string.Empty : reader["Name"].ToString() ?? string.Empty;
                            model.DateOfBirth = reader["DateOfBirth"] == DBNull.Value ? DateTime.Now : (DateTime)reader["DateOfBirth"];
                            model.Gender = reader["Gender"] == DBNull.Value ? string.Empty : reader["Gender"].ToString() ?? string.Empty;
                            model.Email = reader["Email"] == DBNull.Value ? string.Empty : reader["Email"].ToString() ?? string.Empty;
                            model.Phone = reader["Phone"] == DBNull.Value ? string.Empty : reader["Phone"].ToString() ?? string.Empty;
                            model.Address = reader["Address"] == DBNull.Value ? string.Empty : reader["Address"].ToString() ?? string.Empty;
                            model.City = reader["City"] == DBNull.Value ? string.Empty : reader["City"].ToString() ?? string.Empty;
                            model.State = reader["State"] == DBNull.Value ? string.Empty : reader["State"].ToString() ?? string.Empty;
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

        #region Patient List
        public IActionResult PatientList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Patient_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion

        [HttpPost]
        public IActionResult PatientAddEdit(PatientModel patientModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    if (patientModel.PatientID == 0)
                    {
                        command.CommandText = "PR_Patient_Insert";
                        command.Parameters.AddWithValue("@Name", patientModel.Name);
                        command.Parameters.AddWithValue("@DateOfBirth", patientModel.DateOfBirth);
                        command.Parameters.AddWithValue("@Gender", patientModel.Gender);
                        command.Parameters.AddWithValue("@Email", patientModel.Email);
                        command.Parameters.AddWithValue("@Phone", patientModel.Phone);
                        command.Parameters.AddWithValue("@Address", patientModel.Address);
                        command.Parameters.AddWithValue("@City", patientModel.City);
                        command.Parameters.AddWithValue("@State", patientModel.State);
                        command.Parameters.AddWithValue("@IsActive", patientModel.IsActive);
                        command.Parameters.AddWithValue("@Created", patientModel.Created);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.Parameters.AddWithValue("@UserID", patientModel.UserID);
                        command.Parameters.Add("@NewPatientID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Patient added successfully!";
                    }
                    else
                    {
                        command.CommandText = "PR_Patient_UpdateByPK";
                        command.Parameters.AddWithValue("@PatientID", patientModel.PatientID);
                        command.Parameters.AddWithValue("@Name", patientModel.Name);
                        command.Parameters.AddWithValue("@DateOfBirth", patientModel.DateOfBirth);
                        command.Parameters.AddWithValue("@Gender", patientModel.Gender);
                        command.Parameters.AddWithValue("@Email", patientModel.Email);
                        command.Parameters.AddWithValue("@Phone", patientModel.Phone);
                        command.Parameters.AddWithValue("@Address", patientModel.Address);
                        command.Parameters.AddWithValue("@City", patientModel.City);
                        command.Parameters.AddWithValue("@State", patientModel.State);
                        command.Parameters.AddWithValue("@IsActive", patientModel.IsActive);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.Parameters.AddWithValue("@UserID", patientModel.UserID);
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Patient updated successfully!";
                    }
                }
                return RedirectToAction("PatientList");
            }
            return View(patientModel);
        }

        #region Patient Delete
        public IActionResult PatientDelete(int id)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Patient_DeleteByPK";
                    command.Parameters.AddWithValue("@PatientID", id);
                    command.ExecuteNonQuery();
                }
                TempData["SuccessMessage"] = "Patient deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting patient: " + ex.Message;
            }
            return RedirectToAction("PatientList");
        }
        #endregion

        #region PatientAdd

        public IActionResult PatientAdd(PatientModel patientModel)
        {

            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (patientModel.PatientID == null || patientModel.PatientID == 0)
                {
                    command.CommandText = "PR_Patient_Insert";
                }
                else
                {
                    command.CommandText = "PR_Patient_  ";
                    command.Parameters.Add("@PatientID", SqlDbType.Int).Value = patientModel.PatientID;
                }
                command.Parameters.AddWithValue("@Name", patientModel.Name);
                command.Parameters.AddWithValue("@DateOfBirth",patientModel.DateOfBirth);
                command.Parameters.AddWithValue("@Gender", patientModel.Gender);
                command.Parameters.AddWithValue("@Email", patientModel.Email);
                command.Parameters.AddWithValue("@Phone", patientModel.Phone);
                command.Parameters.AddWithValue("@Address", patientModel.Address);
                command.Parameters.AddWithValue("@City", patientModel.City);
                command.Parameters.AddWithValue("@State", patientModel.State);
                command.Parameters.AddWithValue("@Created", patientModel.Created);
                command.Parameters.AddWithValue("@IsActive", patientModel.IsActive);
                command.Parameters.AddWithValue("@Modified", patientModel.Created);
                command.Parameters.AddWithValue("@UserID", SqlDbType.Int).Value = 1;
                command.ExecuteNonQuery();
                TempData["SuccessMessage"] = "Patient saved successfully!";
                return RedirectToAction("PatientList");
            }
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            return View("PatientAddEdit", patientModel);
        }
        #endregion
    }
}
