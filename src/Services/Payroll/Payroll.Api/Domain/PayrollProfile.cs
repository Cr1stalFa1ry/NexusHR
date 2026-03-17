using NexusHR.Shared.Kernel;

namespace Payroll.Api.Domain;

public class PayrollProfile : Entity
{
    public Guid EmployeeId { get; private set; }
    public decimal BaseSalary { get; private set; }
    public string Currency { get; private set; } = "USD";
    public bool IsActive { get; private set; }

    private PayrollProfile() { }

    public PayrollProfile(Guid employeeId, decimal baseSalary = 0)
    {
        EmployeeId = employeeId;
        BaseSalary = baseSalary;
        IsActive = true;
    }

    public void UpdateSalary(decimal newSalary)
    {
        // тут должны быть логика допуска к изменению зп
        MarkUpdated();
    }
}