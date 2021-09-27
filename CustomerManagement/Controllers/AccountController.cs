using CustomerManagement.Data;
using CustomerManagement.Helper;
using CustomerManagement.Helper.Mail;
using CustomerManagement.Repositories;
using CustomerManagement.RequestModels;
using CustomerManagement.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly EmailConfiguration emailConfig;
        private readonly Helper.Utility utility;
        private readonly AppDbContext dbContext;
        private readonly ICustomerRepository customerRepository;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration,
            EmailConfiguration emailConfig, Helper.Utility utility, AppDbContext dbContext, ICustomerRepository customerRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._configuration = configuration;
            this.emailConfig = emailConfig;
            this.utility = utility;
            this.dbContext = dbContext;
            this.customerRepository = customerRepository;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {

                var user = await userManager.FindByNameAsync(model.Username);
                if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (!await userManager.IsLockedOutAsync(user))
                    {
                        var userRoles = await userManager.GetRolesAsync(user);
                        var frole = userRoles[0];

                        var authClaims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                                new Claim("UserId", user.Id),
                            };
                        foreach (var userRole in userRoles)
                        {
                            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                        }

                        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                        var token = new JwtSecurityToken(
                            issuer: _configuration["JWT:ValidIssuer"],
                            audience: _configuration["JWT:ValidAudience"],
                            expires: DateTime.Now.AddMinutes(60),
                            claims: authClaims,
                            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                            );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                            Status = "Success",
                            Message = "Logged in Successfully",
                            username = model.Username,
                            userId = user.Id,
                            role = frole
                        });
                    }
                    else
                    {
                        return Ok(new
                        {
                            token = "",
                            expiration = "",
                            Status = "Locked",
                            Message = "Your account has been locked, kindly contact the Admin"
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        token = "",
                        expiration = "",
                        Status = "Failed",
                        Message = "Invalid Username or Password"
                    });
                }
            }
            catch (Exception ex)
            {
                utility.SaveErrorMessage(ex);
                return Ok(new GenericResponseModel { StatusCode = 405, StatusMessage = "Login failed: " + ex.Message });
            }
        }

        [HttpPost("createCustomer")]
        [Authorize(Roles ="admin")]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> RegisterUser([FromForm] CustomerRequestModel model)
        {
            try
            {
                var curUser = HttpContext.User;
                if (curUser.HasClaim(c => c.Type == "UserId"))
                {
                    var usid = Convert.ToString(curUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

                    if (ModelState.IsValid)
                    {
                        var userExists = await userManager.FindByNameAsync(model.Email);
                        if (userExists != null)
                            return StatusCode(StatusCodes.Status409Conflict, new GenericResponseModel { StatusCode = 409, StatusMessage = "User already exists!" });

                        ApplicationUser user = new ApplicationUser()
                        {
                            Email = model.Email,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            UserName = model.Email,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            LockoutEnd = DateTime.MaxValue,
                            LockoutEnabled = true,
                            PhoneNumber = model.PhoneNumber
                        };
                        //Craete user account with default password
                        var result = await userManager.CreateAsync(user, "FU456OPraHunter#");
                        if (!result.Succeeded)
                            return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponseModel { StatusCode = 401, StatusMessage = "User creation failed! Please check user details and try again." });
                        await userManager.AddToRoleAsync(user, "customer");

                        //Manage profile picture
                        string profilePicturePath = string.Empty;
                        if (model.ProfilePicture != null)
                        {
                            //save profile picture
                            string filename = "";
                            IFormFile file;
                            file = model.ProfilePicture;
                            filename = Guid.NewGuid() + file.FileName.Replace(" ", "_");
                            string extension = Path.GetExtension(filename);
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ProfilePicture", filename);
                            var stream = new FileStream(path, FileMode.Create);
                            await file.CopyToAsync(stream);
                            profilePicturePath = "ProfilePicture/" + filename;
                        }
                        //Save user profile
                        var response = await customerRepository.CreateCustomerAsync(model, user.Id, usid, profilePicturePath);

                        var token = await userManager.GeneratePasswordResetTokenAsync(user);
                        //Implement a email sender here.. send reset password link to the user
                        Mailer mail = new Mailer(emailConfig, utility, dbContext);
                        mail.SendMail(_configuration["EmailConfiguration:Appurl"] + "change-password?token=" + token + "&email=" + model.Email, model.Email, "Customer Registration", model.FirstName );

                        if (response.StatusCode == 200)
                        {
                            return Ok(new GenericResponseModel { StatusCode = 200, StatusMessage = "User created successfully and onboarding email sent to the email address provided." });
                        }
                        else
                        {
                            return Ok(new GenericResponseModel { StatusCode = 200, StatusMessage = "User created successfully and onboarding email sent to the email address provided. Failed to create user profile, kindly contact the admin" });
                        }
                    }
                    else
                    {
                        return BadRequest(ModelState);
                    }
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
            catch (Exception ex)
            {
                utility.SaveErrorMessage(ex);
                return Ok(new GenericResponseModel { StatusCode = 405, StatusMessage = "User creation failed: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model)
        {
            try
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    var resetPassResult = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (resetPassResult.Succeeded)
                    {
                        var accountupd = await userManager.SetLockoutEndDateAsync(user, DateTime.Now);
                        if (accountupd.Succeeded)
                        {
                            return Ok(new
                            {
                                Status = "Success",
                                Message = "Password Changed Successfully"
                            });
                        }
                        else
                        {
                            return Ok(new
                            {
                                Status = "Failed",
                                Message = "Password change was successful but unable to activate account, contact admin."
                            });
                        }

                    }
                    else
                    {
                        string ecode = "";
                        string edescription = "";
                        foreach (var error in resetPassResult.Errors)
                        {
                            ModelState.TryAddModelError(error.Code, error.Description);
                            ecode = error.Code;
                            edescription = error.Description;
                        }
                        return Ok(new
                        {
                            Status = "Failed",
                            Message = "Password change failed. " + ecode + " :: " + edescription
                        });
                    }
                }
                else
                {
                    return Ok(new
                    {
                        Status = "Failed",
                        Message = "Invalid User account specified"
                    });
                }
            }
            catch (Exception ex)
            {
                utility.SaveErrorMessage(ex);
                return Ok(new GenericResponseModel { StatusCode = 405, StatusMessage = "Exception in changepassword: " + ex.Message });
            }
        }
    }
}
