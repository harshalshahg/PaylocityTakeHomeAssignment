using Api.Models;

namespace Api.Services;

public sealed class HighSalaryDeductionCalculator : PayrollAdjustmentCalculatorBase
{
    public override string Name => "High Salary Deductions";

    protected override decimal InvokeCalculation(Employee employee)
    {
        // deduct 2% of the employee's annual salary
        var annualCost = employee.Salary * 0.02m;
        var costPerCheck = -1 * CalculateFromAnnualCost(annualCost);

        return costPerCheck;
    }

    public override bool IsEligible(Employee employee) => employee != null && employee.Salary >= 80_000m;
}