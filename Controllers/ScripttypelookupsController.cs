using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeConverterTool.Models;
using Microsoft.AspNetCore.Authorization;

namespace CodeConverterTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScripttypelookupsController : ControllerBase
    {
        private readonly ConvertToolDbContext _context;

        public ScripttypelookupsController(ConvertToolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Scripttypelookup>>> GetScripttypelookups()
        {
            return await _context.Scripttypelookups.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Scripttypelookup>> GetScripttypelookup(char id)
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

            var scripttypelookup = await _context.Scripttypelookups.FindAsync(idValue);

            if (scripttypelookup == null)
            {
                return NotFound();
            }

            return scripttypelookup;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutScripttypelookup(int id, Scripttypelookup scripttypelookup)
        {
            if (id != scripttypelookup.TypeId)
            {
                return BadRequest();
            }

            _context.Entry(scripttypelookup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScripttypelookupExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Scripttypelookup>> PostScripttypelookup(Scripttypelookup scripttypelookup)
        {
            _context.Scripttypelookups.Add(scripttypelookup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScripttypelookup", new { id = scripttypelookup.TypeId }, scripttypelookup);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScripttypelookup(char id)
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

            var scripttypelookup = await _context.Scripttypelookups.FindAsync(idValue);
            if (scripttypelookup == null)
            {
                return NotFound();
            }

            _context.Scripttypelookups.Remove(scripttypelookup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ScripttypelookupExists(int id)
        {
            return _context.Scripttypelookups.Any(e => e.TypeId == id);
        }
    }
}
