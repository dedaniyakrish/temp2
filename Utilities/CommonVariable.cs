using Microsoft.AspNetCore.Http;

namespace HospitalManagement.Utilities
{
    public static class CommonVariable
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static int? UserID()
        {
            var userId = _httpContextAccessor?.HttpContext?.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            return int.Parse(userId);
        }

        public static string UserName()
        {
            return _httpContextAccessor?.HttpContext?.Session.GetString("UserName") ?? string.Empty;
        }

        public static string Email()
        {
            return _httpContextAccessor?.HttpContext?.Session.GetString("Email") ?? string.Empty;
        }

        public static string Role()
        {
            return _httpContextAccessor?.HttpContext?.Session.GetString("Role") ?? string.Empty;
        }

        public static bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(_httpContextAccessor?.HttpContext?.Session.GetString("UserID"));
        }
    }
}
