using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Service.Tests.Patterns
{
    public partial class PatterResourceTests
    {
        public class GivenValidTokenShould
        {
            [Fact]
            public async Task RespondWithCreatedCode()
            {
                using (var client = new TestServer(Program.BuildWebHost()).CreateClient())
                {
                    // Arrange
                    var token = await GetAuthToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var content = await GetFileContent(@"Resources/M198_Seaside beauty.xsd");

                    // Act
                    var response = await client.PostAsync("/api/patterns/", content);

                    // Assert
                    response.StatusCode.Should().Be(HttpStatusCode.Created);
                }
            }
        }
    }
}
