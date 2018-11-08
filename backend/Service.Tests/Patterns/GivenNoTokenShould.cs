using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Service.Tests.Patterns
{
    public partial class PatterResourceTests
    {
        public class GivenNoProvidedShould
        {
            [Fact]
            public async Task RespondWithUnauthorizedCode()
            {
                using (var client = new TestServer(Program.BuildWebHost()).CreateClient())
                {
                    // Arrange
                    var content = await GetFileContent(@"Resources/M198_Seaside beauty.xsd");

                    // Act
                    var response = await client.PostAsync("/api/patterns/", content);

                    // Assert
                    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
                }
            }
        }
    }
}
