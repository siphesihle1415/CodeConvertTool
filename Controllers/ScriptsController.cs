
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeConverterTool.Models;
using Microsoft.AspNetCore.Authorization;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.S3.Model;
using System;

namespace CodeConverterTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScriptsController : ControllerBase
    {
        private readonly ConvertToolDbContext _context;
        private readonly AmazonS3Client _s3Client;



        public ScriptsController(ConvertToolDbContext context)
        {
            _context = context;
            _s3Client = new AmazonS3Client("", "", Amazon.RegionEndpoint.EUWest1);
        }

        [HttpGet("GetScriptVersionsByKey")]
        public async Task<IActionResult> GetScriptVersionsByKey(string prefix)
        {
            try
            {
                ListVersionsRequest request = new ListVersionsRequest
                {
                    BucketName = "codeconvertbucket",
                    Prefix = prefix
                };

                ListVersionsResponse response = await _s3Client.ListVersionsAsync(request);

                var versions = response.Versions.Select(v => new
                {
                    VersionId = v.VersionId,
                    LastModified = v.LastModified,
                    Type = v.IsDeleteMarker ? "DeleteMarker" : "ObjectVersion"
                });
                return Ok(versions);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

            return Ok("Error Occurd");
        }

        [HttpGet("GetScriptById")]
        public async Task<IActionResult> GetScriptByIdAsync(string key, string versionId)
        {

            if (string.IsNullOrEmpty(versionId) || string.IsNullOrEmpty(key))
            {
                return BadRequest("Version ID an key is required.");
            }

            try
            {
                ListVersionsRequest ListRequest = new ListVersionsRequest
                {
                    BucketName = "codeconvertbucket",
                    Prefix = key
                };

                ListVersionsResponse ListResponse = await _s3Client.ListVersionsAsync(ListRequest);

                var versions = ListResponse.Versions.Select(v => new
                {
                    VersionId = v.VersionId,
                    LastModified = v.LastModified,
                    Type = v.IsDeleteMarker ? "DeleteMarker" : "ObjectVersion"
                });

                bool isValidId = versions.Any(v => v.VersionId == versionId && v.Type != "DeleteMarker");

                if (!isValidId)
                {
                    return BadRequest("You Cannot access a script version that was deleted. The script version that you selected is a Delete Marker.");
                }

                GetObjectRequest request = new GetObjectRequest
                {
                    BucketName = "codeconvertbucket",
                    Key = key,
                    VersionId = versionId
                };

                using (GetObjectResponse response = await _s3Client.GetObjectAsync(request))
                {

                    if (response == null)
                    {
                        return Ok("No Object found with that version ID");
                    }

                    using (Stream responseStream = response.ResponseStream)
                    {
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            string fileContent = await reader.ReadToEndAsync();
                            return Ok(fileContent);
                        }
                    }
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

            return Ok("Error Occurd");
        }


        [HttpPost("UploadScript")]
        public async Task<IActionResult> UploadFileToS3(IFormFile file)
        {
            using var fileTransferUtility = new TransferUtility(_s3Client);

            try
            {
                await fileTransferUtility.UploadAsync(file.OpenReadStream(), "codeconvertbucket", file.FileName);
                Console.WriteLine("Upload completed successfully.");
                return Ok("File Uploaded");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

            return Ok("Error Occurd");
        }



        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Script>>> GetScripts()
        {
            return await _context.Scripts.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Script>> GetScript(char id)
        {
            if (!Char.IsDigit(id))
            {
                return BadRequest("Invalid ID format");
            }

            int idValue = int.Parse("" + id);

            if (idValue <= 0)
            {
                return BadRequest("Invalid ID format. Must be a Positive Integer.");
            }

            var script = await _context.Scripts.FindAsync(idValue);

            if (script == null)
            {
                return NotFound();
            }

            return script;
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScript(int id, Script script)
        {
            if (id != script.ScriptId)
            {
                return BadRequest();
            }

            _context.Entry(script).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScriptExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Script>> PostScript(Script script)
        {
            _context.Scripts.Add(script);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScript", new { id = script.ScriptId }, script);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScript(char id)
        {
            if (!Char.IsDigit(id))
            {
                return BadRequest("Invalid ID format");
            }

            int idValue = int.Parse("" + id);

            if (idValue <= 0)
            {
                return BadRequest("Invalid ID format. Must be a Positive Integer.");
            }

            var script = await _context.Scripts.FindAsync(idValue);
            if (script == null)
            {
                return NotFound();
            }

            _context.Scripts.Remove(script);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ScriptExists(int id)
        {
            return _context.Scripts.Any(e => e.ScriptId == id);
        }
    }
}
