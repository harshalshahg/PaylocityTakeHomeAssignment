using Api.Models;

namespace Api.Services;

public interface IPayCheckGenerator
{
    Task<ValidationResult<PayCheck>> GeneratePaycheck(int employeeId);
}

public sealed class PayCheckGenerator : IPayCheckGenerator
{
    private readonly IEmployeeDataService _employeeDataService;
    private readonly IPayrollAdjustmentCalculator[] _payrollAdjustmentCalculators;

    public PayCheckGenerator(IEmployeeDataService employeeDataService,
                             IPayrollAdjustmentCalculator[] payrollAdjustmentCalculators)
    {
        _employeeDataService = employeeDataService;
        _payrollAdjustmentCalculators = payrollAdjustmentCalculators;
    }

    #region Methods

    #region Private Methods

    private IEnumerable<Adjustment> CalculateAdjustments(Employee employee)
    {
        foreach (var payrollAdjustmentCalculator in _payrollAdjustmentCalculators)
        {
            if (!payrollAdjustmentCalculator.IsEligible(employee))
                continue;

            var adjustment = new Adjustment
            {
                Amount = payrollAdjustmentCalculator.Execute(employee),
                Name = payrollAdjustmentCalculator.Name
            };
            yield return adjustment;
        }
    }
    #endregion

    public async Task<ValidationResult<PayCheck>> GeneratePaycheck(int employeeId)
    {
        try
        {

            var employee = await _employeeDataService.GetByIdAsync(employeeId);
            if (employee is null)
                return ValidationResult<PayCheck>.GetFailure($"Employee with ID {employeeId} not found");

            var basePay = employee.Salary / PayPeriodSettings.CHECKS_PER_YEAR;
            var adjustments = CalculateAdjustments(employee);

            var paycheck = new PayCheck
            {
                Adjustments = adjustments.ToArray(),
                BasePay = basePay,
                Employee = employee
            };
            return ValidationResult<PayCheck>.GetSuccess(paycheck);
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion
}