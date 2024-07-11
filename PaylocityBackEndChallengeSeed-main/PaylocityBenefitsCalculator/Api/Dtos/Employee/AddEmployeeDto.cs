using Api.Dtos.Dependent;

namespace Api.Dtos.Employee;

public sealed class AddEmployeeDto : EmployeeDtoBase
{
    public ICollection<AddDependentDto> Dependents { get; set; } = new List<AddDependentDto>();
}