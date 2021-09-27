using CustomerManagement.Data;
using CustomerManagement.Helper;
using CustomerManagement.Models;
using CustomerManagement.RequestModels;
using CustomerManagement.ResponseModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext dbContext;
        private readonly Utility utility;

        public CustomerRepository(AppDbContext dbContext, Utility utility)
        {
            this.dbContext = dbContext;
            this.utility = utility;
        }
        public async Task<GenericResponseModel> CreateCustomerAsync(CustomerRequestModel model, string userId, string createdBy, string profilePicture)
        {
            try
            {
                //save new item
                CustomerProfile customerProfile = new CustomerProfile
                {
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = createdBy,
                    Address = model.Address,
                    CustomerUserId = userId,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    ProfilePicture = profilePicture,
                    UpdatedBy = createdBy
                };
                await dbContext.CustomerProfiles.AddAsync(customerProfile);
                await dbContext.SaveChangesAsync();
                return new GenericResponseModel { StatusCode = 200, StatusMessage = "Customer Created Successfully" };
            }
            catch (Exception ex)
            {
                dbContext.CustomerProfiles.Local.Clear();
                //save error to db
                ErrorLog errorLog = new ErrorLog
                {
                    ErrorDate = DateTime.Now,
                    ErrorMessage = ex.Message,
                    ErrorSource = ex.Source,
                    ErrorStackTrace = ex.StackTrace
                };
                dbContext.ErrorLogs.Add(errorLog);
                dbContext.SaveChanges();
                //save error to file
                utility.SaveErrorMessage(ex);
                return new GenericResponseModel { StatusCode = 500, StatusMessage = ex.Message };
            }
        }

        public async Task<GenericResponseModel> GetAllCustomerAsync()
        {
            try
            {
                var result = await dbContext.CustomerProfiles.Select(x =>
                new
                {
                    x.Address,
                    x.CreatedAt,
                    x.CreatedBy,
                    x.CustomerUserId,
                    x.Email,
                    x.FirstName,
                    x.Id,
                    x.LastName,
                    x.PhoneNumber,
                    x.ProfilePicture,
                    x.UpdatedAt,
                    x.UpdatedBy
                }).ToListAsync();
                //Check if record exist
                if(result.Count > 0)
                {
                    return new GenericResponseModel { StatusCode = 200, StatusMessage = "Success", Data = result };
                }
                return new GenericResponseModel { StatusCode = 404, StatusMessage = "No Record Found" };
            }
            catch (Exception ex)
            {
                //save error to db
                ErrorLog errorLog = new ErrorLog
                {
                    ErrorDate = DateTime.Now,
                    ErrorMessage = ex.Message,
                    ErrorSource = ex.Source,
                    ErrorStackTrace = ex.StackTrace
                };
                dbContext.ErrorLogs.Add(errorLog);
                dbContext.SaveChanges();
                //save error to file
                utility.SaveErrorMessage(ex);
                return new GenericResponseModel { StatusCode = 500, StatusMessage = ex.Message };
            }
        }

        public async Task<GenericResponseModel> GetCustomerByIdAsync(int customerProfileId)
        {
            try
            {
                var result = await dbContext.CustomerProfiles.Where(x=>x.Id == customerProfileId).Select(x =>
                new
                {
                    x.Address,
                    x.CreatedAt,
                    x.CreatedBy,
                    x.CustomerUserId,
                    x.Email,
                    x.FirstName,
                    x.Id,
                    x.LastName,
                    x.PhoneNumber,
                    x.ProfilePicture,
                    x.UpdatedAt,
                    x.UpdatedBy
                }).FirstOrDefaultAsync();
                //Check if record exist
                if (result != null)
                {
                    return new GenericResponseModel { StatusCode = 200, StatusMessage = "Success", Data = result };
                }
                return new GenericResponseModel { StatusCode = 404, StatusMessage = "No Record Found" };
            }
            catch (Exception ex)
            {
                //save error to db
                ErrorLog errorLog = new ErrorLog
                {
                    ErrorDate = DateTime.Now,
                    ErrorMessage = ex.Message,
                    ErrorSource = ex.Source,
                    ErrorStackTrace = ex.StackTrace
                };
                dbContext.ErrorLogs.Add(errorLog);
                dbContext.SaveChanges();
                //save error to file
                utility.SaveErrorMessage(ex);
                return new GenericResponseModel { StatusCode = 500, StatusMessage = ex.Message };
            }
        }
    }
}
