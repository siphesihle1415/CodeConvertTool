using CodeConverterTool.Controllers;
using CodeConverterTool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Azure;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeConverterTool.Pages
{
    public class ViewUploadedModel : PageModel
    {
        ScriptsController scriptsController;

        public String uploadStatus { get; set; }
        public List<String> keys { get; set; }

        public ViewUploadedModel()
        {
            ConvertToolDbContext context = new ConvertToolDbContext();
            scriptsController = new ScriptsController(context);

            keys = new List<String>();
        }

        public async Task OnGet()
        {
            String nickname = Request.Cookies["nickname"];

            var result = await scriptsController.GetObjectsByFolderName(nickname);
            var resultObj = JsonConvert.SerializeObject(result);
            JObject resultJson = JObject.Parse(resultObj);

            foreach (var item in resultJson["Value"])
            {
                keys.Add((string)item["Key"]);
            }

        }
    }
}