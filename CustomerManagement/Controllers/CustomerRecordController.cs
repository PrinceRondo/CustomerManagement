using CustomerManagement.Repositories;
using CustomerManagement.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerRecordController : ControllerBase
    {
        private readonly ICustomerRecordRepository customerRecordRepository;

        public CustomerRecordController(ICustomerRecordRepository customerRecordRepository)
        {
            this.customerRecordRepository = customerRecordRepository;
        }

        [HttpPost("createCustomerRecord")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateCustomerRecord([FromBody]CustomerRecordRequestModel model)
        {
            var curUser = HttpContext.User;
            if (curUser.HasClaim(c => c.Type == "UserId"))
            {
                var usid = Convert.ToString(curUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var result = await customerRecordRepository.CreateCustomerRecordAsync(model, usid);
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
        [HttpPut("updateCustomerRecord")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCustomerRecord(int customerRecordId, [FromBody]CustomerRecordRequestModel model)
        {
            var curUser = HttpContext.User;
            if (curUser.HasClaim(c => c.Type == "UserId"))
            {
                var usid = Convert.ToString(curUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var result = await customerRecordRepository.UpdateCustomerRecordAsync(customerRecordId, model, usid);
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
        [HttpDelete("deleteCustomerRecord")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCustomerRecord(int customerRecordId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await customerRecordRepository.DeleteCustomerRecordAsync(customerRecordId);

            return Ok(result);
        }
        [HttpGet("allCustomerRecord")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllCustomerRecord()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await customerRecordRepository.GetAllCustomerRecordAsync();

            return Ok(result);
        }
        [HttpGet("allCustomerRecordByCustomerId")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllCustomerRecordByCustomerId(int customerProfileId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await customerRecordRepository.GetAllCustomerRecordByCustomerIdAsync(customerProfileId);

            return Ok(result);
        }
        [HttpGet("customerRecordDetails")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetCustomerRecordByIdAsync(int customerRecordId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await customerRecordRepository.GetCustomerRecordByIdAsync(customerRecordId);

            return Ok(result);
        }
        [HttpPost("sendAllCustomerRecord")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SendAllCustomerRecord(int customerProfileId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await customerRecordRepository.SendAllCustomerRecordAsync(customerProfileId);

            return Ok(result);
        }
    }
}
