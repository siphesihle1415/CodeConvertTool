using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;

namespace CodeConverterTool.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
	public class ScriptConvertController : ControllerBase
	{

		[Authorize]
		[HttpPost("{apiKey}")]
		public async Task<IActionResult> PostScriptConvert(string apiKey, Models.ScriptConvert scriptConvert)
		{
			try
			{
				var model = scriptConvert.Model;
				var content = scriptConvert.Content;
				var sourceScript = scriptConvert.SourceScript;
				var targetScript = scriptConvert.TargetScript;
				var maxTokens = scriptConvert.MaxTokens;

				using HttpClient client = new();

				client.BaseAddress = new Uri("https://api.openai.com/v1/");
				client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

				var req = new
				{
                    model,
					prompt = $"Translate the following {sourceScript} code to {targetScript}: {content}",
					max_tokens = maxTokens
				};

				var jsonReq = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

				var response = await client.PostAsync("completions", jsonReq);

				if (response.IsSuccessStatusCode)
				{
					var jsonResponse = await response.Content.ReadAsStringAsync();

					var parsedResponse = JObject.Parse(jsonResponse);

					return Ok(parsedResponse["choices"][0]["text"].ToString());
				}
				else
				{
					return StatusCode((int)response.StatusCode, response.ReasonPhrase);
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
	}
}