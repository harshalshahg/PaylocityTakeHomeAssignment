﻿using Api.Models;

namespace Api.Services;

public sealed class EmployeeBenefitDeductionCalculator : PayrollAdjustmentCalculatorBase
{
    public override string Name => "Employee Benefits Deductions";

    protected override decimal InvokeCalculation(Employee employee)
    {
        // deduct $1000 per month
        var charge = CalculateFromMonthlyCost(1000m);
        return -1 * charge;
    }

    public override bool IsEligible(Employee employee) => true;
}