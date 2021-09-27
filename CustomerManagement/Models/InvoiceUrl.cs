using CustomerManagement.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.Models
{
    public class InvoiceUrl
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerProfileId { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public bool IsUrlUsed { get; set; }
        public string UploadedInvoicePath { get; set; }
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool IsSent { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ExpiredAt { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("CustomerProfileId")]
        public virtual CustomerProfile CustomerProfile { get; set; }
    }
}
