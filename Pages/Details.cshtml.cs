using CodeConverterTool.Controllers;
using CodeConverterTool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace CodeConverterTool.Pages
{
    public class DetailsModel : PageModel
    {

        ScriptsController scriptsController;
        ScriptConvertController scriptsConvertController;

        public List<String> versionID { get; set; }
        public List<String> lastModified { get; set; }
        public IEnumerable<dynamic> zippedArrays { get; set; }
        public string ID { get; set;}
        HttpClient _httpClient;

        public DetailsModel()
        {
            ConvertToolDbContext context = new ConvertToolDbContext();
            scriptsController = new ScriptsController(context);

            versionID = new List<String>();
            lastModified = new List<String>();
            zippedArrays = new List<dynamic>();

            scriptsConvertController = new ScriptConvertController();

            _httpClient = new HttpClient();
        }

        public async Task OnGet(String id)
        {

            ID = id;

            var result = await scriptsController.GetScriptVersionsByKey(id);

            var resultObj = JsonConvert.SerializeObject(result);
            JObject resultJson = JObject.Parse(resultObj);

            foreach ( var item in resultJson["Value"])
            {
                versionID.Add((string)item["VersionId"]);
                lastModified.Add((string)item["LastModified"]);
            }

            zippedArrays = versionID.Zip(lastModified, (item1, item2) => new { Item1 = item1, Item2 = item2 });

        }

        public async Task OnPostConvertScript()
        {
            string convertLanguage = Request.Form["convertLanguage"];
            string versionID = Request.Form["versionID"];
            string userID = Request.Form["ID"];

            String downloadURL = "https://codeconvertbucket.s3.eu-west-1.amazonaws.com/" + userID + "?versionId=" + versionID;

            String fileContent = "";

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(downloadURL))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await stream.CopyToAsync(memoryStream);
                                memoryStream.Seek(0, SeekOrigin.Begin);

                                var downloadedFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", userID);

                                using (var streamReader = new StreamReader(downloadedFile.OpenReadStream()))
                                {
                                    fileContent = await streamReader.ReadToEndAsync();
                                    Console.WriteLine(fileContent);
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to download the file. Status code: " + response.StatusCode);
                    }
                }
            }

                /*Console.WriteLine(downloadURL);

                IFormFile downloadedFile = await DownloadFileAsFormFileAsync(downloadURL, userID);

                Console.WriteLine(downloadedFile.Length);


                // Read the stream from the IFormFile
                var streamReader = new StreamReader(downloadedFile.OpenReadStream());

                // Read the entire content of the file
                var fileContent = streamReader.ReadToEnd();


                 Console.WriteLine(fileContent);

                Console.WriteLine("*****************");
                Console.WriteLine(convertLanguage);
                Console.WriteLine(versionID);*/

                ScriptConvert scriptConvert = new ScriptConvert();
                scriptConvert.Model = "gpt-3.5-turbo-instruct";
                scriptConvert.TargetScript = convertLanguage;
                scriptConvert.SourceScript = "";
                scriptConvert.Content = fileContent;
                scriptConvert.MaxTokens = 10000;

                var result = await scriptsConvertController.PostScriptConvert("", scriptConvert);

                Console.WriteLine(result);

                var resultObj = JsonConvert.SerializeObject(result);
                JObject resultJson = JObject.Parse(resultObj);

                String convertedCode = (string) resultJson["Value"];

                int index = userID.IndexOf("/");
                StringBuilder stringBuilder = new StringBuilder(userID);
                stringBuilder.Insert(index + 1, convertLanguage+"_");

                string newFileName = stringBuilder.ToString();

                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(convertedCode);
            
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    IFormFile formFile = new FormFile(stream, 0, bytes.Length, "file", newFileName);
                    await scriptsController.UploadFileToS3(formFile);
                }

                


                //IFormFile myIFormFile = new IFormFile(content, filename);


                /*this.versionID = new List<String>();
                this.lastModified = new List<String>();
                this.zippedArrays = new List<dynamic>();
                await OnGet(ID);*/
                Response.Redirect("/viewUploaded");
        }

        public async Task<IFormFile> DownloadFileAsFormFileAsync(string fileUrl, string fileName)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(fileUrl))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var formFile = new FormFile(stream, 0, stream.Length, null, fileName)
                            {
                                Headers = new HeaderDictionary(),
                                ContentType = "application/octet-stream"
                            };

                            return formFile;
                        }
                    }
                    else
                    {
                        throw new Exception($"Failed to download file. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
