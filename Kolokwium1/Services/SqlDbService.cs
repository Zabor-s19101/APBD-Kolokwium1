using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Kolokwium1.Models;

namespace Kolokwium1.Services {
    public class SqlDbService : IDbService {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s19101;Integrated Security=True";
        
        public Medicament GetMedicament(int id) {
            Medicament medicament = null;
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand()) {
                com.Connection = con;
                com.CommandText = "select * from dbo.Medicament where Medicament.IdMedicament = @IdMedicament";
                com.Parameters.Clear();
                com.Parameters.AddWithValue("IdMedicament", id);
                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read()) {
                    medicament = new Medicament {
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString(),
                        Type = dr["Type"].ToString()
                    };
                } else {
                    return null;
                }
                dr.Close();
                com.CommandText = "select Prescription_Medicament.Dose, Prescription_Medicament.Details, Prescription.Date, Prescription.DueDate from dbo.Prescription inner join dbo.Prescription_Medicament on Prescription_Medicament.IdPrescription = Prescription.IdPrescription where Prescription_Medicament.IdMedicament = @IdMedicament order by Date desc";
                com.Parameters.Clear();
                com.Parameters.AddWithValue("IdMedicament", id);
                dr = com.ExecuteReader();
                medicament.PrescriptionList = new List<Prescription>();
                while (dr.Read()) {
                    medicament.PrescriptionList.Add(new Prescription {
                        Dose = int.Parse(dr["Dose"].ToString()),
                        Details = dr["Details"].ToString(),
                        Date = dr["Date"].ToString(),
                        DueDate = dr["DueDate"].ToString()
                    });
                }
                return medicament;
            }
        }

        public bool DeletePatient(int id) {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand()) {
                com.Connection = con;
                con.Open();
                try {
                    com.Transaction = con.BeginTransaction();

                    com.CommandText = "select 1 from dbo.Patient where Patient.IdPatient = @IdPatient";
                    com.Parameters.Clear();
                    com.Parameters.AddWithValue("IdPatient", id);
                    var dr = com.ExecuteReader();
                    if (!dr.Read()) {
                        return false;
                    }
                    dr.Close();

                    var idPrescriptionList = new List<int>();
                    com.CommandText = "select Prescription.IdPrescription from dbo.Prescription where Prescription.IdPatient = @IdPatient";
                    com.Parameters.Clear();
                    com.Parameters.AddWithValue("IdPatient", id);
                    dr = com.ExecuteReader();
                    while (dr.Read()) {
                        idPrescriptionList.Add(int.Parse(dr["IdPrescription"].ToString()));
                    }
                    dr.Close();

                    if (idPrescriptionList.Any()) {
                        foreach (var idPrescription in idPrescriptionList) {
                            com.CommandText = "delete from dbo.Prescription_Medicament where Prescription_Medicament.IdPrescription = @IdPrescription";
                            com.Parameters.Clear();
                            com.Parameters.AddWithValue("IdPrescription", idPrescription);
                            com.ExecuteNonQuery();

                            com.CommandText = "delete from dbo.Prescription where Prescription.IdPrescription = @IdPrescription";
                            com.Parameters.Clear();
                            com.Parameters.AddWithValue("IdPrescription", idPrescription);
                            com.ExecuteNonQuery();
                        }
                    }

                    com.CommandText = "delete from dbo.Patient where Patient.IdPatient = @IdPatient";
                    com.Parameters.Clear();
                    com.Parameters.AddWithValue("IdPatient", id);
                    com.ExecuteNonQuery();
                    
                    com.Transaction.Commit();
                    return true;
                } catch (SqlException) {
                    com.Transaction.Rollback();
                    throw;
                }
            }
        }
    }
}