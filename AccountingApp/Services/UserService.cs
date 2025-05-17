using AccountingApp.Database;
using AccountingApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AccountingApp.Services
{
    public class UserService
    {
        public static List<User> GetAllUsers()
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Users.ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get all users error: {ex.Message}");
                return new List<User>();
            }
        }

        public static User GetUserById(int id)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Users.Find(id);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get user by id error: {ex.Message}");
                return null;
            }
        }

        public static User GetUserByUsername(string username)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Users.FirstOrDefault(u => u.Username == username);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get user by username error: {ex.Message}");
                return null;
            }
        }

        public static bool CreateUser(User user)
        {
            if (user == null)
                return false;

            try
            {
                using (var context = new AccountingDbContext())
                {
                    // Check if username already exists
                    if (context.Users.Any(u => u.Username == user.Username))
                        return false;

                    context.Users.Add(user);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Create user error: {ex.Message}");
                return false;
            }
        }

        public static bool UpdateUser(User user)
        {
            if (user == null)
                return false;

            try
            {
                using (var context = new AccountingDbContext())
                {
                    // Check if username already exists for another user
                    if (context.Users.Any(u => u.Username == user.Username && u.Id != user.Id))
                        return false;

                    context.Entry(user).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Update user error: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteUser(int id)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    var user = context.Users.Find(id);
                    if (user == null)
                        return false;

                    // Don't actually delete, just mark as inactive
                    user.IsActive = false;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Delete user error: {ex.Message}");
                return false;
            }
        }

        public static bool ResetPassword(int id, string newPassword)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    var user = context.Users.Find(id);
                    if (user == null)
                        return false;

                    user.Password = newPassword;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Reset password error: {ex.Message}");
                return false;
            }
        }
    }
}
