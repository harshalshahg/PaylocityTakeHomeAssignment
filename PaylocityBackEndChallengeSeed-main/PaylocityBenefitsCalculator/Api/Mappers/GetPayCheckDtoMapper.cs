using Api.Dtos.Paycheck;
using Api.Models;

namespace Api.Mappers;

public interface IGetPayCheckDtoMapper : IDtoMapper<GetPayCheckDto, PayCheck>
{

}

public sealed class GetPayCheckDtoMapper : DtoMapperBase<GetPayCheckDto, PayCheck>, IGetPayCheckDtoMapper
{
    private readonly IGetAdjustmentDtoMapper _adjustmentMapper;
    private readonly IGetEmployeeDtoMapper _employeeMapper;

    public GetPayCheckDtoMapper(IGetAdjustmentDtoMapper adjustmentMapper,
                                IGetEmployeeDtoMapper employeeMapper)
    {
        _adjustmentMapper = adjustmentMapper;
        _employeeMapper = employeeMapper;
    }

    public override GetPayCheckDto Map(PayCheck entity)
    {
        return new GetPayCheckDto
        {
            Adjustments = _adjustmentMapper.Map(entity.Adjustments).ToArray(),
            BasePay = entity.BasePay,
            Employee = _employeeMapper.Map(entity.Employee),
            NetPay = entity.NetPay
        };
    }
}