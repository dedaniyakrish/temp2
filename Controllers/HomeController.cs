using System.Diagnostics;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HospitalManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                ViewBag.PatientCount = ExecuteScalarCount(connection, "SELECT COUNT(*) FROM Patient");
                ViewBag.DoctorCount = ExecuteScalarCount(connection, "SELECT COUNT(*) FROM Doctor");
                ViewBag.AppointmentCount = ExecuteScalarCount(connection, "SELECT COUNT(*) FROM Appointment");
                ViewBag.DepartmentCount = ExecuteScalarCount(connection, "SELECT COUNT(*) FROM Department");
                ViewBag.UserCount = ExecuteScalarCount(connection, "SELECT COUNT(*) FROM [User]");
                ViewBag.DoctorDepartmentCount = ExecuteScalarCount(connection, "SELECT COUNT(*) FROM DoctorDepartment");

                var chartLabels = new List<string>();
                var chartData = new List<int>();
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT dept.DepartmentName, COUNT(a.AppointmentID) AS AppointmentCount
            FROM Department dept
            LEFT JOIN DoctorDepartment dd ON dept.DepartmentID = dd.DepartmentID
            LEFT JOIN Doctor d ON dd.DoctorID = d.DoctorID
            LEFT JOIN Appointment a ON d.DoctorID = a.DoctorID
            GROUP BY dept.DepartmentName
            ORDER BY dept.DepartmentName
        ", connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        chartLabels.Add(reader["DepartmentName"].ToString());
                        chartData.Add(Convert.ToInt32(reader["AppointmentCount"]));
                    }
                }
                ViewBag.ChartLabels = chartLabels;
                ViewBag.ChartData = chartData;

                var todaysAppointments = new List<dynamic>();
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT p.Name AS PatientName, d.Name AS DoctorName, FORMAT(a.AppointmentDate, 'HH:mm') AS Time
            FROM Appointment a
            INNER JOIN Patient p ON a.PatientID = p.PatientID
            INNER JOIN Doctor d ON a.DoctorID = d.DoctorID
            WHERE CAST(a.AppointmentDate AS DATE) = CAST(GETDATE() AS DATE)
            ORDER BY a.AppointmentDate
        ", connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        todaysAppointments.Add(new {
                            PatientName = reader["PatientName"].ToString(),
                            DoctorName = reader["DoctorName"].ToString(),
                            Time = reader["Time"].ToString()
                        });
                    }
                }
                ViewBag.TodaysAppointments = todaysAppointments;

                var recentActivity = new List<string>();
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT TOP 10 p.Name AS PatientName, d.Name AS DoctorName, a.AppointmentDate
            FROM Appointment a
            INNER JOIN Patient p ON a.PatientID = p.PatientID
            INNER JOIN Doctor d ON a.DoctorID = d.DoctorID
            ORDER BY a.AppointmentDate DESC
        ", connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var activity = $"{reader["PatientName"]} had an appointment with Dr. {reader["DoctorName"]} on {Convert.ToDateTime(reader["AppointmentDate"]).ToString("dd-MM-yyyy HH:mm")}";
                        recentActivity.Add(activity);
                    }
                }
                ViewBag.RecentActivity = recentActivity;
            }
            return View();
        }

        private int ExecuteScalarCount(SqlConnection connection, string query)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                return (int)command.ExecuteScalar();
            }
        }
    }
}
