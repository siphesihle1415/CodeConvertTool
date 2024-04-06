using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeConverterTool.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace CodeConverterTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevelopersController : ControllerBase
    {
        private readonly ConvertToolDbContext _context;

        public DevelopersController(ConvertToolDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Developer>>> GetDevelopers()
        {
            return await _context.Developers.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Developer>> GetDeveloper(char id)
        {

            if (!Char.IsDigit(id))
            {
                return BadRequest("Invalid ID format");
            }

            int idValue = int.Parse("" + id);

            if (idValue <= 0)
            {
                return BadRequest("Invalid ID format. Must be a positive integer");
            }


            var developer = await _context.Developers.FindAsync(idValue);

            if (developer == null)
            {
                return NotFound();
            }

            return developer;
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeveloper(int id, Developer developer)
        {
            if (id != developer.DevId)
            {
                return BadRequest();
            }

            _context.Entry(developer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeveloperExists(id))
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
        public async Task<ActionResult<Developer>> PostDeveloper(Developer developer)
        {
            _context.Developers.Add(developer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeveloper", new { id = developer.DevId }, developer);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeveloper(char id)
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

            var developer = await _context.Developers.FindAsync(idValue);
            if (developer == null)
            {
                return NotFound();
            }

            _context.Developers.Remove(developer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeveloperExists(int id)
        {
            return _context.Developers.Any(e => e.DevId == id);
        }
    }
}
