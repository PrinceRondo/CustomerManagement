using CustomerManagement.Data;
using CustomerManagement.Helper.Mail;
using CustomerManagement.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.Helper
{
    public class PdfDocument
    {
        private readonly AppDbContext dbContext;
        private readonly Mailer mailer;
        private readonly Utility utility;
        private Font blackFnt = FontFactory.GetFont("Arial", 8, new BaseColor(0, 0, 0));
        private static Font blackFntB = FontFactory.GetFont("Arial", 8, Font.BOLD, new BaseColor(0, 0, 0));

        public PdfDocument(AppDbContext dbContext, Mailer mailer, Utility utility)
        {
            this.dbContext = dbContext;
            this.mailer = mailer;
            this.utility = utility;
        }

        public void GeneratePdfFile(CustomerProfile customerProfile, IList<CustomerRecord> customerRecord)
        {
            try
            {
                if (!Directory.Exists("wwwroot/Records/"))
                {
                    Directory.CreateDirectory("wwwroot/Records/");
                }
                string path = "wwwroot/Records/" + Guid.NewGuid() + ".pdf";
                FileStream fs = new FileStream(path, FileMode.Create);

                Document document = new Document();    // instantiate a iTextSharp.text.pdf.Document
                document.SetMargins(80f, 80f, 30f, 0f);
                MemoryStream mem = new MemoryStream(); // PDF data will be written here
                PdfWriter.GetInstance(document, fs);
                //PdfWriter.GetInstance(document, mem);  // tie a PdfWriter instance to the stream
                //document.SetPageSize(iTextSharp.text.PageSize.A3.Rotate());
                document.SetPageSize(PageSize.A4);
                document.Open();
                document.NewPage();
                BaseColor bcFaintRed = new BaseColor(169, 34, 82);
                
                Paragraph customerManagement = new Paragraph(string.Format("{0}", "CUSTOMER MANAGEMENT"));
                customerManagement.Alignment = Element.ALIGN_CENTER;
                document.Add(customerManagement);
                
                Paragraph record = new Paragraph(string.Format("{0}", "(Customer Record)"));
                record.Alignment = Element.ALIGN_CENTER;
                document.Add(record);
                document.Add(new Phrase(Environment.NewLine));

                string[] date = DateTime.Now.ToString().Split(" ");

                PdfPTable SNTable = new PdfPTable(4);
                PdfPCell sncell1 = new PdfPCell(new Phrase("Document No.", blackFntB)); sncell1.Colspan = 2; sncell1.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell sncell2 = new PdfPCell((new Phrase(Guid.NewGuid().ToString(), blackFnt))); sncell2.Colspan = 2; sncell2.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell sncell3 = new PdfPCell(new Phrase("Date", blackFntB)); sncell3.Colspan = 2; sncell3.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell sncell4 = new PdfPCell(new Phrase(date[0], blackFnt)); sncell4.Colspan = 2; sncell4.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell sncell5 = new PdfPCell(new Phrase("Customer Name", blackFntB)); sncell5.Colspan = 1; sncell5.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell sncell6 = new PdfPCell(new Phrase(customerProfile.FirstName + " " + customerProfile.LastName, blackFnt)); sncell6.Colspan = 3; sncell6.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell sncell7 = new PdfPCell(new Phrase("Note", blackFntB)); sncell7.Colspan = 2; sncell7.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell sncell8 = new PdfPCell(new Phrase("CreatedAt", blackFnt)); sncell8.Colspan = 2; sncell8.HorizontalAlignment = Element.ALIGN_LEFT;
                SNTable.AddCell(sncell1);
                SNTable.AddCell(sncell2);
                SNTable.AddCell(sncell3);
                SNTable.AddCell(sncell4);
                SNTable.AddCell(sncell5);
                SNTable.AddCell(sncell6);
                SNTable.AddCell(sncell7);
                SNTable.AddCell(sncell8);
                foreach (var x in customerRecord)
                {
                    PdfPCell sncell9 = new PdfPCell(new Phrase(x.Note, blackFntB)); sncell9.Colspan = 2; sncell9.HorizontalAlignment = Element.ALIGN_LEFT;
                    PdfPCell sncell10 = new PdfPCell(new Phrase(x.CreatedAt.ToString(), blackFnt)); sncell10.Colspan = 2; sncell10.HorizontalAlignment = Element.ALIGN_LEFT;
                    SNTable.AddCell(sncell9);
                    SNTable.AddCell(sncell10);
                }
                //document.Add(new Phrase(Environment.NewLine));
                PdfPCell sncell11 = new PdfPCell(new Phrase("Manager", blackFntB)); sncell11.Colspan = 1; sncell11.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell sncell12 = new PdfPCell(new Phrase("Olayemi Nurudeen Afolabi", blackFnt)); sncell12.Colspan = 3; sncell12.HorizontalAlignment = Element.ALIGN_LEFT;
                SNTable.AddCell(sncell11);
                SNTable.AddCell(sncell12);
                document.Add(SNTable);

                document.Close();   // automatically closes the attached MemoryStream
                string body = "Kindly find the attachment";
                mailer.SendInvoice(customerProfile.FirstName, customerProfile.Email, body, path);
                fs.Close();


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
