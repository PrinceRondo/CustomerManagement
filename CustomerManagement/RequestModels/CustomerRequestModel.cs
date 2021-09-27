using CustomerManagement.Helper.Image;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.RequestModels
{
    public class CustomerRequestModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }
        [MaxFileSize(1 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".gif", ".jpeg" })]
        public IFormFile ProfilePicture { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}
