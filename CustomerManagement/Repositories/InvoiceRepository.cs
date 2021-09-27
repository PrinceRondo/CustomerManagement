using CustomerManagement.Data;
using CustomerManagement.Helper;
using CustomerManagement.Models;
using CustomerManagement.ResponseModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext dbContext;
        private readonly Utility utility;

        public InvoiceRepository(AppDbContext dbContext, Utility utility)
        {
            this.dbContext = dbContext;
            this.utility = utility;
        }

        public async Task<GenericResponseModel> CreateInvoiceUrlAsync(int customerProfileId, string createdBy)
        {
            try
            {
                //Check if customer exist 
                var checkCustomer = await dbContext.CustomerProfiles.Where(x => x.Id == customerProfileId).FirstOrDefaultAsync();
                if (checkCustomer == null)
                {
                    return new GenericResponseModel { StatusCode = 300, StatusMessage = "Invalid customer seleted" };
                }
                string code = Guid.NewGuid().ToString();
                string url = "http://customermanagement.com/uploadinvoice?code" + code;
                //save new item
                InvoiceUrl invoiceUrl = new InvoiceUrl
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy,
                    CustomerProfileId = customerProfileId,
                    Code = code,
                    Email = checkCustomer.Email,
                    IsUrlUsed = false,
                    IsSent = false,
                    Url = url
                };
                await dbContext.InvoiceUrls.AddAsync(invoiceUrl);
                await dbContext.SaveChangesAsync();
                return new GenericResponseModel { StatusCode = 200, StatusMessage = "Customer Invoice Url Created Successfully" };
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

        public async Task<GenericResponseModel> UploadInvoiceAsync(string code, string invoicePath)
        {
            try
            {
                //Check if code exist and update the record
                var getRecord = await dbContext.InvoiceUrls.Where(x => x.Code == code).FirstOrDefaultAsync();
                getRecord.UploadedInvoicePath = invoicePath;
                getRecord.IsUrlUsed = true;
                await dbContext.SaveChangesAsync();

                return new GenericResponseModel { StatusCode = 200, StatusMessage = "Record updated successfully" };
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

        public async Task<GenericResponseModel> VerifyCodeAsync(string code)
        {
            try
            {
                //Check if code exist 
                var checkCode = await dbContext.InvoiceUrls.Where(x => x.Code == code).FirstOrDefaultAsync();
                if (checkCode == null)
                {
                    return new GenericResponseModel { StatusCode = 300, StatusMessage = "Invalid Code" };
                }
                if (checkCode.IsSent == false)
                {
                    return new GenericResponseModel { StatusCode = 301, StatusMessage = "This is a fraud" };
                }
                if (checkCode.IsSent == true && DateTime.Now > checkCode.ExpiredAt)
                {
                    return new GenericResponseModel { StatusCode = 302, StatusMessage = "Code expired" };
                }
                if (checkCode.IsUrlUsed == true)
                {
                    return new GenericResponseModel { StatusCode = 303, StatusMessage = "Code has already been used" };
                }

                return new GenericResponseModel { StatusCode = 200, StatusMessage = "Code verified" };
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
