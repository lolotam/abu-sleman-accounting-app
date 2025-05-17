using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingApp.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(50)]
        public string Barcode { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumStock { get; set; }

        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual ProductCategory Category { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [StringLength(255)]
        public string ImagePath { get; set; }
    }

    [Table("ProductCategories")]
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    [Table("Inventory")]
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    [Table("Warehouses")]
    public class Warehouse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Location { get; set; }

        [StringLength(50)]
        public string Manager { get; set; }

        public bool IsActive { get; set; } = true;
    }

    [Table("InventoryTransactions")]
    public class InventoryTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Required]
        public InventoryTransactionType TransactionType { get; set; }

        [StringLength(255)]
        public string Reference { get; set; }

        public int? SourceDocumentId { get; set; }

        public SourceDocumentType? SourceDocumentType { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [StringLength(255)]
        public string Notes { get; set; }
    }

    public enum InventoryTransactionType
    {
        Purchase,
        Sale,
        Return,
        Adjustment,
        Transfer,
        Initial
    }

    public enum SourceDocumentType
    {
        PurchaseInvoice,
        SalesInvoice,
        PurchaseReturn,
        SalesReturn,
        InventoryAdjustment,
        InventoryTransfer
    }
}
