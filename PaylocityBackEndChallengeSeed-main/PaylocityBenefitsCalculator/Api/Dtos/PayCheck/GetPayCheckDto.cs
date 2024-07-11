﻿using Api.Dtos.Employee;

namespace Api.Dtos.Paycheck;

public sealed class GetPayCheckDto : IDto
{
    public decimal BasePay { get; set; }
    public decimal NetPay { get; set; }
    public GetAdjustmentDto[] Adjustments { get; set; } = Array.Empty<GetAdjustmentDto>();
    public GetEmployeeDto Employee { get; set; } = null!;
}