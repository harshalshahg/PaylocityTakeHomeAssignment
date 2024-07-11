using Api.Dtos.Employee;
using Api.Models;

namespace Api.Mappers;

public interface IAddEmployeeMapper : IEntityMapper<Employee, AddEmployeeDto>
{

}

public sealed class AddEmployeeMapper : EntityMapperBase<Employee, AddEmployeeDto>, IAddEmployeeMapper
{
    private readonly IAddDependentMapper _dependentMapper;

    public AddEmployeeMapper(IAddDependentMapper dependentMapper)
    {
        _dependentMapper = dependentMapper;
    }

    public override Employee Map(AddEmployeeDto dto)
    {
        return new Employee
        {
            DateOfBirth = dto.DateOfBirth,
            Dependents = _dependentMapper.Map(dto.Dependents).ToList(),
            FirstName = dto.FirstName,
            Id = 0,
            LastName = dto.LastName,
            Salary = dto.Salary
        };
    }
}