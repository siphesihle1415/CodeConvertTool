using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CodeConverterTool.Controllers;
using CodeConverterTool.Models;
using NuGet.Protocol;
using Newtonsoft.Json;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using Azure.Core;
using System.Security.Principal;
using Azure;
using Microsoft.JSInterop;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CodeConverterTool.Pages
{
    public class loginModel : PageModel
    {

        LoginController loginController;
        public String linkUrl { get; set;}

        public loginModel()
        {
            ConvertToolDbContext context = new ConvertToolDbContext();
            loginController = new LoginController(context);
            linkUrl = "null";
        }

        [ValidateAntiForgeryToken]
        public async Task OnPostGetLink()
        {
            linkUrl = await getOAuthLink();
        }

        public async Task OnPostGetToken()
        {
        
            string token_link = Request.Form["token_link"];

            JObject result = Deserialize(HttpContext.Session.Get(token_link));

            string value = (string)result["Value"];
            JObject value_json = JObject.Parse(value);

            GetTokenObject tokenObj = new GetTokenObject();

            tokenObj.device_code = (string) value_json["device_code"];
            tokenObj.user_code = (string) value_json["user_code"];
            tokenObj.verification_uri = (string) value_json["verification_uri"];
            tokenObj.expires_in = (int) value_json["expires_in"];
            tokenObj.interval = (int) value_json["interval"];
            tokenObj.verification_uri_complete = (string) value_json["verification_uri_complete"];

            var result_token = await loginController.LoginGetToken(tokenObj);

            var resultObj = JsonConvert.SerializeObject(result_token);
            JObject resultJson = JObject.Parse(resultObj);

            Response.Cookies.Append("oAuth_Token", (string) resultJson["Value"]);

            ////////////////////////////////////////////

            JObject details = new JObject();

            details.Add("token", (string)resultJson["Value"]);

            var resultDetails = await loginController.GetUserInfo(details);

            var resultObjDetails = JsonConvert.SerializeObject(resultDetails);
            JObject resultJsonDetails= JObject.Parse(resultObjDetails);


            var resultObjLast = JsonConvert.SerializeObject(resultJsonDetails["Value"]);
            JObject userDetails_json = JObject.Parse(resultObjLast);

            Response.Cookies.Append("nickname", (string)userDetails_json["nickname"]);
            Response.Cookies.Append("Email", (string)userDetails_json["Email"]);

            Response.Redirect("/home");

        }
       
        public async Task<String> getOAuthLink()
        {

            var result = await loginController.LoginGetLink();

            var resultObj = JsonConvert.SerializeObject(result);
            JObject resultJson = JObject.Parse(resultObj);

            string value = (string)resultJson["Value"];
            JObject verification_uri_complete_json = JObject.Parse(value);   

            string verification_uri_complete = (string)verification_uri_complete_json["verification_uri_complete"];

            HttpContext.Session.Set(verification_uri_complete, Serialize(resultJson));

            return verification_uri_complete;
        }

        public async void getOAuthToken(Task<IActionResult> result)
        {
            //await result;
            //var okResult = result.Result as OkObjectResult;
            //return okResult.Value.ToString();

            //var result_link = await loginController.LoginGetLink();
            //var okResult = result_link.Result as OkObjectResult;

            //Console.WriteLine(result_link);

        }



        private byte[] Serialize(JObject obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        private JObject Deserialize(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<JObject> (json);
        }



    }
}
