using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AccountingApp.Database;
using AccountingApp.Models;

namespace AccountingApp.Services
{
    public class AuthService
    {
        private readonly AccountingDbContext _dbContext;
        
        // Current logged in user
        public static User CurrentUser { get; private set; }

        public AuthService()
        {
            _dbContext = new AccountingDbContext();
        }

        /// <summary>
        /// Attempts to log in a user with the provided credentials
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Plain text password</param>
        /// <returns>User object if successful, null if not</returns>
        public User Login(string username, string password)
        {
            try
            {
                // Hash the password
                string passwordHash = HashPassword(password);

                // Find the user
                var user = _dbContext.Users.FirstOrDefault(u => 
                    u.Username == username && 
                    u.PasswordHash == passwordHash &&
                    u.IsActive);

                if (user != null)
                {
                    // Update last login
                    user.LastLogin = DateTime.Now;
                    _dbContext.SaveChanges();

                    // Set current user
                    CurrentUser = user;
                }

                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        public void Logout()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Changes a user's password
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="currentPassword">Current password</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            try
            {
                // Find the user
                var user = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                    return false;

                // Verify current password
                string currentPasswordHash = HashPassword(currentPassword);
                if (user.PasswordHash != currentPasswordHash)
                    return false;

                // Update password
                user.PasswordHash = HashPassword(newPassword);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a hash of the provided password using SHA-256
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Hashed password</returns>
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password to bytes
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                
                // Compute the hash
                byte[] hash = sha256.ComputeHash(bytes);
                
                // Convert the hash to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                
                return builder.ToString();
            }
        }

        /// <summary>
        /// Verifies if the current user has the required role
        /// </summary>
        /// <param name="role">Required role</param>
        /// <returns>True if user has the role, false otherwise</returns>
        public static bool HasRole(string role)
        {
            if (CurrentUser == null)
                return false;

            return CurrentUser.Role == role || CurrentUser.Role == "Admin";
        }
    }
}