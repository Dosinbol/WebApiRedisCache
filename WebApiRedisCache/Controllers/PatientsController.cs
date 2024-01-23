using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiRedisCache.Data;
using WebApiRedisCache.Models;
using WebApiRedisCache.Services.Interfaces;

namespace WebApiRedisCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private ProjectContext _context;
        private ICacheService _cache;
        public PatientsController(ProjectContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }
        [HttpGet("patient")]
        public async Task<IActionResult> Get()
        {
            var cacheData = _cache.GetData<IEnumerable<Patient>>("patients");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }
            cacheData = await _context.Patients.ToListAsync();
            _cache.SetData<IEnumerable<Patient>>("patients", cacheData);
            return Ok(cacheData);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null)
            {
                return NotFound();
            }
            var cacheData = _cache.GetData<Patient>($"patiensts:{id}");
            if (cacheData != null)
            {
                return Ok(cacheData);
            }
            _cache.SetData<Patient>($"patients:{id}", cacheData);
            return Ok(patient);
        }

        [HttpPost("AddPatient")]
        public async Task<IActionResult> Post(Patient patient)
        {
            var addedPatient = await _context.Patients.AddAsync(patient);

            _cache.SetData<Patient>($"patients:{patient.Id}", addedPatient.Entity);
            await _context.SaveChangesAsync();
            return Ok(addedPatient.Entity);
        }

        [HttpPut("UpdatePatient")]
        public async Task<IActionResult> Update(Patient patient)
        {
            var selected = await _context.Patients.FirstOrDefaultAsync(x => x.Id == patient.Id);
            if (selected == null)
            {
                return NotFound();
            }
            selected.Name = patient.Name;
            selected.Illness = patient.Illness;
            selected.Gender = patient.Gender;
            _cache.SetData<Patient>($"patients:{patient.Id}", selected);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("DeletePatient")]
        public async Task<IActionResult> Delete(int id)
        {
            var exist = await _context.Patients.FirstOrDefaultAsync(a => a.Id == id);

            if (exist != null)
            {
                _context.Remove(exist);
                _cache.RemoveData($"patients:{id}");
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }
    }
}
