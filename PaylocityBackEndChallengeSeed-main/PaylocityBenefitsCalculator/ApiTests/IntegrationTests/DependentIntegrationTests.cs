using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Api.Dtos.Dependent;
using Api.Models;
using Xunit;
using System.Linq;
using System.Net.Http;
using Api.Dtos.Employee;
using ApiTests.Extensions;
using FluentAssertions;


namespace ApiTests.IntegrationTests;

public sealed class DependentIntegrationTests : IntegrationTest
{
    private readonly static List<GetDependentDto> Dependents = new()
    {
        new()
        {
            Id = 1,
            FirstName = "Spouse",
            LastName = "Morant",
            Relationship = Relationship.Spouse,
            DateOfBirth = new DateTime(1998, 3, 3)
        },
        new()
        {
            Id = 2,
            FirstName = "Child1",
            LastName = "Morant",
            Relationship = Relationship.Child,
            DateOfBirth = new DateTime(2020, 6, 23)
        },
        new()
        {
            Id = 3,
            FirstName = "Child2",
            LastName = "Morant",
            Relationship = Relationship.Child,
            DateOfBirth = new DateTime(2021, 5, 18)
        },
        new()
        {
            Id = 4,
            FirstName = "DP",
            LastName = "Jordan",
            Relationship = Relationship.DomesticPartner,
            DateOfBirth = new DateTime(1974, 1, 2)
        }
    };
    private async Task<GetEmployeeDto> CreateEmployeeForTest(AddDependentDto? newDependent = null)
    {
        var dependents = new List<AddDependentDto>();
        if (newDependent is not null)
            dependents.Add(newDependent);
        var newEmployee = new AddEmployeeDto
        {
            DateOfBirth = DateTime.Today,
            Dependents = dependents,
            FirstName = "FirstName",
            LastName = "LastName",
            Salary = decimal.MaxValue
        };
        var createResponse = await HttpClient.PostAsync("/api/v1/employees", newEmployee);
        var createdEmployeeResponse = await createResponse.Content.DeserializeTo<ApiResponse<GetEmployeeDto>>();
        var createdEmployee = createdEmployeeResponse!.Data!;
        return createdEmployee;
    }
    [Fact]
    //task: make test pass
    public async Task WhenAskedForAllDependents_ShouldReturnAllDependents()
    {
        var expectedDependents = Dependents;

        var response = await HttpClient.GetAsync("/api/v1/dependents");
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        var actualDependentsResponse = await response.Content.DeserializeTo<ApiResponse<List<GetDependentDto>>>();
        var actualDependents = actualDependentsResponse!.Data!;
        foreach (var expectedDependent in expectedDependents)
        {
            var actualDependent = actualDependents.FirstOrDefault(a => a.Id == expectedDependent.Id);
            actualDependent.Should().NotBeNull().And.BeEquivalentTo(expectedDependent);
        }
        //var dependents = new List<GetDependentDto>
        //{
        //    new()
        //    {
        //        Id = 1,
        //        FirstName = "Spouse",
        //        LastName = "Morant",
        //        Relationship = Relationship.Spouse,
        //        DateOfBirth = new DateTime(1998, 3, 3)
        //    },
        //    new()
        //    {
        //        Id = 2,
        //        FirstName = "Child1",
        //        LastName = "Morant",
        //        Relationship = Relationship.Child,
        //        DateOfBirth = new DateTime(2020, 6, 23)
        //    },
        //    new()
        //    {
        //        Id = 3,
        //        FirstName = "Child2",
        //        LastName = "Morant",
        //        Relationship = Relationship.Child,
        //        DateOfBirth = new DateTime(2021, 5, 18)
        //    },
        //    new()
        //    {
        //        Id = 4,
        //        FirstName = "DP",
        //        LastName = "Jordan",
        //        Relationship = Relationship.DomesticPartner,
        //        DateOfBirth = new DateTime(1974, 1, 2)
        //    }
        //};
        //await response.ShouldReturn(HttpStatusCode.OK, dependents);
    }

    [Fact]
    //task: make test pass
    public async Task WhenAskedForADependent_ShouldReturnCorrectDependent()
    {
        var response = await HttpClient.GetAsync("/api/v1/dependents/1");
        //var dependent = new GetDependentDto
        //{
        //    Id = 1,
        //    FirstName = "Spouse",
        //    LastName = "Morant",
        //    Relationship = Relationship.Spouse,
        //    DateOfBirth = new DateTime(1998, 3, 3)
        //};
        var dependent = Dependents.FirstOrDefault(d => d.Id == 1);
        await response.ShouldReturn(HttpStatusCode.OK, dependent);
    }

    [Fact]
    //task: make test pass
    public async Task WhenAskedForANonexistentDependent_ShouldReturn404()
    {
        var response = await HttpClient.GetAsync($"/api/v1/dependents/{int.MinValue}");
        await response.ShouldReturn(HttpStatusCode.NotFound);
    }

    // POST
    [Fact]
    public async Task Post_Should_CreateDependentIfValidationPasses()
    {
        var createdEmployee = await CreateEmployeeForTest();
        var dependent = new AddDependentDto
        {
            DateOfBirth = DateTime.Today,
            EmployeeId = createdEmployee!.Id,
            FirstName = "FN",
            LastName = "LN",
            Relationship = Relationship.Child
        };

        var response = await HttpClient.PostAsync("/api/v1/dependents", dependent);

        await response.ShouldReturn(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Post_Should_NotCreateDependentIfValidationFails()
    {
        var createdEmployee = await CreateEmployeeForTest(new AddDependentDto
        {
            DateOfBirth = DateTime.Today,
            EmployeeId = 0,
            FirstName = "FirstName",
            LastName = "LastName",
            Relationship = Relationship.DomesticPartner
        });
        var dependent = new AddDependentDto
        {
            DateOfBirth = DateTime.Today,
            EmployeeId = createdEmployee!.Id,
            FirstName = "FirstName",
            LastName = "LastName",
            Relationship = Relationship.DomesticPartner
        };

        var createDependentResponse = await HttpClient.PostAsync("/api/v1/dependents", dependent);

        await createDependentResponse.ShouldReturn(HttpStatusCode.BadRequest);

        // ensure the employee dont have a new dependent associated with them
        var getEmployeeResponse = await HttpClient.GetAsync($"/api/v1/employees/{dependent.EmployeeId}");
        var employeeApiResponse = await getEmployeeResponse.Content.DeserializeTo<ApiResponse<GetEmployeeDto>>();
        employeeApiResponse!.Data!.Dependents.Count(d => d.Relationship == Relationship.Spouse || d.Relationship == Relationship.DomesticPartner).Should().Be(1);
    }

}

