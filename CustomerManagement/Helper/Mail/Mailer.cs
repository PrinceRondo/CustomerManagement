using MimeKit;
using System;
using System.Linq;
using MailKit.Net.Smtp;
using CustomerManagement.Helper;
using CustomerManagement.Helper.Mail;
using CustomerManagement.Data;
using CustomerManagement.Models;

namespace CustomerManagement.Helper.Mail
{
    public class Mailer
    {
        private readonly EmailConfiguration emailConfig;
        private readonly Utility utility;
        private readonly AppDbContext dbContext;

        public Mailer(EmailConfiguration emailConfig, Utility utility, AppDbContext dbContext)
        {
            this.emailConfig = emailConfig;
            this.utility = utility;
            this.dbContext = dbContext;
        }
        public void SendMail(string link, string mailTo, string subject, string name)
        {
            try
            {
                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("Customer Manager", emailConfig.From);
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress(name, mailTo);
                message.To.Add(to);

                message.Subject = "Customer Management Registration";

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = "<h1>Customer Management Email Confirmation</h1><br/><h3>Click <a href=" + link + ">here</a> to change your password</h3>";
                //bodyBuilder.TextBody = "Click this link " + link + " to confirm your email";

                //bodyBuilder.Attachments.Add(path);
                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new SmtpClient();

                client.Connect(emailConfig.SmtpServer, Convert.ToInt16(emailConfig.Port), emailConfig.EnableSSL);
                client.Authenticate(emailConfig.UserName, emailConfig.Password);

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
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
            }
        }

        public void SendInvoice(string name, string email, string body, string path)
        {

            try
            {
                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress("Customer Manager", emailConfig.From);
                message.From.Add(from);

                //MailboxAddress to = new MailboxAddress(hnenrolleePayment.DepositorFirstName,
                //hnenrolleePayment.Email);
                MailboxAddress to = new MailboxAddress(name,
                email);
                message.To.Add(to);

                message.Subject = "Customer Records";

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = "<h3>" + body + "</h3>";

                bodyBuilder.Attachments.Add(path);
                message.Body = bodyBuilder.ToMessageBody();

                SmtpClient client = new SmtpClient();
                client.Connect(emailConfig.SmtpServer, Convert.ToInt16(emailConfig.Port), emailConfig.EnableSSL);
                client.Authenticate(emailConfig.UserName, emailConfig.Password);

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();

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
            }
        }
    }
}
