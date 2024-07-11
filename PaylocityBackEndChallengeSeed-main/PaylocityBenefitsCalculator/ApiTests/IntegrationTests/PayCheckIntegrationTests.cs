using System.Net;
using System.Threading.Tasks;
using ApiTests.Extensions;
using Xunit;

namespace ApiTests.IntegrationTests;

public sealed class PayCheckIntegrationTests : IntegrationTest
{
    // GET
    [Fact]
    public async Task Get_Should_ReturnPaycheck()
    {
        var employeeId = 1;

        var response = await HttpClient.GetAsync($"/api/v1/paychecks/employee/{employeeId}");

        await response.ShouldReturn(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_Should_ReturnErrorIfEmployeeDoesNotExist()
    {
        var employeeId = int.MaxValue;

        var response = await HttpClient.GetAsync($"/api/v1/paychecks/employee/{employeeId}");

        await response.ShouldReturn(HttpStatusCode.BadRequest);
    }
}