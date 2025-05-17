using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingApp.Models
{
    [Table("AccountGroups")]
    public class AccountGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual AccountGroup Parent { get; set; }

        public virtual ICollection<AccountGroup> Children { get; set; }

        public AccountType Type { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    [Table("ChartOfAccounts")]
    public class Account
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
        public AccountType Type { get; set; }

        public int? GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual AccountGroup Group { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OpeningBalance { get; set; }

        public DateTime OpeningBalanceDate { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsCashOrBank { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }

    public enum AccountType
    {
        Asset,
        Liability,
        Equity,
        Revenue,
        Expense
    }

    [Table("JournalEntries")]
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string EntryNumber { get; set; }

        [Required]
        public DateTime EntryDate { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string Reference { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public int? SourceDocumentId { get; set; }

        public SourceDocumentType? SourceDocumentType { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public bool IsPosted { get; set; }

        public DateTime? PostedDate { get; set; }

        public virtual ICollection<JournalEntryLine> Lines { get; set; }
    }

    [Table("JournalEntryLines")]
    public class JournalEntryLine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JournalEntryId { get; set; }

        [ForeignKey("JournalEntryId")]
        public virtual JournalEntry JournalEntry { get; set; }

        [Required]
        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DebitAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CreditAmount { get; set; }

        public int? ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }
    }

    [Table("FiscalYears")]
    public class FiscalYear
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsClosed { get; set; }

        public DateTime? ClosedDate { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }

    [Table("FiscalPeriods")]
    public class FiscalPeriod
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FiscalYearId { get; set; }

        [ForeignKey("FiscalYearId")]
        public virtual FiscalYear FiscalYear { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsClosed { get; set; }

        public DateTime? ClosedDate { get; set; }
    }
}
