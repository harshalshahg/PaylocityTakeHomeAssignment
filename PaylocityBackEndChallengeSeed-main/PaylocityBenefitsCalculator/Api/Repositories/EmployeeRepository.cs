using Api.Models;

namespace Api.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
}

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly BenefitsDbContext _context;

    public EmployeeRepository(BenefitsDbContext context)
    {
        _context = context;
    }

    // add
    public async Task<Employee> CreateAsync(Employee newEmployee)
    {
        await Task.CompletedTask;
        var nextId = _context.Employees.Max(e => e.Id) + 1;
        newEmployee.Id = nextId;

        _context.Employees.Add(newEmployee);
        return newEmployee;
    }

    public  async Task<IEnumerable<Employee>> CreateMultipleAsync(IEnumerable<Employee> newEmployees)
    {
        var createdEmployees = new List<Employee>();
        foreach (var employee in newEmployees)
            createdEmployees.Add(await CreateAsync(employee));

        return createdEmployees;
    }

    // reading the data
    public async Task<IQueryable<Employee>> GetAllAsync()
    {
        await Task.CompletedTask;
        return _context.Employees.AsQueryable();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        await Task.CompletedTask;
        return _context.Employees.FirstOrDefault(e => e.Id == id);
    }

}