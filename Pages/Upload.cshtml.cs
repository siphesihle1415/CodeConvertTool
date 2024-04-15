using CodeConverterTool.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CodeConverterTool.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CodeConverterTool.Pages
{
    public class UploadModel : PageModel
    {
        ScriptsController scriptsController;

        public String uploadStatus { get; set; }

        public UploadModel()
        {
            ConvertToolDbContext context = new ConvertToolDbContext();
            scriptsController = new ScriptsController(context);
        }

        [ValidateAntiForgeryToken]
        public async Task OnPostUpload(IFormFile file)
        {
            String nickname = Request.Cookies["nickname"];

            var result = await scriptsController.UploadFileToS3(file, nickname);

            var resultObj = JsonConvert.SerializeObject(result);
            JObject resultJson = JObject.Parse(resultObj);

            uploadStatus = (string) resultJson["Value"];
        }

        [ValidateAntiForgeryToken]
        public async Task OnPostBack()
        {
            Response.Redirect("/home");
        }


    }
}

/*
 <form method="post" asp-page-handler="Back">
        @Html.AntiForgeryToken()
        <button type="submit">Back</button>
    </form>*/