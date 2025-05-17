using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingApp.Models
{
    public enum ContactType
    {
        Customer,
        Supplier,
        Both
    }

    [Table("Contacts")]
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public ContactType Type { get; set; }

        [StringLength(100)]
        public string ContactPerson { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [StringLength(20)]
        public string TaxNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OpeningBalance { get; set; }

        public DateTime OpeningBalanceDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditLimit { get; set; }

        public int? PaymentTerms { get; set; }

        [StringLength(255)]
        public string Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }

    [Table("ContactTransactions")]
    public class ContactTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        public ContactTransactionType TransactionType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string Reference { get; set; }

        public int? SourceDocumentId { get; set; }

        public SourceDocumentType? SourceDocumentType { get; set; }

        [StringLength(255)]
        public string Notes { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

    public enum ContactTransactionType
    {
        Invoice,
        Payment,
        CreditNote,
        DebitNote,
        OpeningBalance
    }
}
