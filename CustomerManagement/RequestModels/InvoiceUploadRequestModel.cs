using CustomerManagement.Helper.Image;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.RequestModels
{
    public class InvoiceUploadRequestModel
    {
        [Required]
        public string Code { get; set; }
        [Required]
        [MaxFileSize(1 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".gif", ".jpeg" })]
        public IFormFile ProfilePicture { get; set; }
    }
}
