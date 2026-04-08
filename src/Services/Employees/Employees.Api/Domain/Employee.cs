using NexusHR.Shared.Kernel;

namespace Employees.Api.Domain;

public class Employee : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string? Contacts { get; private set; }
    public WorkStatus Status { get; private set; }
    public string Position { get; private set; }
    public string Department { get; private set; }
    public decimal Salary { get; private set; }
    public DateTimeOffset HireDate { get; private set; }
    public Guid? ManagerId { get; private set; }

    public Employee(
        string firstName, 
        string lastName, 
        string email, 
        string position, 
        string department, 
        decimal salary,
        string? contacts = null,
        Guid? managerId = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Position = position;
        Department = department;
        Salary = salary;
        Contacts = contacts;
        ManagerId = managerId;
        
        HireDate = DateTimeOffset.UtcNow;
        Status = WorkStatus.Working;
    }

    public void UpdateProfile(string firstName, string lastName, string email, string? contacts = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Contacts = contacts;

        MarkUpdated();
    }

    public void Fire()
    {
        if (Status == WorkStatus.Fired)
           return;

        Status = WorkStatus.Fired;
        MarkUpdated();
    }
}

public enum WorkStatus
{
    Working = 1,
    Fired = 2,
    OnVacation = 3
}

//? имеет ли смысл добавлять enum ?
public enum LevelPosition
{
    Intern = 0,
    Junior = 1,
    Middle = 2,
    Senior = 3,
    Lead = 4
}