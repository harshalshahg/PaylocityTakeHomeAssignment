using Api.Models;
using Api.Repositories;
using Api.Validators;

namespace Api.Services;


public interface IEmployeeDataService
{
    // add
    Task<ValidationResult<Employee>> Add(Employee newEmployee);

    // read
    Task<Employee[]> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
}

public sealed class EmployeeDataService : IEmployeeDataService
{

    private readonly IDependentRepository _dependentRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeValidator _employeeValidator;

    public EmployeeDataService(IDependentRepository dependentRepository,
                               IEmployeeRepository employeeRepository,
                               IEmployeeValidator employeeValidator)
    {
        _dependentRepository = dependentRepository;
        _employeeRepository = employeeRepository;
        _employeeValidator = employeeValidator;
    }

    #region  Methods

    public async Task<ValidationResult<Employee>> Add(Employee newEmployee)
    {
        try
        {
            var validationResult = _employeeValidator.Validate(newEmployee);
            if (!validationResult.IsSuccess)
                return validationResult;

            if (validationResult.Data == null) throw new ArgumentNullException(nameof(validationResult), "No valid employee was returned");

            await _employeeRepository.CreateAsync(validationResult.Data);
            validationResult.Data.Dependents.Select(x =>
            {
                x.EmployeeId = validationResult.Data!.Id;
                return x;
            });

            await _dependentRepository.CreateMultipleAsync(validationResult.Data.Dependents);

            return validationResult;
        }
        catch (Exception)
        {
            throw;
        }

    }

    // getting the data
    public async Task<Employee[]> GetAllAsync()
    {
        try
        {

            var employees = (await _employeeRepository.GetAllAsync()).ToArray();
            foreach (var employee in employees)
            {
                employee.Dependents = (await _dependentRepository.GetAllAsync()).Where(d => d.EmployeeId == employee.Id).ToList();
            }
            return employees;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        try
        {

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee is null)
                return null;

            employee.Dependents = (await _dependentRepository.GetAllAsync()).Where(d => d.EmployeeId == employee.Id).ToList();
            return employee;
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion

}