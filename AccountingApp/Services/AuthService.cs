using AccountingApp.Database;
using AccountingApp.Models;
using System;
using System.Linq;

namespace AccountingApp.Services
{
    public class AuthService
    {
        private static User _currentUser;

        public static User CurrentUser
        {
            get { return _currentUser; }
        }

        public static bool Login(string username, string password)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    var user = context.Users
                        .FirstOrDefault(u => u.Username == username && u.IsActive);

                    if (user != null && user.VerifyPassword(password))
                    {
                        // Update last login time
                        user.LastLogin = DateTime.Now;
                        context.SaveChanges();

                        // Set current user
                        _currentUser = user;

                        // Log the login
                        LogActivity(user.Id, "Login", "User", user.Id, $"User {user.Username} logged in");

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Login error: {ex.Message}");
            }

            return false;
        }

        public static void Logout()
        {
            if (_currentUser != null)
            {
                // Log the logout
                LogActivity(_currentUser.Id, "Logout", "User", _currentUser.Id, $"User {_currentUser.Username} logged out");
                
                // Clear current user
                _currentUser = null;
            }
        }

        public static bool ChangePassword(string oldPassword, string newPassword)
        {
            if (_currentUser == null)
                return false;

            try
            {
                using (var context = new AccountingDbContext())
                {
                    var user = context.Users.Find(_currentUser.Id);

                    if (user != null && user.VerifyPassword(oldPassword))
                    {
                        user.Password = newPassword;
                        context.SaveChanges();

                        // Update current user
                        _currentUser = user;

                        // Log the password change
                        LogActivity(user.Id, "ChangePassword", "User", user.Id, $"User {user.Username} changed password");

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Change password error: {ex.Message}");
            }

            return false;
        }

        public static bool HasPermission(string permission)
        {
            if (_currentUser == null)
                return false;

            // For now, simple role-based permissions
            switch (_currentUser.Role)
            {
                case UserRole.Admin:
                    return true; // Admin has all permissions
                case UserRole.Accountant:
                    // Accountant has all permissions except user management
                    return !permission.StartsWith("User");
                case UserRole.Viewer:
                    // Viewer has only view permissions
                    return permission.StartsWith("View");
                default:
                    return false;
            }
        }

        private static void LogActivity(int userId, string action, string entityType, int? entityId, string details)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    var log = new AuditLog
                    {
                        UserId = userId,
                        Action = action,
                        EntityType = entityType,
                        EntityId = entityId,
                        Details = details,
                        Timestamp = DateTime.Now
                    };

                    context.AuditLogs.Add(log);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Log activity error: {ex.Message}");
            }
        }
    }
}
