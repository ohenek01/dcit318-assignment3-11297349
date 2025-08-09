using System;
using System.Collections.Generic;
using System.Linq;

public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => items;

    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

    public bool Remove(Func<T, bool> predicate)
    {
        var item = GetById(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }
}

public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }
}

public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "Ohene Kwadwo", 20, "Male"));
        _patientRepo.Add(new Patient(2, "Paa Kofi", 20, "Male"));

        _prescriptionRepo.Add(new Prescription(1, 1, "Ibrupofen", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(2, 1, "Multivitamins", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(3, 2, "Amoxicillin", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(4, 2, "Paracetamol", DateTime.Now));
    }

    public void BuildPrescriptionMap()
    {
        var allPrescriptions = _prescriptionRepo.GetAll();
        _prescriptionMap = allPrescriptions
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("Patients:");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
        }
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        if (_prescriptionMap.TryGetValue(id, out var prescriptions))
        {
            Console.WriteLine($"\nPrescriptions for Patient ID {id}");
            foreach (var prescription in prescriptions)
            {
                Console.WriteLine($"- {prescription.MedicationName} (Issued: {prescription.DateIssued: yyyy-MM-dd})");
            }
        }
        else
        {
            Console.WriteLine($"No prescriptions found for Patient ID {id}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var app = new HealthSystemApp();

        app.SeedData();

        app.BuildPrescriptionMap();

        app.PrintAllPatients();

        app.PrintPrescriptionsForPatient(1);
    }
}