using AccountingApp.Database;
using AccountingApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace AccountingApp.Services
{
    public class ProductService
    {
        #region Products
        public static List<Product> GetAllProducts()
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Products
                        .Include(p => p.Category)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get all products error: {ex.Message}");
                return new List<Product>();
            }
        }

        public static Product GetProductById(int id)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Products
                        .Include(p => p.Category)
                        .FirstOrDefault(p => p.Id == id);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get product by id error: {ex.Message}");
                return null;
            }
        }

        public static Product GetProductByCode(string code)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Products
                        .Include(p => p.Category)
                        .FirstOrDefault(p => p.Code == code);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get product by code error: {ex.Message}");
                return null;
            }
        }

        public static Product GetProductByBarcode(string barcode)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Products
                        .Include(p => p.Category)
                        .FirstOrDefault(p => p.Barcode == barcode);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get product by barcode error: {ex.Message}");
                return null;
            }
        }

        public static bool CreateProduct(Product product)
        {
            if (product == null)
                return false;

            try
            {
                using (var context = new AccountingDbContext())
                {
                    // Check if code already exists
                    if (context.Products.Any(p => p.Code == product.Code))
                        return false;

                    // Check if barcode already exists (if provided)
                    if (!string.IsNullOrEmpty(product.Barcode) && 
                        context.Products.Any(p => p.Barcode == product.Barcode))
                        return false;

                    context.Products.Add(product);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Create product error: {ex.Message}");
                return false;
            }
        }

        public static bool UpdateProduct(Product product)
        {
            if (product == null)
                return false;

            try
            {
                using (var context = new AccountingDbContext())
                {
                    // Check if code already exists for another product
                    if (context.Products.Any(p => p.Code == product.Code && p.Id != product.Id))
                        return false;

                    // Check if barcode already exists for another product (if provided)
                    if (!string.IsNullOrEmpty(product.Barcode) && 
                        context.Products.Any(p => p.Barcode == product.Barcode && p.Id != product.Id))
                        return false;

                    product.UpdatedAt = DateTime.Now;
                    context.Entry(product).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Update product error: {ex.Message}");
                return false;
            }
        }

        public static bool DeleteProduct(int id)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    var product = context.Products.Find(id);
                    if (product == null)
                        return false;

                    // Don't actually delete, just mark as inactive
                    product.IsActive = false;
                    product.UpdatedAt = DateTime.Now;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Delete product error: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Inventory
        public static decimal GetProductStock(int productId, int warehouseId)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    var inventory = context.Inventory
                        .FirstOrDefault(i => i.ProductId == productId && i.WarehouseId == warehouseId);

                    return inventory?.Quantity ?? 0;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get product stock error: {ex.Message}");
                return 0;
            }
        }

        public static decimal GetTotalProductStock(int productId)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    return context.Inventory
                        .Where(i => i.ProductId == productId)
                        .Sum(i => i.Quantity);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Get total product stock error: {ex.Message}");
                return 0;
            }
        }

        public static bool UpdateInventory(int productId, int warehouseId, decimal quantity, 
            InventoryTransactionType transactionType, string reference, int userId, 
            int? sourceDocumentId = null, SourceDocumentType? sourceDocumentType = null)
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    // Get current inventory
                    var inventory = context.Inventory
                        .FirstOrDefault(i => i.ProductId == productId && i.WarehouseId == warehouseId);

                    if (inventory == null)
                    {
                        // Create new inventory record if it doesn't exist
                        inventory = new Inventory
                        {
                            ProductId = productId,
                            WarehouseId = warehouseId,
                            Quantity = 0,
                            LastUpdated = DateTime.Now
                        };
                        context.Inventory.Add(inventory);
                    }

                    // Update quantity based on transaction type
                    switch (transactionType)
                    {
                        case InventoryTransactionType.Purchase:
                        case InventoryTransactionType.Return:
                        case InventoryTransactionType.Initial:
                            inventory.Quantity += quantity;
                            break;
                        case InventoryTransactionType.Sale:
                            inventory.Quantity -= quantity;
                            break;
                        case InventoryTransactionType.Adjustment:
                            inventory.Quantity = quantity; // Direct adjustment
                            break;
                    }

                    inventory.LastUpdated = DateTime.Now;

                    // Record transaction
                    var transaction = new InventoryTransaction
                    {
                        ProductId = productId,
                        WarehouseId = warehouseId,
                        Quantity = quantity,
                        TransactionType = transactionType,
                        Reference = reference,
                        SourceDocumentId = sourceDocumentId,
                        SourceDocumentType = sourceDocumentType,
                        TransactionDate = DateTime.Now,
                        UserId = userId
                    };

                    context.InventoryTransactions.Add(transaction);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Update inventory error: {ex.Message}");
                return false;
            }
        }
        #endregion
    }
}
