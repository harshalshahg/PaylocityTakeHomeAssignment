using Api.Models;
using Api.Repositories;
using Api.Validators;

namespace Api.Services;

public interface IDependentDataService
{
	// add
	Task<ValidationResult<Dependent>> Add(Dependent newDependent);

	// read
	Task<Dependent[]> GetAllAsync();
	Task<Dependent?> GetByIdAsync(int id);

}

public sealed class DependentDataService : IDependentDataService
{
	private readonly IDependentRepository _dependentRepository;
	private readonly IEmployeeRepository _employeeRepository;
	private readonly IDependentValidator _dependentValidator;

	public DependentDataService(IDependentRepository dependentRepository,
								IEmployeeRepository employeeRepository,
								IDependentValidator dependentValidator)
	{
		_dependentRepository = dependentRepository;
		_employeeRepository = employeeRepository;
		_dependentValidator = dependentValidator;
	}

    #region Methods
    // add
    public async Task<ValidationResult<Dependent>> Add(Dependent newDependent)
    {
        try
        {

            var validationResult = await _dependentValidator.Validate(newDependent);
            if (!validationResult.IsSuccess)
                return validationResult;

            await _dependentRepository.CreateAsync(validationResult.Data!);
            return validationResult;
        }
        catch (Exception)
        {
            throw;
        }
    }

    // read
    public async Task<Dependent[]> GetAllAsync()
    {
        try
        {
            var dependents = (await _dependentRepository.GetAllAsync()).ToArray();
            foreach (var dependent in dependents)
            {
                dependent.Employee = await _employeeRepository.GetByIdAsync(dependent.EmployeeId);
            }
            return dependents;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Dependent?> GetByIdAsync(int id)
    {
        try
        {
            var dependent = await _dependentRepository.GetByIdAsync(id);
            if (dependent is null)
                return null;

            dependent.Employee = await _employeeRepository.GetByIdAsync(dependent.EmployeeId);
            return dependent;
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion

}