using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public interface IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    int Quantity { get; set; }
}

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }
}

public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"{item.Id} already exists");
        }
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item))
        {
            throw new ItemNotFoundException($"{id} not found");
        }
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException($"{id} not found");
        }
        _items.Remove(id);
    }

    public List<T> GetAllItems() => _items.Values.ToList();

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException("Quantity can't be negative");
        }
        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public InventoryRepository<ElectronicItem> Electronics => _electronics;
    public InventoryRepository<GroceryItem> Groceries => _groceries;

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Phone", 3, "Apple", 24));
        _electronics.AddItem(new ElectronicItem(2, "Laptop", 2, "HP", 12));

        _groceries.AddItem(new GroceryItem(1, "Juice", 4, DateTime.Now.AddMonths(24)));
        _groceries.AddItem(new GroceryItem(2, "Bread", 5, DateTime.Now.AddDays(3)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        Console.WriteLine($"\n{typeof(T).Name}s");
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
            if (item is ElectronicItem electronic)
            {
                Console.WriteLine($"Brand: {electronic.Brand}, Warranty: {electronic.WarrantyMonths}. months");
            }
            else if (item is GroceryItem grocery)
            {
                Console.WriteLine($"Expiry date: {grocery.ExpiryDate:yyyy-MM-dd}");
            }
        }
    }
    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Stock increased for item {id}. New quantity: {item.Quantity + quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error increasing stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item {id} removed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var app = new WareHouseManager();

        app.SeedData();

        app.PrintAllItems(app.Electronics);
        app.PrintAllItems(app.Groceries);

        try
        {
            app.Electronics.AddItem(new ElectronicItem(1, "Laptop", 2, "HP", 12));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Duplicate item: {ex.Message}");
        }

        app.RemoveItemById(app.Groceries, 9);

        try
        {
            app.Electronics.UpdateQuantity(2, -9);
        }
        catch(InvalidQuantityException ex)
        {
            Console.WriteLine($"Invalid quantity: {ex.Message}");
        }
    }
}