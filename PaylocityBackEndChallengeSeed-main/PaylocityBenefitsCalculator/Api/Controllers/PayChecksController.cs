using Api.Dtos.Paycheck;
using Api.Mappers;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class PaychecksController : ControllerBase
{
    private readonly IGetPayCheckDtoMapper _paycheckMapper;
    private readonly IPayCheckGenerator _paycheckGenerator;

    public PaychecksController(IGetPayCheckDtoMapper paycheckMapper,
                               IPayCheckGenerator paycheckGenerator)
    {
        _paycheckMapper = paycheckMapper;
        _paycheckGenerator = paycheckGenerator;
    }

    [SwaggerOperation(Summary = "Retrieves a paycheck for the given employee")]
    [HttpGet("employee/{employeeId}")]

    public async Task<ActionResult<ApiResponse<GetPayCheckDto>>> Get(int employeeId)
    {
        var paycheckResult = await _paycheckGenerator.GeneratePaycheck(employeeId);
        if (!paycheckResult.IsSuccess)
            return BadRequest(paycheckResult.Error);

        var getPaycheckDto = _paycheckMapper.Map(paycheckResult.Data!);
        return ApiResponse<GetPayCheckDto>.BuildSuccess(getPaycheckDto);
    }

}