using Kolokwium1.Models;

namespace Kolokwium1.Services {
    public interface IDbService {
        Medicament GetMedicament(int id);
        bool DeletePatient(int id);
    }
}