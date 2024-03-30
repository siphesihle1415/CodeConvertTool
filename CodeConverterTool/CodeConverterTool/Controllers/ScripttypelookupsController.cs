using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeConverterTool.Models;

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

        // GET: api/Scripttypelookups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Scripttypelookup>>> GetScripttypelookups()
        {
            return await _context.Scripttypelookups.ToListAsync();
        }

        // GET: api/Scripttypelookups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Scripttypelookup>> GetScripttypelookup(int id)
        {
            var scripttypelookup = await _context.Scripttypelookups.FindAsync(id);

            if (scripttypelookup == null)
            {
                return NotFound();
            }

            return scripttypelookup;
        }

        // PUT: api/Scripttypelookups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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

        // POST: api/Scripttypelookups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Scripttypelookup>> PostScripttypelookup(Scripttypelookup scripttypelookup)
        {
            _context.Scripttypelookups.Add(scripttypelookup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScripttypelookup", new { id = scripttypelookup.TypeId }, scripttypelookup);
        }

        // DELETE: api/Scripttypelookups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScripttypelookup(int id)
        {
            var scripttypelookup = await _context.Scripttypelookups.FindAsync(id);
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
