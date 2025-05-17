using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingApp.Models
{
    [Table("CompanySettings")]
    public class CompanySettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Website { get; set; }

        [StringLength(20)]
        public string TaxNumber { get; set; }

        [StringLength(255)]
        public string LogoPath { get; set; }

        [Required]
        [StringLength(10)]
        public string CurrencyCode { get; set; } = "USD";

        [Required]
        [StringLength(10)]
        public string CurrencySymbol { get; set; } = "$";

        [Required]
        public bool ShowCurrencySymbolBefore { get; set; } = true;

        [Required]
        public int DecimalPlaces { get; set; } = 2;

        [Required]
        public decimal DefaultTaxRate { get; set; } = 0;

        [Required]
        public string DefaultLanguage { get; set; } = "en-US";

        public DateTime FiscalYearStart { get; set; } = new DateTime(DateTime.Now.Year, 1, 1);

        public DateTime? LastBackupDate { get; set; }
    }

    [Table("SystemSettings")]
    public class SystemSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string SettingKey { get; set; }

        [Required]
        [StringLength(255)]
        public string SettingValue { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public bool IsEditable { get; set; } = true;
    }

    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; }

        [Required]
        [StringLength(50)]
        public string EntityType { get; set; }

        public int? EntityId { get; set; }

        [StringLength(255)]
        public string Details { get; set; }

        [StringLength(50)]
        public string IpAddress { get; set; }
    }

    [Table("Notifications")]
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Message { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        [StringLength(50)]
        public string NotificationType { get; set; }

        public int? ReferenceId { get; set; }

        [StringLength(50)]
        public string ReferenceType { get; set; }
    }
}
