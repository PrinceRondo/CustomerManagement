using CustomerManagement.RequestModels;
using CustomerManagement.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.Repositories
{
    public interface ICustomerRepository
    {
        Task<GenericResponseModel> CreateCustomerAsync(CustomerRequestModel model, string userId, string createdBy, string profilePicture);
        Task<GenericResponseModel> GetAllCustomerAsync();
        Task<GenericResponseModel> GetCustomerByIdAsync(int customerProfileId);
    }
    public interface ICustomerRecordRepository
    {
        Task<GenericResponseModel> CreateCustomerRecordAsync(CustomerRecordRequestModel model, string createdBy);
        Task<GenericResponseModel> UpdateCustomerRecordAsync(int customerRecordId, CustomerRecordRequestModel model, string updatedBy);
        Task<GenericResponseModel> GetAllCustomerRecordAsync();
        Task<GenericResponseModel> GetAllCustomerRecordByCustomerIdAsync(int customerProfileId);
        Task<GenericResponseModel> GetCustomerRecordByIdAsync(int customerRecordId);
        Task<GenericResponseModel> DeleteCustomerRecordAsync(int customerRecordId);
        Task<GenericResponseModel> SendAllCustomerRecordAsync(int customerProfileId);
    }
    public interface IInvoiceRepository
    {
        Task<GenericResponseModel> CreateInvoiceUrlAsync(int customerProfileId, string createdBy);
        Task<GenericResponseModel> VerifyCodeAsync(string code);
        Task<GenericResponseModel> UploadInvoiceAsync(string code, string invoicePath);
    }

}
