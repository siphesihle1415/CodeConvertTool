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
using Azure;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace CodeConverterTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ConvertToolDbContext _context;

        public String VerificationUri = "null";

        public LoginController(ConvertToolDbContext context)
        {
            _context = context;
        }

        [HttpPost("LoginGetLink")]
        public async Task<IActionResult> LoginGetLink()
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

            RestResponse response = await RestClient.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                JObject data = JObject.Parse(response.Content);

                return Ok(data.ToString());

            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPost("LoginGetToken")]
        public async Task<IActionResult> LoginGetToken([FromBody] GetTokenObject data)
        {
            string deviceCode = data.device_code;
            string verificationUriComplete = data.verification_uri_complete;
            string accessCode = "";
            JObject responseData = null;

            try
            {
                var tcs = new TaskCompletionSource<bool>();


                Timer timer = new Timer(_ =>
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
                return BadRequest();
            }

            return Ok(accessCode);

        }



        [HttpPost("Login")]
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

                Console.WriteLine(data.ToString());

                string deviceCode = (string)data["device_code"];
                string verificationUriComplete = (string)data["verification_uri_complete"];
                string accessCode = "";
                JObject responseData = null;

                VerificationUri = verificationUriComplete;

                Console.WriteLine(verificationUriComplete);
                try
                {
                    var tcs = new TaskCompletionSource<bool>();


                    Timer timer = new Timer(_ =>
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


        [HttpPost("InitiateLogin")]
        public async Task<IActionResult> InitiateLogin()
        {
            try
            {
                string deviceDomain = Environment.GetEnvironmentVariable("LOGIN_AUTH_CLIENT_DEVICE");
                var restClient = new RestClient(deviceDomain);
                var request = new RestRequest
                {
                    Method = Method.Post
                };

                string helperString = "client_id=" + Environment.GetEnvironmentVariable("CLI_CLIENT_ID") + "&scope=openid profile email&audience=https://localhost:7074/swagger/index.html/api";
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", helperString, ParameterType.RequestBody);

                var response = await restClient.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    JObject data = JObject.Parse(response.Content);
                    string verificationUriComplete = (string)data["verification_uri_complete"];
                    string deviceCode = (string)data["device_code"];
                    string interval = (string)data["interval"];


                    return Ok(new
                    {
                        verificationUriComplete,
                        deviceCode,
                        interval
                    });
                }
                else
                {
                    return BadRequest(response.Content);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("PollForAccessCode")]
        public async Task<IActionResult> PollForAccessCode(JObject requestBody)
        {
            try
            {
                Console.WriteLine(requestBody.ToString());


                string deviceCode = (string)requestBody["deviceCode"];

                var tokenClient = new RestClient(Environment.GetEnvironmentVariable("LOGIN_AUTH_CLIENT_TOKEN"));
                var restRequest = new RestRequest
                {
                    Method = Method.Post
                };
                restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                restRequest.AddParameter("application/x-www-form-urlencoded", "grant_type=urn:ietf:params:oauth:grant-type:device_code&device_code=" + deviceCode + "&client_id=" + Environment.GetEnvironmentVariable("CLI_CLIENT_ID"), ParameterType.RequestBody);

                var response = await tokenClient.ExecuteAsync(restRequest);

                if (response.IsSuccessful)
                {
                    JObject responseData = JObject.Parse(response.Content);
                    string accessCode = (string)responseData["access_token"];
                    return Ok(new { AccessCode = accessCode });
                }
                else
                {
                    return BadRequest(response.Content);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo(JObject requestBody)
        {
            try
            {
                string token = (string)requestBody["token"];

                var DetailsClient = new RestClient(Environment.GetEnvironmentVariable("LOGIN_AUTH_CLIENT_INFO"));
                var restRequest = new RestRequest
                {
                    Method = Method.Get
                };
                restRequest.AddHeader("Content-Type", "application/json");
                restRequest.AddHeader("Authorization", $"Bearer {token}");

                var DetailsResponse = await DetailsClient.ExecuteAsync(restRequest);

                JObject responseData = JObject.Parse(DetailsResponse.Content);
                string email = (string)responseData["email"];
                string nickname = (string)responseData["nickname"];
                return Ok(new
                {
                    nickname = nickname,
                    Email = email,
                });




            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
