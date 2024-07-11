using Api.Dtos.Dependent;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Mappers;
using Api.Services;


namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class DependentsController : ControllerBase
{
    private readonly IDependentDataService _dependentDataService;
    private readonly IGetDependentDtoMapper _getDependentDtoMapper;
    private readonly IAddDependentMapper _createDependentDtoMapper;

    public DependentsController(IDependentDataService dependentDataService,
                                IGetDependentDtoMapper getDependentDtoMapper,
                                IAddDependentMapper createDependentDtoMapper)
    {
        _dependentDataService = dependentDataService;
        _getDependentDtoMapper = getDependentDtoMapper;
        _createDependentDtoMapper = createDependentDtoMapper;
    }

    [SwaggerOperation(Summary = "Get dependent by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get(int id)
    {
        var dependent = await _dependentDataService.GetByIdAsync(id);
        if (dependent is null)
            return NotFound();

        var getDependentDto = _getDependentDtoMapper.Map(dependent);
        return ApiResponse<GetDependentDto>.BuildSuccess(getDependentDto);
    }

    [SwaggerOperation(Summary = "Get all dependents")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAll()
    {
        var dependents = await _dependentDataService.GetAllAsync();
        var dependentDtos = _getDependentDtoMapper.Map(dependents).ToList();
        return ApiResponse<List<GetDependentDto>>.BuildSuccess(dependentDtos);
    }

    // Add Dependent
    [SwaggerOperation(Summary = "Create a dependent and associate them with an employee")]
    [HttpPost("")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Post([FromBody] AddDependentDto createDependentDto)
    {
        var dependent =  _createDependentDtoMapper.Map(createDependentDto);
        var creationResult = await _dependentDataService.Add(dependent);
        if (!creationResult.IsSuccess)
            return BadRequest(creationResult.Error);

        var getDependentDto = _getDependentDtoMapper.Map(creationResult.Data!);
        return ApiResponse<GetDependentDto>.BuildSuccess(getDependentDto);
    }
}
