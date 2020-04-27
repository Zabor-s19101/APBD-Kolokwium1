using System.Data.SqlClient;
using Kolokwium1.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium1.Controllers {
    [ApiController]
    [Route("api/patients")]
    public class PatientsController : ControllerBase {
        private readonly IDbService _dbService;

        public PatientsController(IDbService dbService) {
            _dbService = dbService;
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePatient(int id) {
            if (!_dbService.DeletePatient(id)) {
                return NotFound("Nie znaleziono takiego pacjenta");
            }
            return Ok("Pacjent usuniety");
        }
    }
}