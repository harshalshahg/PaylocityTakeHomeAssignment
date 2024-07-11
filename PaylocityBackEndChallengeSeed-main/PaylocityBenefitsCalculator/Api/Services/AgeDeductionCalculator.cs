using Api.Models;

namespace Api.Services;

public sealed class AgeDeductionCalculator : PayrollAdjustmentCalculatorBase
{
    public override string Name => "Age Deductions";

    protected override decimal InvokeCalculation(Employee employee)
    {
        // -$200 per month
        var charge = CalculateFromMonthlyCost(200m);
        return -1 * charge;
    }

    public override bool IsEligible(Employee employee)
    {
        if (employee == null) return false;
        var age = DateTime.Today.Year - employee.DateOfBirth.Year;
        if (employee.DateOfBirth.Date > DateTime.Today.AddYears(-1 * age))
            age--;

        return age >= 50;
    }
}