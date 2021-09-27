using CustomerManagement.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.Models
{
    public class CustomerProfile
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CustomerUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ProfilePicture { get; set; }
        public string Email { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }


        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual ApplicationUser ApplicationUser1 { get; set; }

        [ForeignKey("CustomerUserId")]
        public virtual ApplicationUser ApplicationUser2 { get; set; }

    }
}
