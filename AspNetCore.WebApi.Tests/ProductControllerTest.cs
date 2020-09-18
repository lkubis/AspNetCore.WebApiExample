using AspNetCore.WebApi.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCore.WebApi.Tests
{
    public class ProductControllerTest
    {
        private readonly HttpClient _client;

        public ProductControllerTest()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());
            _client = server.CreateClient();
        }

        [Fact]
        public async Task FindAll_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/v1.0/products");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task FindById_UnknownIdPassed_ReturnsNotFoundResult()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/v1.0/products/0");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task FindById_ExistingIdPassed_ReturnsRightItem()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/v1.0/products/1");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(1, (await GetAsync<ProductDTO>(response.Content)).Id);
        }

        [Fact]
        public async Task UpdatedName_ValidNamePassed_ReturnsRightItem()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("PUT"), "/api/v1.0/products/1");
            request.Content = new StringContent(JsonConvert.SerializeObject("NewName"), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("NewName", (await GetAsync<ProductDTO>(response.Content)).Name);
        }

        [Fact]
        public async Task UpdatedName_ExistingNamePassed_ReturnsConflictResult()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("PUT"), "/api/v1.0/products/1");
            request.Content = new StringContent(JsonConvert.SerializeObject("Name_10"), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task FindAllPaged_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/v2.0/products/1/20");

            // Act
            var response = await _client.SendAsync(request);
            var paginationMetadata = JsonConvert.DeserializeObject<PaginationMetadata>(response.Headers.GetValues("X-Pagination-Metadata").First());
            
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(100, paginationMetadata.TotalCount);
            Assert.Equal(5, paginationMetadata.TotalPages);
            Assert.Equal(1, paginationMetadata.PageNumber);
            Assert.Equal(20, paginationMetadata.PageSize);
            Assert.False(paginationMetadata.HasPreviousPage);
            Assert.True(paginationMetadata.HasNextPage);
        }

        private async Task<T> GetAsync<T>(HttpContent content)
        {
            var contentAsString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(contentAsString);
        }
    }
}
