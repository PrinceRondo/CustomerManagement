using CustomerManagement.Repositories;
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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }
        [HttpGet("allCustomer")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllCustomer()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await customerRepository.GetAllCustomerAsync();

            return Ok(result);
        }
        [HttpGet("customerDetail")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetCustomerById(int customerProfileId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await customerRepository.GetCustomerByIdAsync(customerProfileId);

            return Ok(result);
        }
    }
}
