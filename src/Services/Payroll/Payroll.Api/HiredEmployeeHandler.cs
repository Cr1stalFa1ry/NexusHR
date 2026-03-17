using Employees.Contracts;
using Employees.Contracts.Events;
using Payroll.Api.Context;
using Payroll.Api.Domain;

namespace Payroll.Api;

public class HiredEmployeeHandler
{
    public async static Task<BonusAwarded> Handle(HiredEmployee message, PayrollDbContext dbContext)
    {
        var payrollProfile = new PayrollProfile(message.EmployeeId, 5000);
        
        dbContext.PayrollProfiles.Add(payrollProfile);
        await dbContext.SaveChangesAsync();

        return new BonusAwarded(message.EmployeeId, 200, "Welcome Bonus");
    }
}
