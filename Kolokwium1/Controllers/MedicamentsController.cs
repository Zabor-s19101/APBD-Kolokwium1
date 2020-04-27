using Kolokwium1.Models;
using Kolokwium1.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium1.Controllers {
    [ApiController]
    [Route("api/medicaments")]
    public class MedicamentsController : ControllerBase {
        private readonly IDbService _dbService;

        public MedicamentsController(IDbService dbService) {
            _dbService = dbService;
        }
        
        [HttpGet("{id}")]
        public IActionResult GetMedicament(int id) {
            var medicament = _dbService.GetMedicament(id);
            if (medicament == null) {
                return NotFound("Nie ma takiego leku");
            }
            return Ok(medicament);
        }
    }
}