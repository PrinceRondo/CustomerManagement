using CustomerManagement.Repositories;
using CustomerManagement.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository invoiceRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository)
        {
            this.invoiceRepository = invoiceRepository;
        }

        [HttpPost("createInvoiceUrl")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateInvoiceUrl(int customerProfileId)
        {
            var curUser = HttpContext.User;
            if (curUser.HasClaim(c => c.Type == "UserId"))
            {
                var usid = Convert.ToString(curUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var result = await invoiceRepository.CreateInvoiceUrlAsync(customerProfileId, usid);
                return Ok(result);
            }
            else
            {
                return Ok(new
                {
                    Status = "002",
                    Message = "Invalid session, logout and try again"
                });
            }
        }
        [HttpPut("uploadInvoice")]
        public async Task<IActionResult> UploadInvoice([FromForm] InvoiceUploadRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var verifyCode = await invoiceRepository.VerifyCodeAsync(model.Code);
            //if code is ok
            if (verifyCode.StatusCode == 200)
            {
                //Manage profile picture
                string invoicePath = string.Empty;
                if (model.ProfilePicture != null)
                {
                    //save profile picture
                    string filename = "";
                    IFormFile file;
                    file = model.ProfilePicture;
                    filename = model.Code + file.FileName.Replace(" ", "_");
                    string extension = Path.GetExtension(filename);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Invoice", filename);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                    invoicePath = "Invoice/" + filename;
                }

                var result = await invoiceRepository.UploadInvoiceAsync(model.Code, invoicePath);

                return Ok(result);
            }
            return Ok(verifyCode);
        }
    }
}
