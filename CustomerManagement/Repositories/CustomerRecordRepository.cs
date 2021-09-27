using CustomerManagement.Data;
using CustomerManagement.Helper;
using CustomerManagement.Helper.Mail;
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
    public class CustomerRecordRepository : ICustomerRecordRepository
    {
        private readonly AppDbContext dbContext;
        private readonly Utility utility;
        private readonly Mailer mailer;

        public CustomerRecordRepository(AppDbContext dbContext, Utility utility, Mailer mailer)
        {
            this.dbContext = dbContext;
            this.utility = utility;
            this.mailer = mailer;
        }

        public async Task<GenericResponseModel> CreateCustomerRecordAsync(CustomerRecordRequestModel model, string createdBy)
        {
            try
            {
                //Check if customer exist 
                var checkCustomer = await dbContext.CustomerProfiles.Where(x => x.Id == model.CustomerProfileId).FirstOrDefaultAsync();
                if(checkCustomer == null)
                {
                    return new GenericResponseModel { StatusCode = 300, StatusMessage = "Invalid customer seleted" };
                }
                //save new item
                CustomerRecord customerRecord = new CustomerRecord
                {
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = createdBy,
                    CustomerProfileId = model.CustomerProfileId,
                    Note = model.Note,
                    UpdatedBy = createdBy
                };
                await dbContext.CustomerRecords.AddAsync(customerRecord);
                await dbContext.SaveChangesAsync();
                return new GenericResponseModel { StatusCode = 200, StatusMessage = "Customer Record Created Successfully" };
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

        public async Task<GenericResponseModel> DeleteCustomerRecordAsync(int customerRecordId)
        {
            try
            {
                //check if record exist
                var record = await dbContext.CustomerRecords.Where(x => x.Id == customerRecordId).FirstOrDefaultAsync();
                if (record != null)
                {
                    dbContext.CustomerRecords.Remove(record);
                    await dbContext.SaveChangesAsync();
                    return new GenericResponseModel { StatusCode = 200, StatusMessage = "Record deleted successfully" };
                }
                return new GenericResponseModel { StatusCode = 404, StatusMessage = "Record not found" };
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
                return new GenericResponseModel
                {
                    StatusCode = 500,
                    StatusMessage = ex.Message
                };
            }
        }

        public async Task<GenericResponseModel> GetAllCustomerRecordAsync()
        {
            try
            {
                var result = await dbContext.CustomerRecords.Select(x =>
                new
                {
                    x.CustomerProfileId,
                    x.CreatedAt,
                    x.CreatedBy,
                    x.Id,
                    x.Note,
                    x.UpdatedAt,
                    x.UpdatedBy
                }).ToListAsync();
                //Check if record exist
                if (result.Count > 0)
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

        public async Task<GenericResponseModel> GetAllCustomerRecordByCustomerIdAsync(int customerProfileId)
        {
            try
            {
                var result = await dbContext.CustomerRecords.Where(x=>x.CustomerProfileId == customerProfileId).Select(x =>
                new
                {
                    x.CustomerProfileId,
                    x.CreatedAt,
                    x.CreatedBy,
                    x.Id,
                    x.Note,
                    x.UpdatedAt,
                    x.UpdatedBy
                }).ToListAsync();
                //Check if record exist
                if (result.Count > 0)
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

        public async Task<GenericResponseModel> GetCustomerRecordByIdAsync(int customerRecordId)
        {
            try
            {
                var result = await dbContext.CustomerRecords.Where(x => x.CustomerProfileId == customerRecordId).Select(x =>
                  new
                  {
                      x.CustomerProfileId,
                      x.CreatedAt,
                      x.CreatedBy,
                      x.Id,
                      x.Note,
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

        public async Task<GenericResponseModel> SendAllCustomerRecordAsync(int customerProfileId)
        {
            try
            {
                //check if customer exist
                var record = await dbContext.CustomerProfiles.Where(x => x.Id == customerProfileId).FirstOrDefaultAsync();
                if (record != null)
                {
                    var customerRecords = await dbContext.CustomerRecords.Where(x => x.CustomerProfileId == customerProfileId).ToListAsync();
                    if (customerRecords.Count > 0)
                    {
                        PdfDocument pdfDocument = new PdfDocument(dbContext, mailer, utility);
                        //Genrate and send document
                        pdfDocument.GeneratePdfFile(record, customerRecords);
                        return new GenericResponseModel { StatusCode = 200, StatusMessage = "Record sent successfully" };
                    }
                    return new GenericResponseModel { StatusCode = 403, StatusMessage = "No record for seleted customer" };
                }
                return new GenericResponseModel { StatusCode = 404, StatusMessage = "Customer Profile not found" };
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
                return new GenericResponseModel
                {
                    StatusCode = 500,
                    StatusMessage = ex.Message
                };
            }
        }

        public async Task<GenericResponseModel> UpdateCustomerRecordAsync(int customerRecordId, CustomerRecordRequestModel model, string updatedBy)
        {
            try
            {
                //check if record exist
                var record = await dbContext.CustomerRecords.Where(x => x.Id == customerRecordId).FirstOrDefaultAsync();
                if (record != null)
                {
                    //Check if customer exist 
                    var checkCustomer = await dbContext.CustomerProfiles.Where(x => x.Id == model.CustomerProfileId).FirstOrDefaultAsync();
                    if (checkCustomer == null)
                    {
                        return new GenericResponseModel { StatusCode = 300, StatusMessage = "Invalid customer seleted" };
                    }

                    record.CustomerProfileId = model.CustomerProfileId;
                    record.Note = model.Note;
                    record.UpdatedAt = DateTime.Now;
                    record.UpdatedBy = updatedBy;
                    await dbContext.SaveChangesAsync();
                    return new GenericResponseModel { StatusCode = 200, StatusMessage = "Record updated successfully" };
                }
                return new GenericResponseModel { StatusCode = 404, StatusMessage = "Record not found" };
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
                return new GenericResponseModel
                {
                    StatusCode = 500,
                    StatusMessage = ex.Message
                };
            }
        }
    }
}
