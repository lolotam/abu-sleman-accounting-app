using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingApp.Models
{
    public enum InvoiceType
    {
        SalesInvoice,
        SalesReturn,
        PurchaseInvoice,
        PurchaseReturn
    }

    public enum InvoiceStatus
    {
        Draft,
        Confirmed,
        Paid,
        PartiallyPaid,
        Cancelled
    }

    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        BankTransfer,
        Check,
        Credit
    }

    [Table("Invoices")]
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string InvoiceNumber { get; set; }

        [Required]
        public InvoiceType Type { get; set; }

        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Required]
        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual Warehouse Warehouse { get; set; }

        [StringLength(255)]
        public string Reference { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DueAmount { get; set; }

        [Required]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime? DueDate { get; set; }

        [StringLength(255)]
        public string Notes { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<InvoiceItem> Items { get; set; }
        public virtual ICollection<InvoicePayment> Payments { get; set; }
    }

    [Table("InvoiceItems")]
    public class InvoiceItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercentage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxPercentage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
    }

    [Table("InvoicePayments")]
    public class InvoicePayment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(50)]
        public string ReferenceNumber { get; set; }

        [StringLength(255)]
        public string Notes { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
