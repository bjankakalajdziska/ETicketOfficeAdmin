using ClosedXML.Excel;
using ETicketOfficeAdmin.Web.Models;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ETicketOfficeAdmin.Web.Controllers
{
    public class OrderController : Controller
    {
        public OrderController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();


            string URI = "https://localhost:44379/api/Admin/GetAllActiveOrders";

            HttpResponseMessage responseMessage = client.GetAsync(URI).Result;

            var result = responseMessage.Content.ReadAsAsync<List<Order>>().Result;

            return View(result);
        }

        public IActionResult Details(Guid orderId)
        {
            HttpClient client = new HttpClient();


            string URI = "https://localhost:44379/api/Admin/GetDetailsForOrder";

            var model = new
            {
                Id = orderId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.PostAsync(URI, content).Result;


            var result = responseMessage.Content.ReadAsAsync<Order>().Result;


            return View(result);
        }

        public IActionResult CreateInvoice(Guid orderId)
        {
            HttpClient client = new HttpClient();


            string URI = "https://localhost:44379/api/Admin/GetDetailsForOrder";

            var model = new
            {
                Id = orderId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.PostAsync(URI, content).Result;


            var result = responseMessage.Content.ReadAsAsync<Order>().Result;


            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);


            document.Content.Replace("{{OrderNumber}}", result.Id.ToString());
            document.Content.Replace("{{UserName}}", result.User.UserName);

            StringBuilder sb = new StringBuilder();

            var totalPrice = 0.0;

            foreach (var item in result.Tickets)
            {
                totalPrice += item.Quantity * item.Ticket.TicketPrice;
                sb.AppendLine(item.Ticket.MovieName + " with quantity of: " + item.Quantity + " and price of: " + item.Ticket.TicketPrice + "$");
            }


            document.Content.Replace("{{TicketList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString() + "$");


            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");
        }

        [HttpGet]
        public FileContentResult ExportAllOrders()
        {
            string fileName = "Orders.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Orders");

                worksheet.Cell(1, 1).Value = "Order Id";
                worksheet.Cell(1, 2).Value = "Costumer Email";


                HttpClient client = new HttpClient();


                string URI = "https://localhost:44309/api/Admin/GetOrders";

                HttpResponseMessage responseMessage = client.GetAsync(URI).Result;

                var result = responseMessage.Content.ReadAsAsync<List<Order>>().Result;

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.User.Email;

                    for (int p = 0; p < item.Tickets.Count(); p++)
                    {
                        worksheet.Cell(1, p + 3).Value = "Product-" + (p + 1);
                        worksheet.Cell(i + 1, p + 3).Value = item.Tickets.ElementAt(p).Ticket.MovieName;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }

            }
        }

    }
}
