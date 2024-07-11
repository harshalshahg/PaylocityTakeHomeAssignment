using Api.Models;

namespace Api.Repositories;

public interface IRepository<T>
	where T : IEntity
{
	// add
	Task<T> CreateAsync(T newEntity);
	Task<IEnumerable<T>> CreateMultipleAsync(IEnumerable<T> newEntities);

	// read
	Task<IQueryable<T>> GetAllAsync();
	Task<T?> GetByIdAsync(int id);

}