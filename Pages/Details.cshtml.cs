using CodeConverterTool.Controllers;
using CodeConverterTool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CodeConverterTool.Pages
{
    public class DetailsModel : PageModel
    {

        ScriptsController scriptsController;

        public List<String> versionID { get; set; }
        public List<String> lastModified { get; set; }
        public IEnumerable<dynamic> zippedArrays { get; set; }

        public string ID { get; set;}

        public DetailsModel()
        {
            ConvertToolDbContext context = new ConvertToolDbContext();
            scriptsController = new ScriptsController(context);

            versionID = new List<String>();
            lastModified = new List<String>();
            zippedArrays = new List<dynamic>();
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

            Console.WriteLine("*****************");
            Console.WriteLine(convertLanguage);
            Console.WriteLine(versionID);

            /*this.versionID = new List<String>();
            this.lastModified = new List<String>();
            this.zippedArrays = new List<dynamic>();
            await OnGet(ID);*/
            Response.Redirect("/viewUploaded");
        }
    }

}
