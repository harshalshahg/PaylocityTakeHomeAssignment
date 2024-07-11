using Api.Dtos.Dependent;
using Api.Models;

namespace Api.Mappers;

public interface IAddDependentMapper : IEntityMapper<Dependent, AddDependentDto>
{

}

public sealed class AddDependentMapper : EntityMapperBase<Dependent, AddDependentDto>, IAddDependentMapper
{
    public override Dependent Map(AddDependentDto dto)
    {
        return new Dependent
        {
            DateOfBirth = dto.DateOfBirth,
            Employee = null,
            EmployeeId = dto.EmployeeId,
            FirstName = dto.FirstName,
            Id = 0,
            LastName = dto.LastName,
            Relationship = dto.Relationship
        };
    }
}