using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Mappers;
using Api.Services;


namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeDataService _employeeDataService;
    private readonly IGetEmployeeDtoMapper _getEmployeeDtoMapper;
    private readonly IAddEmployeeMapper _createEmployeeDtoMapper;

    public EmployeesController(IEmployeeDataService employeeDataService,
                               IGetEmployeeDtoMapper getEmployeeDtoMapper,
                               IAddEmployeeMapper createEmployeeDtoMapper)
    {
        _employeeDataService = employeeDataService;
        _getEmployeeDtoMapper = getEmployeeDtoMapper;
        _createEmployeeDtoMapper = createEmployeeDtoMapper;
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        var employee = await _employeeDataService.GetByIdAsync(id);
        if (employee is null)
            return NotFound();

        var getEmployeeDto = _getEmployeeDtoMapper.Map(employee);
        return ApiResponse<GetEmployeeDto>.BuildSuccess(getEmployeeDto);
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
    {
        var employees = await _employeeDataService.GetAllAsync();
        var employeeDtos = _getEmployeeDtoMapper.Map(employees).ToList();
        var result = ApiResponse<List<GetEmployeeDto>>.BuildSuccess(employeeDtos);
        return result;
    }

    // Add Employee
    [SwaggerOperation(Summary = "Add a new employee")]
    [HttpPost("")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Post([FromBody] AddEmployeeDto postEmployeeDto)
    {
        var employee = _createEmployeeDtoMapper.Map(postEmployeeDto);
        var creationResult = await _employeeDataService.Add(employee);
        if (!creationResult.IsSuccess)
            return BadRequest(creationResult.Error);

        var getEmployeeDto = _getEmployeeDtoMapper.Map(creationResult.Data!);
        return ApiResponse<GetEmployeeDto>.BuildSuccess(getEmployeeDto);
    }
}
