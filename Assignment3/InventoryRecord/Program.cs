using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public record InventoryItem (int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

public interface IInventoryEntity
{
    int Id { get; }
}

public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll() => _log;

    public void SaveToFile()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_log, options);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Loading from file: {ex.Message}");
        }
    }
}

public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp()
    {
        _logger = new InventoryLogger<InventoryItem>("inventory.json");
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Phone", 3, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Laptop", 2, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Monitor", 5, DateTime.Now));
    }

    public void SaveData() => _logger.SaveToFile();
    public void LoadData() => _logger.LoadFromFile();
    public void PrintAllItems()
    {
        Console.WriteLine("Inventory Items:");
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded: yyyy-MM-dd}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var app = new InventoryApp();

        app.SeedSampleData();

        app.SaveData();
        Console.WriteLine("Saved. Clearing memory");

        var newApp = new InventoryApp();

        newApp.LoadData();
        
        newApp.PrintAllItems();
    }
}