using CodeConverterTool.Models;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

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
        public IActionResult Login()
        {
            
            var client = new RestClient(Environment.GetEnvironmentVariable("LOGIN_AUTH_CLIENT"));



            RestRequest request = new RestRequest
            {
                Method = Method.Post
            };
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"client_id\":\"" + Environment.GetEnvironmentVariable("CLIENT_ID") + "\",\"client_secret\":\"" + Environment.GetEnvironmentVariable("CLIENT_SECRET") + "\",\"audience\":\"https://localhost:7074/swagger/index.html/api\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            RestResponse response = client.Execute(request);


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
