using CodeConverterTool.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Amazon;
using Amazon.Runtime;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Azure.Core;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeConverterTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ConvertToolDbContext _context;

        public LoginController(ConvertToolDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login()
        {
            string deviceDomain = Environment.GetEnvironmentVariable("LOGIN_AUTH_CLIENT_DEVICE");

            var RestClient = new RestClient(deviceDomain);
            RestRequest request = new RestRequest
            {
                Method = Method.Post
            };
            string helperString = "client_id=" + Environment.GetEnvironmentVariable("CLI_CLIENT_ID") + "&scope=openid profile email&audience=https://localhost:7074/swagger/index.html/api";



            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("application/x-www-form-urlencoded", helperString, ParameterType.RequestBody);

            RestResponse response = RestClient.Execute(request);






            if (response.IsSuccessful)
            {
                JObject data = JObject.Parse(response.Content);

                string deviceCode = (string)data["device_code"];
                string verificationUriComplete = (string)data["verification_uri_complete"];
                string accessCode = "";
                JObject responseData = null;

                Console.WriteLine(verificationUriComplete);
                try
                {
                    var tcs = new TaskCompletionSource<bool>();


                    Timer timer = new Timer( _=>
                    {
                        var TokenClient = new RestClient(Environment.GetEnvironmentVariable("LOGIN_AUTH_CLIENT_TOKEN"));
                        RestRequest request = new RestRequest
                        {
                            Method = Method.Post
                        };
                        request.AddHeader("content-type", "application/x-www-form-urlencoded");
                        request.AddParameter("application/x-www-form-urlencoded", "grant_type=urn:ietf:params:oauth:grant-type:device_code&device_code=" + deviceCode + "&client_id=" + Environment.GetEnvironmentVariable("CLI_CLIENT_ID"), ParameterType.RequestBody);
                        RestResponse response = TokenClient.Execute(request);

                        Console.WriteLine(response.Content);

                        if (response.IsSuccessful)
                        {
                            responseData = JObject.Parse(response.Content);
                            accessCode = (string)responseData["access_token"];
                            tcs.SetResult(true);
                        }

                    }
                    , null, 0, 10000);

                    await tcs.Task;

                    timer.Dispose(); 

                }
                catch (Exception ex)
                {
                    return BadRequest(response.Content);
                }


                return Ok(accessCode);

            }
            else
            {
                return BadRequest(response.Content);
            }
        }

    }
}
