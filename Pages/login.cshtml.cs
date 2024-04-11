using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeConverterTool.Controllers;
using CodeConverterTool.Models;
using NuGet.Protocol;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using Azure.Core;

namespace CodeConverterTool.Pages
{
    public class loginModel : PageModel
    {
        public string Message { get; set; }
        LoginController loginController;
        public loginModel()
        {
            ConvertToolDbContext context = new ConvertToolDbContext();
            loginController = new LoginController(context);
            Message = "Hello there!";

        }
       
        public async void OnGet()
        {
            /*var result = loginController.Login();
            await result;

            Console.WriteLine("----------------");

            var resultObj = JsonConvert.SerializeObject(result);
            JObject resultJson = JObject.Parse(resultObj);

            Console.WriteLine(resultJson);

            string value = (string) resultJson["Result"]["Value"];
            JObject access_token_json = JObject.Parse(value);
            string access_token = (string) access_token_json["access_token"];


            Console.WriteLine(access_token);

            Console.WriteLine("----------------");*/

        }
    }
}
