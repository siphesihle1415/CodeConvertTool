using System;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;

namespace CodeConverterTool.Controllers
{
    [ApiController]
	[Route("[controller]")]
	public class ScriptConvertController : ControllerBase
	{

		[Authorize]
		[HttpPost("{apiKey}")]
		public async System.Threading.Tasks.Task<IActionResult> PostScriptConvert(string apiKey, Models.ScriptConvert scriptConvert)
		{
			try
			{
				var model = scriptConvert.Model;
				var content = scriptConvert.Content;
				var sourceScript = scriptConvert.SourceScript;
				var targetScript = scriptConvert.TargetScript;
				var maxTokens = scriptConvert.MaxTokens;

				using System.Net.Http.HttpClient client = new();

				//Console.WriteLine("*************");

				client.BaseAddress = new Uri("https://api.openai.com/v1/");
				client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

				var req = new
				{
                    model,
                    //prompt = $"Translate the following {sourceScript} code to {targetScript}: {content}",
                    prompt = $"Translate the following code to {targetScript}, provide only the code, and nothing else. Make sure the code you provide is complete and well formatted : {content}",
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