using Api.Mappers;
using Api.Repositories;
using Api.Services;
using Api.Validators;

namespace Api.Initialization;

public static class DependencyRegistration
{
    public static void RegisterDependencies(IServiceCollection services)
    {
        RegisterDataAccessLayer(services);
        RegisterMappers(services);
        RegisterPayrollServices(services);
        RegisterValidators(services);
    }

    private static void RegisterDataAccessLayer(IServiceCollection services)
    {
        services
            .AddSingleton<BenefitsDbContext>(sp =>
            {
                var context = new BenefitsDbContext();
                var seeder = new DataSeeder(context);
                seeder.SeedDatabase();
                return context;
            })
            .AddSingleton<IDependentDataService, DependentDataService>()
            .AddSingleton<IEmployeeDataService, EmployeeDataService>()
            .AddSingleton<IDependentRepository, DependentRepository>()
            .AddSingleton<IEmployeeRepository, EmployeeRepository>()
            ;
    }

    private static void RegisterMappers(IServiceCollection services)
    {
        services
            // Dependent Mappers
            .AddTransient<IGetDependentDtoMapper, GetDependentDtoMapper>()
            .AddTransient<IAddDependentMapper, AddDependentMapper>()

            // Employee Mappers
            .AddTransient<IGetEmployeeDtoMapper, GetEmployeeDtoMapper>()
            .AddTransient<IAddEmployeeMapper, AddEmployeeMapper>()

            // Paycheck Mappers
            .AddTransient<IGetAdjustmentDtoMapper, GetAdjustmentDtoMapper>()
            .AddTransient<IGetPayCheckDtoMapper, GetPayCheckDtoMapper>()
            ;
    }

    private static void RegisterPayrollServices(IServiceCollection services)
    {
        services
            .AddScoped(sp => new IPayrollAdjustmentCalculator[]
                {
                    new EmployeeBenefitDeductionCalculator(),
                    new DependentBenefitDeductionCalculator(),
                    new HighSalaryDeductionCalculator(),
                    new AgeDeductionCalculator()
                })
            .AddScoped<IPayCheckGenerator, PayCheckGenerator>()
            ;
    }

    private static void RegisterValidators(IServiceCollection services)
    {
        services
            .AddTransient<IDependentValidator, DependentValidator>()
            .AddTransient<IEmployeeValidator, EmployeeValidator>() ;
    }
}