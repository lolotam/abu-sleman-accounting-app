using AccountingApp.Database;
using AccountingApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AccountingApp.Services
{
    public class ContactService
    {
        #region Contacts
        public static List<Contact> GetAllContacts()
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Contacts.ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get all contacts error: {ex.Message}");
                return new List<Contact>();
            }
        }

        public static List<Contact> GetCustomers()
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Contacts
                        .Where(c => c.Type == ContactType.Customer || c.Type == ContactType.Both)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get customers error: {ex.Message}");
                return new List<Contact>();
            }
        }

        public static List<Contact> GetSuppliers()
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Contacts
                        .Where(c => c.Type == ContactType.Supplier || c.Type == ContactType.Both)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get suppliers error: {ex.Message}");
                return new List<Contact>();
            }
        }

        public static Contact GetContactById(int id)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Contacts.Find(id);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get contact by id error: {ex.Message}");
                return null;
            }
        }

        public static Contact GetContactByCode(string code)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Contacts.FirstOrDefault(c => c.Code == code);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get contact by code error: {ex.Message}");
                return null;
            }
        }

        public static bool CreateContact(Contact contact)
        {
            if (contact == null)
                return false;

            try
            {
                using (var context = new AccountingDbContext())
                {
                    // Check if code already exists
                    if (context.Contacts.Any(c => c.Code == contact.Code))
                        return false;

                    context.Contacts.Add(contact);
                    
                    // Add opening balance transaction if needed
                    if (contact.OpeningBalance != 0)
                    {
                        var transaction = new ContactTransaction
                        {
                            ContactId = contact.Id,
                            TransactionDate = contact.OpeningBalanceDate,
                            TransactionType = ContactTransactionType.OpeningBalance,
                            Amount = contact.OpeningBalance,
                            Reference = "Opening Balance",
                            UserId = AuthService.CurrentUser?.Id ?? 1 // Default to admin if not logged in
                        };
                        
                        context.ContactTransactions.Add(transaction);
                    }
                    
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Create contact error: {ex.Message}");
                return false;
            }
        }

        public static bool UpdateContact(Contact contact)
        {
            if (contact == null)
                return false;

            try
            {
                using (var context = new AccountingDbContext())
                {
                    // Check if code already exists for another contact
                    if (context.Contacts.Any(c => c.Code == contact.Code && c.Id != contact.Id))
                        return false;

                    contact.UpdatedAt = DateTime.Now;
                    context.Entry(contact).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Update contact error: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteContact(int id)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    var contact = context.Contacts.Find(id);
                    if (contact == null)
                        return false;

                    // Don't actually delete, just mark as inactive
                    contact.IsActive = false;
                    contact.UpdatedAt = DateTime.Now;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Delete contact error: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Contact Transactions
        public static decimal GetContactBalance(int contactId)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.ContactTransactions
                        .Where(t => t.ContactId == contactId)
                        .Sum(t => t.Amount);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get contact balance error: {ex.Message}");
                return 0;
            }
        }

        public static List<ContactTransaction> GetContactTransactions(int contactId)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.ContactTransactions
                        .Where(t => t.ContactId == contactId)
                        .OrderByDescending(t => t.TransactionDate)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get contact transactions error: {ex.Message}");
                return new List<ContactTransaction>();
            }
        }

        public static bool AddContactTransaction(ContactTransaction transaction)
        {
            if (transaction == null)
                return false;

            try
            {
                using (var context = new AccountingDbContext())
                {
                    context.ContactTransactions.Add(transaction);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Add contact transaction error: {ex.Message}");
                return false;
            }
        }
        #endregion
    }
}
