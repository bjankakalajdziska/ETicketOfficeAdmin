using ETicketOfficeAdmin.Web.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
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
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ImportUsers(IFormFile file)
        {

            //make a copy
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            //read data from copy file

            List<User> users = getAllUsersFromFile(file.FileName);


            HttpClient client = new HttpClient();


            string URI = "https://localhost:44309/api/Admin/ImportAllUsers";


            HttpContent content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = client.PostAsync(URI, content).Result;


            var result = responseMessage.Content.ReadAsAsync<bool>().Result;


            return RedirectToAction("Index", "Order");
        }

        private List<User> getAllUsersFromFile(string fileName)
        {

            List<User> users = new List<User>();

            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        users.Add(new Models.User
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            ConfirmPassword = reader.GetValue(2).ToString()
                        });
                    }
                }
            }


            return users;
        }
    }
}
