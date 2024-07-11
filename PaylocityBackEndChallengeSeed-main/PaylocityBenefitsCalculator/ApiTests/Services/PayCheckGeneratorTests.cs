using System;
using System.Collections.Generic;
using Api.Models;
using Api.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ApiTests.Services;

public sealed class PaycheckGeneratorTests
{
    private readonly IEmployeeDataService _employeeDataService;
    private readonly IPayrollAdjustmentCalculator _mockedPayrollAdjustmentCalculator;
    private readonly IPayrollAdjustmentCalculator[] _payrollAdjustmentCalculators;
    private readonly IPayCheckGenerator _paycheckGenerator;

    public PaycheckGeneratorTests()
    {
        _employeeDataService = Substitute.For<IEmployeeDataService>();
        _mockedPayrollAdjustmentCalculator = Substitute.For<IPayrollAdjustmentCalculator>();
        _payrollAdjustmentCalculators = new[]
        {
            _mockedPayrollAdjustmentCalculator
        };
        _paycheckGenerator = new PayCheckGenerator(_employeeDataService, _payrollAdjustmentCalculators);
    }

    private static Employee GetEmployee()
    {
        return new()
        {
            DateOfBirth = DateTime.Today,
            Dependents = new List<Dependent>(),
            FirstName = "FirstName",
            Id = 123,
            LastName = "LastName",
            Salary = 52_000
        };
    }


    [Fact]
    public async void GeneratePaycheck_Should_FailIfEmployeeDoesNotExist()
    {
        Employee? employee = null;
        var mockAdjustment = new Adjustment
        {
            Name = "Mocked Adjustment",
            Amount = -100m
        };
        _employeeDataService.GetByIdAsync(0).ReturnsForAnyArgs(employee);
        _mockedPayrollAdjustmentCalculator.IsEligible(employee).ReturnsForAnyArgs(true);
        _mockedPayrollAdjustmentCalculator.Execute(employee).ReturnsForAnyArgs(mockAdjustment.Amount);
        _mockedPayrollAdjustmentCalculator.Name.Returns(mockAdjustment.Name);

        var paycheckResult = await _paycheckGenerator.GeneratePaycheck(100);

        _mockedPayrollAdjustmentCalculator.DidNotReceiveWithAnyArgs().IsEligible(employee);
        _mockedPayrollAdjustmentCalculator.DidNotReceiveWithAnyArgs().Execute(employee);
        paycheckResult.Should().NotBeNull();
        paycheckResult.IsSuccess.Should().BeFalse();
        paycheckResult.Data.Should().BeNull();
    }

    [Fact]
    public async void GeneratePaycheck_Should_PassEvenIfAdjustmentsAreNotAdded()
    {
        var employee = GetEmployee();
        var mockAdjustment = new Adjustment
        {
            Name = "Mocked Adjustment",
            Amount = -100m
        };
        _employeeDataService.GetByIdAsync(0).ReturnsForAnyArgs(employee);
        _mockedPayrollAdjustmentCalculator.IsEligible(employee).ReturnsForAnyArgs(false);
        _mockedPayrollAdjustmentCalculator.Execute(employee).ReturnsForAnyArgs(mockAdjustment.Amount);
        _mockedPayrollAdjustmentCalculator.Name.Returns(mockAdjustment.Name);

        var paycheckResult = await _paycheckGenerator.GeneratePaycheck(employee.Id);

        _mockedPayrollAdjustmentCalculator.Received().IsEligible(employee);
        _mockedPayrollAdjustmentCalculator.DidNotReceiveWithAnyArgs().Execute(employee);
        paycheckResult.Should().NotBeNull();
        paycheckResult.IsSuccess.Should().BeTrue();

        var paycheck = paycheckResult.Data!;
        paycheck.BasePay.Should().Be(employee.Salary / 26);
        paycheck.Adjustments.Should().BeEmpty();
        paycheck.Employee.Should().Be(employee);
        paycheck.NetPay.Should().Be(employee.Salary / 26);
    }

    [Fact]
    public async void GeneratePaycheck_Should_ReturnExpectedData()
    {
        var employee = GetEmployee();
        var mockAdjustment = new Adjustment
        {
            Name = "Mocked Adjustment",
            Amount = -100m
        };
        _employeeDataService.GetByIdAsync(0).ReturnsForAnyArgs(employee);
        _mockedPayrollAdjustmentCalculator.IsEligible(employee).ReturnsForAnyArgs(true);
        _mockedPayrollAdjustmentCalculator.Execute(employee).ReturnsForAnyArgs(mockAdjustment.Amount);
        _mockedPayrollAdjustmentCalculator.Name.Returns(mockAdjustment.Name);

        var paycheckResult = await _paycheckGenerator.GeneratePaycheck(employee.Id);

        _mockedPayrollAdjustmentCalculator.Received().IsEligible(employee);
        _mockedPayrollAdjustmentCalculator.Received().Execute(employee);
        paycheckResult.Should().NotBeNull();
        paycheckResult.IsSuccess.Should().BeTrue();

        var paycheck = paycheckResult.Data!;
        paycheck.BasePay.Should().Be(employee.Salary / 26);
        paycheck.Adjustments.Should().HaveCount(1);
        paycheck.Adjustments[0].Name.Should().Be(mockAdjustment.Name);
        paycheck.Adjustments[0].Amount.Should().Be(mockAdjustment.Amount);
        paycheck.Employee.Should().Be(employee);
        paycheck.NetPay.Should().Be((employee.Salary / 26) + mockAdjustment.Amount);
    }
}