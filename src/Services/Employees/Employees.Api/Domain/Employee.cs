using NexusHR.Shared.Kernel;

namespace Employees.Api.Domain;

public class Employee : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string? Contacts { get; private set; }
    public WorkStatus Status { get; private set; }

    public Employee(string firstName, string lastName, string email, string? contacts = null)
    {
        // валидация name surname, думаю это можно сделать до создания сущности

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Contacts = contacts;

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
    Fired = 2
}