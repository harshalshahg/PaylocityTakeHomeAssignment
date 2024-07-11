using Api.Models;

namespace Api.Repositories;

public interface IDependentRepository : IRepository<Dependent>
{

}

public sealed class DependentRepository : IDependentRepository
{
    private readonly BenefitsDbContext _context;

    public DependentRepository(BenefitsDbContext context)
    {
        _context = context;
    }

    // add
    public async Task<Dependent> CreateAsync(Dependent newDependent)
    {
        //adding this hack to remove warning because of data getting inserted in in-memory db context
        await Task.CompletedTask;

        var nextId = _context.Dependents.Max(d => d.Id) + 1;
        newDependent.Id = nextId;

        _context.Dependents.Add(newDependent);
        return newDependent;
    }

    public async Task<IEnumerable<Dependent>> CreateMultipleAsync(IEnumerable<Dependent> newDependents)
    {
        var createdDependents = new List<Dependent>();
        foreach (var newDependent in newDependents) { 
            createdDependents.Add(await CreateAsync(newDependent));
        }
        return createdDependents;
    }

    // read
    public async Task<IQueryable<Dependent>> GetAllAsync()
    {
        await Task.CompletedTask;
        return _context.Dependents.AsQueryable();
    }

    public async Task<Dependent?> GetByIdAsync(int id)
    {
        await Task.CompletedTask;
        return _context.Dependents.FirstOrDefault(d => d.Id == id);
    }
}
