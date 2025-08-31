using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using HospitalManagement.Filters;
using HospitalManagement.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace HospitalManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            CommonVariable.Configure(_httpContextAccessor);
        }

        public IActionResult UserAddEdit(int? id)
        {
            UserModel model = new UserModel();
            if (id.HasValue)
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_User_SelectByPK";
                        command.Parameters.AddWithValue("@UserID", id.Value);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                model.UserID = reader["UserID"] == DBNull.Value ? 0 : (int)reader["UserID"];
                                model.UserName = reader["UserName"] as string ?? string.Empty;
                                model.Password = reader["Password"] as string ?? string.Empty;
                                model.Email = reader["Email"] as string ?? string.Empty;
                                model.MobileNo = reader["MobileNo"] as string ?? string.Empty;
                                model.IsActive = reader["IsActive"] == DBNull.Value ? true : (bool)reader["IsActive"];
                                model.Created = reader["Created"] == DBNull.Value ? DateTime.Now : (DateTime)reader["Created"];
                                model.Modified = reader["Modified"] == DBNull.Value ? DateTime.Now : (DateTime)reader["Modified"];
                            }
                        }
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult UserAddEdit(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    if (userModel.UserID == 0)
                    {
                        command.CommandText = "PR_User_Insert";
                        command.Parameters.AddWithValue("@UserName", userModel.UserName ?? string.Empty);
                        command.Parameters.AddWithValue("@Password", HashPassword(userModel.Password ?? string.Empty));
                        command.Parameters.AddWithValue("@Email", userModel.Email ?? string.Empty);
                        command.Parameters.AddWithValue("@MobileNo", userModel.MobileNo ?? string.Empty);
                        command.Parameters.AddWithValue("@IsActive", userModel.IsActive);
                        command.Parameters.AddWithValue("@Created", userModel.Created);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.Parameters.Add("@NewUserID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "User added successfully!";
                    }
                    else
                    {
                        command.CommandText = "PR_User_UpdateByPK";
                        command.Parameters.AddWithValue("@UserID", userModel.UserID);
                        command.Parameters.AddWithValue("@UserName", userModel.UserName ?? string.Empty);
                        command.Parameters.AddWithValue("@Password", HashPassword(userModel.Password ?? string.Empty));
                        command.Parameters.AddWithValue("@Email", userModel.Email ?? string.Empty);
                        command.Parameters.AddWithValue("@MobileNo", userModel.MobileNo ?? string.Empty);
                        command.Parameters.AddWithValue("@IsActive", userModel.IsActive);
                        command.Parameters.AddWithValue("@Modified", DateTime.Now);
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "User updated successfully!";
                    }
                }
                return RedirectToAction("UserList");
            }
            TempData["ErrorMessage"] = "Please correct the errors and try again.";
            return View(userModel);
        }

        #region User List
        public IActionResult UserList()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_User_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        #endregion

        #region User Delete

        public IActionResult UserDelete(int id)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_User_DeleteByPK";
                    command.Parameters.AddWithValue("@UserID", id);
                    command.ExecuteNonQuery();
                }
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error deleting user.";
            }
            return RedirectToAction("UserList");
        }
        #endregion

        #region Authentication
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (CommonVariable.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel model, string? returnUrl = null)
        {
            try
            {
                // If the model is not valid, show only toast via TempData and redirect to GET
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please correct the errors and try again.";
                    return RedirectToAction("Login");
                }

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_User_ValidateLogin", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserName", model.Username ?? string.Empty);
                        command.Parameters.AddWithValue("@Password", HashPassword(model.Password ?? string.Empty));

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Read user details
                                var userId = reader["UserID"] == DBNull.Value ? 0 : (int)reader["UserID"];
                                var username = reader["UserName"] as string ?? string.Empty;
                                var email = reader["Email"] as string ?? string.Empty;
                                var role = reader["Role"] as string ?? string.Empty;

                                // Optionally store user info in session
                                try
                                {
                                    HttpContext.Session.SetString("UserID", userId.ToString());
                                    HttpContext.Session.SetString("UserName", username);
                                    HttpContext.Session.SetString("Email", email);
                                    HttpContext.Session.SetString("Role", role);
                                }
                                catch { /* session may not be configured; ignore if not */ }

                                // Sign in with cookie authentication so [Authorize] or other auth checks work
                                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                                    new Claim(ClaimTypes.Name, username),
                                    new Claim(ClaimTypes.Email, email)
                                };
                                if (!string.IsNullOrEmpty(role)) claims.Add(new Claim(ClaimTypes.Role, role));

                                var identity = new ClaimsIdentity(claims, "Cookies");
                                var principal = new ClaimsPrincipal(identity);
                                await HttpContext.SignInAsync("Cookies", principal);

                                // Set remember me cookie if needed
                                if (model.RememberMe)
                                {
                                    var options = new CookieOptions
                                    {
                                        Expires = DateTime.Now.AddDays(30),
                                        HttpOnly = true,
                                        IsEssential = true
                                    };
                                    Response.Cookies.Append("RememberMe", "true", options);
                                }

                                TempData["SuccessMessage"] = "Login successful!";

                                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                {
                                    return LocalRedirect(returnUrl);
                                }
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                // For invalid credentials show toast-only using TempData and redirect to GET
                                TempData["ErrorMessage"] = "Invalid username or password.";
                                return RedirectToAction("Login");
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request.";
            }

            // Fallback: redirect to GET Login which will display the toast
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (CommonVariable.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserRegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                    {
                        connection.Open();

                        // Check if username already exists (use UserName column)
                        using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM [User] WHERE UserName = @UserName", connection))
                        {
                            checkCmd.Parameters.AddWithValue("@UserName", model.Username ?? string.Empty);
                            int userCount = (int)checkCmd.ExecuteScalar();

                            if (userCount > 0)
                            {
                                ModelState.AddModelError("Username", "Username is already taken.");
                                return View(model);
                            }
                        }

                        // Check if email already exists
                        using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM [User] WHERE Email = @Email", connection))
                        {
                            checkCmd.Parameters.AddWithValue("@Email", model.Email ?? string.Empty);
                            int emailCount = (int)checkCmd.ExecuteScalar();

                            if (emailCount > 0)
                            {
                                ModelState.AddModelError("Email", "Email is already registered.");
                                return View(model);
                            }
                        }

                        // Create new user using PR_User_Insert (match parameter names used elsewhere)
                        using (SqlCommand command = new SqlCommand("PR_User_Insert", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@UserName", model.Username ?? string.Empty);
                            command.Parameters.AddWithValue("@Password", HashPassword(model.Password ?? string.Empty));
                            command.Parameters.AddWithValue("@Email", model.Email ?? string.Empty);
                            command.Parameters.AddWithValue("@MobileNo", model.MobileNo ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@IsActive", true);
                            command.Parameters.AddWithValue("@Created", DateTime.Now);
                            command.Parameters.AddWithValue("@Modified", DateTime.Now);

                            // Stored procedure expects an output parameter for the new user id
                            var outParam = command.Parameters.Add("@NewUserID", SqlDbType.Int);
                            outParam.Direction = ParameterDirection.Output;

                            command.ExecuteNonQuery();

                            // Optionally read the new user id
                            int? newUserId = outParam.Value != DBNull.Value ? (int?)outParam.Value : null;
                        }

                        TempData["SuccessMessage"] = "Registration successful! Please log in.";
                        return RedirectToAction("Login");
                    }
                }
            }
            catch (Exception ex)
            {
                // Provide error details for debugging; in production consider logging instead
                TempData["ErrorMessage"] = "An error occurred while processing your registration. " + ex.Message;
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            // Sign out the authentication cookie
            await HttpContext.SignOutAsync("Cookies");

            // Clear session and cookies
            try { HttpContext.Session.Clear(); } catch { }
            Response.Cookies.Delete("RememberMe");

            TempData["SuccessMessage"] = "You have been successfully logged out.";
            return RedirectToAction("Login");
        }

        [NonAction]
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        #endregion
    }
}
