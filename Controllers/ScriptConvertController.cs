using System;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using CodeConverterTool.Models;
using System.Linq;
using System.Net.Http.Json;

namespace CodeConverterTool.Controllers
{
    [Route("api")]
    [ApiController]
    public class ScriptConvertController : ControllerBase
	{
		[HttpPost("ScriptConvertGPT/{apiKey}")]
        [Authorize]
        public async System.Threading.Tasks.Task<IActionResult> PostScriptConvert(string apiKey, Models.ScriptConvert scriptConvert)
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
                    prompt = $"Translate the following code to {targetScript}, provide only the code, and nothing else. Make sure the code you provide is complete and well formatted : {content}",
				};

				var jsonReq = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

				var response = await client.PostAsync("completions", jsonReq);

				if (response.IsSuccessStatusCode)
				{
					var jsonResponse = await response.Content.ReadAsStringAsync();

					var parsedResponse = JObject.Parse(jsonResponse);

					return Ok(parsedResponse["choices"]![0]!["text"]!.ToString());
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

        [HttpPost("ScriptConvertGemini")]
        [Authorize]
        public async System.Threading.Tasks.Task<IActionResult> PostScriptConvert(ScriptConvertGemini scriptConvertRequest)
        {
            try
            {
                var part = new Part
                {
                    Text = $"Convert this {scriptConvertRequest.Source} code {scriptConvertRequest.Content} to {scriptConvertRequest.Target} (without backticks)"
                };

                var content = new Content
                {
                    Parts = [part]
                };

                var generateContentRequest = new GenerateContentRequest
                {
                    Contents = [content]

                };

                using HttpClient client = new();
                client.BaseAddress = new Uri("https://generativelanguage.googleapis.com");
                client.DefaultRequestHeaders.Authorization = null;

                var response = await client.PostAsJsonAsync($"/v1beta/models/gemini-pro:generateContent?key={Environment.GetEnvironmentVariable("GEMINI_API_KEY")}", generateContentRequest);

                if (response.IsSuccessStatusCode)
                {
                    using var jsonResponse = await response.Content.ReadAsStreamAsync();

                    var generateContentResponse = System.Text.Json.JsonSerializer.Deserialize<GenerateContentResponse>(jsonResponse);

                    string modelResponse = String.Join("", generateContentResponse?.Candidates?[0].Content?.Parts?.Select(part => part.Text)!);

                    string trimmedResponse = modelResponse.Contains('`') ? String.Join('\n', modelResponse.Split('\n').Skip(1).SkipLast(1)) : modelResponse;

                    return Ok(
                        new
                        {
                            response = trimmedResponse
                        });
                }

                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}