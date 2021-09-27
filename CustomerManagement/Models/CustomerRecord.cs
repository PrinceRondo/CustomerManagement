using CustomerManagement.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.Models
{
    public class CustomerRecord
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CustomerProfileId { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }


        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual ApplicationUser ApplicationUser1 { get; set; }

        [ForeignKey("CustomerProfileId")]
        public virtual CustomerProfile CustomerProfile { get; set; }
    }
}
