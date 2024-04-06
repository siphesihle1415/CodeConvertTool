using CodeConverterTool.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Amazon;
using Amazon.Runtime;

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
            var RestClient = new RestClient(Environment.GetEnvironmentVariable("LOGIN_AUTH_CLIENT"));
            RestRequest request = new RestRequest
            {
                Method = Method.Post
            };
            request.AddHeader("Content-Type", "application/json");
            string jsonBody = "{\"client_id\":\"" + "" + "\",\"client_secret\":\"" + Environment.GetEnvironmentVariable("CLIENT_SECRET") + "\",\"audience\":\"https://localhost:7074/swagger/index.html/api\",\"grant_type\":\"client_credentials\"}";

            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            Console.WriteLine(request.Parameters.ToString());

            RestResponse response = RestClient.Execute(request);

            Console.WriteLine(response.StatusCode.ToString());


            if (response.IsSuccessful)
            {
                return Ok(response.Content);
            }
            else
            {
                return BadRequest(response.Content);
            }
        }
    }
}
