using CodeConverterTool.Controllers;
using CodeConverterTool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Dynamic;

namespace CodeConverterTool.Pages
{
    public class HomeModel : PageModel
    {

        ScriptsController scriptsController;
        public String nickname { get; set; }
        public String email { get; set; }

        public HomeModel()
        {
            ConvertToolDbContext context = new ConvertToolDbContext();
            scriptsController = new ScriptsController(context);
        }

        public void OnGet()
        {
            nickname = Request.Cookies["nickname"];
            email = Request.Cookies["email"];
        }

        [ValidateAntiForgeryToken]
        public async Task OnPostUpload()
        {
            Response.Redirect("/upload");
        }

        [ValidateAntiForgeryToken]
        public async Task OnPostViewUploaded()
        {
            Response.Redirect("/viewUploaded");
        }
    }
}
