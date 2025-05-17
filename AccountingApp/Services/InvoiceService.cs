using AccountingApp.Database;
using AccountingApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AccountingApp.Services
{
    public class InvoiceService
    {
        #region Invoices
        public static List<Invoice> GetAllInvoices()
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Invoices
                        .Include(i => i.Contact)
                        .Include(i => i.Warehouse)
                        .Include(i => i.User)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get all invoices error: {ex.Message}");
                return new List<Invoice>();
            }
        }

        public static List<Invoice> GetInvoicesByType(InvoiceType type)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Invoices
                        .Include(i => i.Contact)
                        .Include(i => i.Warehouse)
                        .Include(i => i.User)
                        .Where(i => i.Type == type)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get invoices by type error: {ex.Message}");
                return new List<Invoice>();
            }
        }

        public static List<Invoice> GetInvoicesByContact(int contactId)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Invoices
                        .Include(i => i.Contact)
                        .Include(i => i.Warehouse)
                        .Include(i => i.User)
                        .Where(i => i.ContactId == contactId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get invoices by contact error: {ex.Message}");
                return new List<Invoice>();
            }
        }

        public static Invoice GetInvoiceById(int id, bool includeItems = false, bool includePayments = false)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    var query = context.Invoices
                        .Include(i => i.Contact)
                        .Include(i => i.Warehouse)
                        .Include(i => i.User);

                    if (includeItems)
                        query = query.Include(i => i.Items.Select(item => item.Product));

                    if (includePayments)
                        query = query.Include(i => i.Payments);

                    return query.FirstOrDefault(i => i.Id == id);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get invoice by id error: {ex.Message}");
                return null;
            }
        }

        public static Invoice GetInvoiceByNumber(string invoiceNumber)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Invoices
                        .Include(i => i.Contact)
                        .Include(i => i.Warehouse)
                        .Include(i => i.User)
                        .FirstOrDefault(i => i.InvoiceNumber == invoiceNumber);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get invoice by number error: {ex.Message}");
                return null;
            }
        }

        public static string GenerateInvoiceNumber(InvoiceType type)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    string prefix = "";
                    switch (type)
                    {
                        case InvoiceType.SalesInvoice:
                            prefix = "SI";
                            break;
                        case InvoiceType.SalesReturn:
                            prefix = "SR";
                            break;
                        case InvoiceType.PurchaseInvoice:
                            prefix = "PI";
                            break;
                        case InvoiceType.PurchaseReturn:
                            prefix = "PR";
                            break;
                    }

                    // Get current year and month
                    string yearMonth = DateTime.Now.ToString("yyMM");
                    
                    // Get last invoice number with this prefix and year/month
                    var lastInvoice = context.Invoices
                        .Where(i => i.InvoiceNumber.StartsWith(prefix + yearMonth))
                        .OrderByDescending(i => i.InvoiceNumber)
                        .FirstOrDefault();

                    int sequence = 1;
                    if (lastInvoice != null)
                    {
                        // Extract sequence number from last invoice number
                        string sequenceStr = lastInvoice.InvoiceNumber.Substring(prefix.Length + yearMonth.Length);
                        if (int.TryParse(sequenceStr, out int lastSequence))
                        {
                            sequence = lastSequence + 1;
                        }
                    }

                    // Format: PREFIX + YYMM + SEQUENCE (4 digits)
                    return $"{prefix}{yearMonth}{sequence:D4}";
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Generate invoice number error: {ex.Message}");
                return $"{DateTime.Now:yyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4)}";
            }
        }

        public static bool CreateInvoice(Invoice invoice, List<InvoiceItem> items)
        {
            if (invoice == null || items == null || !items.Any())
                return false;

            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    using (var context = new AccountingDbContext())
                    {
                        // Check if invoice number already exists
                        if (context.Invoices.Any(i => i.InvoiceNumber == invoice.InvoiceNumber))
                            return false;

                        // Calculate totals
                        decimal subTotal = 0;
                        decimal taxAmount = 0;
                        decimal discountAmount = 0;

                        foreach (var item in items)
                        {
                            item.Total = (item.Quantity * item.UnitPrice) - item.DiscountAmount + item.TaxAmount;
                            subTotal += (item.Quantity * item.UnitPrice);
                            taxAmount += item.TaxAmount;
                            discountAmount += item.DiscountAmount;
                        }

                        invoice.SubTotal = subTotal;
                        invoice.TaxAmount = taxAmount;
                        invoice.DiscountAmount = discountAmount;
                        invoice.Total = subTotal - discountAmount + taxAmount;
                        invoice.DueAmount = invoice.Total - invoice.PaidAmount;
                        invoice.Status = invoice.PaidAmount >= invoice.Total ? InvoiceStatus.Paid : 
                                        (invoice.PaidAmount > 0 ? InvoiceStatus.PartiallyPaid : InvoiceStatus.Confirmed);

                        // Save invoice
                        context.Invoices.Add(invoice);
                        context.SaveChanges();

                        // Save items
                        foreach (var item in items)
                        {
                            item.InvoiceId = invoice.Id;
                            context.InvoiceItems.Add(item);
                        }

                        context.SaveChanges();

                        // Update inventory
                        UpdateInventoryForInvoice(invoice, items);

                        // Update contact balance
                        UpdateContactBalanceForInvoice(invoice);

                        transaction.Complete();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    // Log error
                    Console.WriteLine($"Create invoice error: {ex.Message}");
                    return false;
                }
            }
        }

        public static bool UpdateInvoice(Invoice invoice, List<InvoiceItem> items)
        {
            if (invoice == null || items == null || !items.Any())
                return false;

            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    using (var context = new AccountingDbContext())
                    {
                        // Check if invoice exists
                        var existingInvoice = context.Invoices
                            .Include(i => i.Items)
                            .FirstOrDefault(i => i.Id == invoice.Id);

                        if (existingInvoice == null)
                            return false;

                        // Check if invoice number already exists for another invoice
                        if (context.Invoices.Any(i => i.InvoiceNumber == invoice.InvoiceNumber && i.Id != invoice.Id))
                            return false;

                        // Reverse inventory transactions
                        ReverseInventoryForInvoice(existingInvoice);

                        // Reverse contact balance
                        ReverseContactBalanceForInvoice(existingInvoice);

                        // Remove existing items
                        foreach (var item in existingInvoice.Items.ToList())
                        {
                            context.InvoiceItems.Remove(item);
                        }

                        // Calculate totals
                        decimal subTotal = 0;
                        decimal taxAmount = 0;
                        decimal discountAmount = 0;

                        foreach (var item in items)
                        {
                            item.Total = (item.Quantity * item.UnitPrice) - item.DiscountAmount + item.TaxAmount;
                            subTotal += (item.Quantity * item.UnitPrice);
                            taxAmount += item.TaxAmount;
                            discountAmount += item.DiscountAmount;
                        }

                        // Update invoice
                        existingInvoice.ContactId = invoice.ContactId;
                        existingInvoice.WarehouseId = invoice.WarehouseId;
                        existingInvoice.InvoiceDate = invoice.InvoiceDate;
                        existingInvoice.DueDate = invoice.DueDate;
                        existingInvoice.Reference = invoice.Reference;
                        existingInvoice.Notes = invoice.Notes;
                        existingInvoice.PaymentMethod = invoice.PaymentMethod;
                        existingInvoice.SubTotal = subTotal;
                        existingInvoice.TaxAmount = taxAmount;
                        existingInvoice.DiscountAmount = discountAmount;
                        existingInvoice.Total = subTotal - discountAmount + taxAmount;
                        existingInvoice.DueAmount = existingInvoice.Total - existingInvoice.PaidAmount;
                        existingInvoice.Status = existingInvoice.PaidAmount >= existingInvoice.Total ? InvoiceStatus.Paid : 
                                               (existingInvoice.PaidAmount > 0 ? InvoiceStatus.PartiallyPaid : InvoiceStatus.Confirmed);
                        existingInvoice.UpdatedAt = DateTime.Now;

                        // Add new items
                        foreach (var item in items)
                        {
                            item.InvoiceId = existingInvoice.Id;
                            context.InvoiceItems.Add(item);
                        }

                        context.SaveChanges();

                        // Update inventory
                        UpdateInventoryForInvoice(existingInvoice, items);

                        // Update contact balance
                        UpdateContactBalanceForInvoice(existingInvoice);

                        transaction.Complete();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    // Log error
                    Console.WriteLine($"Update invoice error: {ex.Message}");
                    return false;
                }
            }
        }

        public static bool DeleteInvoice(int id)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    using (var context = new AccountingDbContext())
                    {
                        var invoice = context.Invoices
                            .Include(i => i.Items)
                            .FirstOrDefault(i => i.Id == id);

                        if (invoice == null)
                            return false;

                        // Reverse inventory transactions
                        ReverseInventoryForInvoice(invoice);

                        // Reverse contact balance
                        ReverseContactBalanceForInvoice(invoice);

                        // Remove items
                        foreach (var item in invoice.Items.ToList())
                        {
                            context.InvoiceItems.Remove(item);
                        }

                        // Remove payments
                        var payments = context.InvoicePayments.Where(p => p.InvoiceId == id).ToList();
                        foreach (var payment in payments)
                        {
                            context.InvoicePayments.Remove(payment);
                        }

                        // Remove invoice
                        context.Invoices.Remove(invoice);
                        context.SaveChanges();

                        transaction.Complete();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    // Log error
                    Console.WriteLine($"Delete invoice error: {ex.Message}");
                    return false;
                }
            }
        }
        #endregion

        #region Payments
        public static bool AddPayment(InvoicePayment payment)
        {
            if (payment == null)
                return false;

            using (var transaction = new System.Transactions.TransactionScope())
            {
                try
                {
                    using (var context = new AccountingDbContext())
                    {
                        var invoice = context.Invoices.Find(payment.InvoiceId);
                        if (invoice == null)
                            return false;

                        // Add payment
                        context.InvoicePayments.Add(payment);

                        // Update invoice
                        invoice.PaidAmount += payment.Amount;
                        invoice.DueAmount = invoice.Total - invoice.PaidAmount;
                        invoice.Status = invoice.PaidAmount >= invoice.Total ? InvoiceStatus.Paid : 
                                       (invoice.PaidAmount > 0 ? InvoiceStatus.PartiallyPaid : InvoiceStatus.Confirmed);
                        invoice.UpdatedAt = DateTime.Now;

                        context.SaveChanges();

                        // Add contact transaction
                        var contactTransaction = new ContactTransaction
                        {
                            ContactId = invoice.ContactId,
                            TransactionDate = payment.PaymentDate,
                            TransactionType = ContactTransactionType.Payment,
                            Amount = payment.Amount * (invoice.Type == InvoiceType.SalesInvoice || invoice.Type == InvoiceType.PurchaseReturn ? 1 : -1),
                            Reference = $"Payment for {invoice.InvoiceNumber}",
                            SourceDocumentId = invoice.Id,
                            SourceDocumentType = SourceDocumentType.SalesInvoice,
                            UserId = payment.UserId,
                            Notes = payment.Notes
                        };

                        context.ContactTransactions.Add(contactTransaction);
                        context.SaveChanges();

                        transaction.Complete();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    // Log error
                    Console.WriteLine($"Add payment error: {ex.Message}");
                    return false;
                }
            }
        }

        public static List<InvoicePayment> GetPaymentsByInvoice(int invoiceId)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.InvoicePayments
                        .Where(p => p.InvoiceId == invoiceId)
                        .OrderByDescending(p => p.PaymentDate)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get payments by invoice error: {ex.Message}");
                return new List<InvoicePayment>();
            }
        }
        #endregion

        #region Helper Methods
        private static void UpdateInventoryForInvoice(Invoice invoice, List<InvoiceItem> items)
        {
            try
            {
                // Determine transaction type based on invoice type
                InventoryTransactionType transactionType;
                switch (invoice.Type)
                {
                    case InvoiceType.SalesInvoice:
                        transactionType = InventoryTransactionType.Sale;
                        break;
                    case InvoiceType.SalesReturn:
                        transactionType = InventoryTransactionType.Return;
                        break;
                    case InvoiceType.PurchaseInvoice:
                        transactionType = InventoryTransactionType.Purchase;
                        break;
                    case InvoiceType.PurchaseReturn:
                        transactionType = InventoryTransactionType.Return;
                        break;
                    default:
                        return;
                }

                // Update inventory for each item
                foreach (var item in items)
                {
                    decimal quantity = item.Quantity;
                    
                    // For sales, we reduce inventory; for purchases, we increase it
                    if (invoice.Type == InvoiceType.SalesInvoice)
                        quantity = -quantity;
                    else if (invoice.Type == InvoiceType.PurchaseReturn)
                        quantity = -quantity;

                    ProductService.UpdateInventory(
                        item.ProductId,
                        invoice.WarehouseId,
                        Math.Abs(quantity),
                        transactionType,
                        invoice.InvoiceNumber,
                        invoice.UserId,
                        invoice.Id,
                        invoice.Type == InvoiceType.SalesInvoice || invoice.Type == InvoiceType.SalesReturn ? 
                            SourceDocumentType.SalesInvoice : SourceDocumentType.PurchaseInvoice
                    );
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Update inventory for invoice error: {ex.Message}");
            }
        }

        private static void ReverseInventoryForInvoice(Invoice invoice)
        {
            try
            {
                // Determine transaction type based on invoice type (opposite of original)
                InventoryTransactionType transactionType;
                switch (invoice.Type)
                {
                    case InvoiceType.SalesInvoice:
                        transactionType = InventoryTransactionType.Return; // Reverse of sale
                        break;
                    case InvoiceType.SalesReturn:
                        transactionType = InventoryTransactionType.Sale; // Reverse of return
                        break;
                    case InvoiceType.PurchaseInvoice:
                        transactionType = InventoryTransactionType.Return; // Reverse of purchase
                        break;
                    case InvoiceType.PurchaseReturn:
                        transactionType = InventoryTransactionType.Purchase; // Reverse of return
                        break;
                    default:
                        return;
                }

                // Reverse inventory for each item
                foreach (var item in invoice.Items)
                {
                    decimal quantity = item.Quantity;
                    
                    // For sales, we increase inventory to reverse; for purchases, we decrease it
                    if (invoice.Type == InvoiceType.SalesReturn)
                        quantity = -quantity;
                    else if (invoice.Type == InvoiceType.PurchaseInvoice)
                        quantity = -quantity;

                    ProductService.UpdateInventory(
                        item.ProductId,
                        invoice.WarehouseId,
                        Math.Abs(quantity),
                        transactionType,
                        $"Reverse {invoice.InvoiceNumber}",
                        invoice.UserId,
                        invoice.Id,
                        invoice.Type == InvoiceType.SalesInvoice || invoice.Type == InvoiceType.SalesReturn ? 
                            SourceDocumentType.SalesInvoice : SourceDocumentType.PurchaseInvoice
                    );
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Reverse inventory for invoice error: {ex.Message}");
            }
        }

        private static void UpdateContactBalanceForInvoice(Invoice invoice)
        {
            try
            {
                // Determine transaction type and amount sign based on invoice type
                ContactTransactionType transactionType;
                decimal amountSign;

                switch (invoice.Type)
                {
                    case InvoiceType.SalesInvoice:
                        transactionType = ContactTransactionType.Invoice;
                        amountSign = 1; // Positive for receivables
                        break;
                    case InvoiceType.SalesReturn:
                        transactionType = ContactTransactionType.CreditNote;
                        amountSign = -1; // Negative for credits
                        break;
                    case InvoiceType.PurchaseInvoice:
                        transactionType = ContactTransactionType.Invoice;
                        amountSign = -1; // Negative for payables
                        break;
                    case InvoiceType.PurchaseReturn:
                        transactionType = ContactTransactionType.DebitNote;
                        amountSign = 1; // Positive for debits
                        break;
                    default:
                        return;
                }

                // Create contact transaction
                var transaction = new ContactTransaction
                {
                    ContactId = invoice.ContactId,
                    TransactionDate = invoice.InvoiceDate,
                    TransactionType = transactionType,
                    Amount = invoice.Total * amountSign,
                    Reference = invoice.InvoiceNumber,
                    SourceDocumentId = invoice.Id,
                    SourceDocumentType = invoice.Type == InvoiceType.SalesInvoice || invoice.Type == InvoiceType.SalesReturn ? 
                        SourceDocumentType.SalesInvoice : SourceDocumentType.PurchaseInvoice,
                    UserId = invoice.UserId,
                    Notes = invoice.Notes
                };

                ContactService.AddContactTransaction(transaction);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Update contact balance for invoice error: {ex.Message}");
            }
        }

        private static void ReverseContactBalanceForInvoice(Invoice invoice)
        {
            try
            {
                // Determine transaction type and amount sign based on invoice type (opposite of original)
                ContactTransactionType transactionType;
                decimal amountSign;

                switch (invoice.Type)
                {
                    case InvoiceType.SalesInvoice:
                        transactionType = ContactTransactionType.CreditNote;
                        amountSign = -1; // Negative to reverse receivables
                        break;
                    case InvoiceType.SalesReturn:
                        transactionType = ContactTransactionType.Invoice;
                        amountSign = 1; // Positive to reverse credits
                        break;
                    case InvoiceType.PurchaseInvoice:
                        transactionType = ContactTransactionType.DebitNote;
                        amountSign = 1; // Positive to reverse payables
                        break;
                    case InvoiceType.PurchaseReturn:
                        transactionType = ContactTransactionType.Invoice;
                        amountSign = -1; // Negative to reverse debits
                        break;
                    default:
                        return;
                }

                // Create contact transaction
                var transaction = new ContactTransaction
                {
                    ContactId = invoice.ContactId,
                    TransactionDate = DateTime.Now,
                    TransactionType = transactionType,
                    Amount = invoice.Total * amountSign,
                    Reference = $"Reverse {invoice.InvoiceNumber}",
                    SourceDocumentId = invoice.Id,
                    SourceDocumentType = invoice.Type == InvoiceType.SalesInvoice || invoice.Type == InvoiceType.SalesReturn ? 
                        SourceDocumentType.SalesInvoice : SourceDocumentType.PurchaseInvoice,
                    UserId = invoice.UserId,
                    Notes = $"Reversed invoice {invoice.InvoiceNumber}"
                };

                ContactService.AddContactTransaction(transaction);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Reverse contact balance for invoice error: {ex.Message}");
            }
        }
        #endregion
    }
}
