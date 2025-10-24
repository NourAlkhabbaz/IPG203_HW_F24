using System;
using System.Collections.Generic;

// Interface for hotel entities
public interface IHotelEntity
{
    void DisplayInfo();
    void PerformAction();
}

// Abstract base class for all hotel members/items
public abstract class HotelEntity : IHotelEntity
{
    private string name;
    private int id;

    // Delegate and Event for alerts
    public delegate void AlertHandler(string message);
    public event AlertHandler OnAlert;

    public HotelEntity(string name, int id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");
        if (id <= 0)
            throw new ArgumentException("ID must be positive");

        this.name = name;
        this.id = id;

        HotelStatistics.IncrementTotalEntities();
    }

    public string Name
    {
        get => name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty");
            name = value;
        }
    }

    public int ID
    {
        get => id;
        private set
        {
            if (value <= 0)
                throw new ArgumentException("ID must be positive");
            id = value;
        }
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name} | ID: {ID}");
    }

    protected void TriggerAlert(string message)
    {
        OnAlert?.Invoke($"[ALERT for {Name}]: {message}");
    }

    public abstract void PerformAction();
}

// Guest class
public class Guest : HotelEntity
{
    public int RoomNumber { get; private set; }
    public int Nights { get; private set; }

    public Guest(string name, int id, int roomNumber, int nights)
        : base(name, id)
    {
        if (roomNumber <= 0)
            throw new ArgumentException("Invalid room number");
        if (nights <= 0)
            throw new ArgumentException("Nights must be positive");

        RoomNumber = roomNumber;
        Nights = nights;
    }

    public override void PerformAction()
    {
        Console.WriteLine($"{Name} is staying in room {RoomNumber} for {Nights} nights.");
        if (Nights > 10)
            TriggerAlert("Long stay detected, consider offering a discount.");
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Room: {RoomNumber} | Nights: {Nights}");
    }
}

// Employee class
public class Employee : HotelEntity
{
    public string Position { get; set; }

    public Employee(string name, int id, string position)
        : base(name, id)
    {
        Position = position;
    }

    public override void PerformAction()
    {
        Console.WriteLine($"{Name} is performing their duties as {Position}.");
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Position: {Position}");
    }
}

// Room class
public class Room : HotelEntity
{
    public bool IsOccupied { get; private set; }

    public Room(string name, int id, bool occupied)
        : base(name, id)
    {
        IsOccupied = occupied;
    }

    public override void PerformAction()
    {
        string status = IsOccupied ? "occupied" : "available";
        Console.WriteLine($"Room {Name} (ID: {ID}) is currently {status}.");
        if (!IsOccupied)
            TriggerAlert("Room is available for booking.");
    }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Occupied: {IsOccupied}");
    }
}

// Class managing all hotel entities
public class Hotel
{
    private List<HotelEntity> entities = new List<HotelEntity>();

    public void AddEntity(HotelEntity entity)
    {
        entities.Add(entity);
        entity.OnAlert += HandleAlert;
    }

    private void HandleAlert(string message)
    {
        Console.WriteLine(message);
    }

    public void ShowAll()
    {
        Console.WriteLine("\n=== Hotel Records ===");
        foreach (var e in entities)
        {
            e.DisplayInfo();
            Console.WriteLine();
        }
    }

    public void PerformAllActions()
    {
        Console.WriteLine("\n=== Performing Hotel Actions ===");
        foreach (var e in entities)
        {
            e.PerformAction();
        }
    }
}

// Static class for statistics
public static class HotelStatistics
{
    public static int TotalEntities { get; private set; } = 0;

    public static void IncrementTotalEntities()
    {
        TotalEntities++;
    }
}

// Main program
public class Program
{
    public static void Main()
    {
        var hotel = new Hotel();

        var g1 = new Guest("Ali Ahmad", 101, 203, 3);
        var g2 = new Guest("Sara Ibrahim", 102, 305, 12);
        var e1 = new Employee("Omar Khaled", 201, "Receptionist");
        var e2 = new Employee("Lina Hasan", 202, "Housekeeper");
        var r1 = new Room("Room-203", 301, true);
        var r2 = new Room("Room-305", 302, false);

        hotel.AddEntity(g1);
        hotel.AddEntity(g2);
        hotel.AddEntity(e1);
        hotel.AddEntity(e2);
        hotel.AddEntity(r1);
        hotel.AddEntity(r2);

        Console.WriteLine($"Total Entities: {HotelStatistics.TotalEntities}\n");

        hotel.ShowAll();

        hotel.PerformAllActions();
    }
}
