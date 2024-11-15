namespace kartverketprosjekt.Services.Admin
{
    public interface IAdminService
    {
        public AdminStats GetAdminViewStats();
        public bool UpdateUserAccess(string userId, int newAccessLevel, out string message);
        public bool DeleteUser(string email, string loggedInUserEmail, out string errorMessage);
        public bool CreateUser(string email, string password, int accessLevel, string organization, string? name, out string errorMessage);
    }
}
