using FluentAssertions;
using LinenManagement.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace LinenManagementTest
{
    public class IntegrationCartLogControllerTests : IClassFixture<CustomWebApplicationFactory<LinenManagement.Program>>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseUrl = "api/Cart/cartlogs"; 

        public IntegrationCartLogControllerTests(CustomWebApplicationFactory<LinenManagement.Program> factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        private StringContent GetJsonContent<T>(T model)
        {
            return new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json");
        }

        #region Get By Id

        [Fact]
        public async Task GetCartLogById_Returns_Ok()
        {
            // Arrange
            var expectedId = 1;

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/cartlogid?cartLogId={expectedId}");

            // Assert
            response.EnsureSuccessStatusCode(); 
            var content = await response.Content.ReadAsStringAsync();
            var cartLogDto = JsonSerializer.Deserialize<CartLogDTO>(content, _jsonOptions);

            cartLogDto.Should().NotBeNull();
            cartLogDto.CartLogId.Should().Be(expectedId);
        }

        [Fact]
        public async Task GetCartLogById_Returns_NotFound()
        {
            // Act
            var response = await _client.GetAsync($"{BaseUrl}/cartlogid?cartLogId=999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound); 
        }

        #endregion

        #region Get All
        [Fact]
        public async Task GetAll_Returns_Ok()
        {
            // Arrange
            var employeeId = 1;
            var locationId = 1;
            var cartType = "CLEAN";

            // Act
            var responseEmployee = await _client.GetAsync($"{BaseUrl}?employeeId={employeeId}");
            var responseLocation = await _client.GetAsync($"{BaseUrl}?locationId={locationId}");
            var responseType = await _client.GetAsync($"{BaseUrl}?cartType={cartType}");
            var responseBase = await _client.GetAsync($"{BaseUrl}");


            // Assert
            responseEmployee.EnsureSuccessStatusCode();
            var content = await responseEmployee.Content.ReadAsStringAsync();
            var cartLogDto = JsonSerializer.Deserialize<IEnumerable<CartLogDTO>>(content, _jsonOptions);

            cartLogDto.Should().NotBeNull();

            responseLocation.EnsureSuccessStatusCode();
            var content1 = await responseEmployee.Content.ReadAsStringAsync();
            var cartLogDto1 = JsonSerializer.Deserialize<IEnumerable<CartLogDTO>>(content, _jsonOptions);

            cartLogDto1.Should().NotBeNull();

            responseType.EnsureSuccessStatusCode();
            var content2 = await responseEmployee.Content.ReadAsStringAsync();
            var cartLogDto2 = JsonSerializer.Deserialize<IEnumerable<CartLogDTO>>(content, _jsonOptions);

            cartLogDto2.Should().NotBeNull();

            responseBase.EnsureSuccessStatusCode();
            var content3 = await responseEmployee.Content.ReadAsStringAsync();
            var cartLogDto3 = JsonSerializer.Deserialize<IEnumerable<CartLogDTO>>(content, _jsonOptions);

            cartLogDto3.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAll_Returns_NotFound()
        {
            // Act
            var responseEmployee = await _client.GetAsync($"{BaseUrl}?employeeId=999");
            var responseLocation = await _client.GetAsync($"{BaseUrl}?locationId=999");
            var responseType = await _client.GetAsync($"{BaseUrl}?cartType=NONE");

            // Assert
            responseEmployee.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseLocation.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseType.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        #endregion

        #region Create/Update 

        [Fact]
        public async Task CreateUpdateCartLog_CreatesNewLog_Returns_Ok()
        {
            // Arrange:
            var newLog = new CartLog
            {
                CartLogId = 0,
                ReceiptNumber = "INT-NEW",
                ActualWeight = 75,
                DateWeighed = DateTime.UtcNow,
                CartId = 1,
                LocationId = 1,
                EmployeeId = 1
            };
            var content = GetJsonContent(newLog);

            // Act:
            var url = BaseUrl + "/upsert";
            var response = await _client.PostAsync(url, content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdLog = JsonSerializer.Deserialize<CartLog>(responseContent, _jsonOptions);

            createdLog.Should().NotBeNull();
            createdLog.ActualWeight.Should().Be(75);
            createdLog.CartLogId.Should().BeGreaterThan(0); // Should have a new ID assigned by the DB
        }

        [Fact]
        public async Task CreateUpdateCartLog_UpdatesExistingLog_Returns_Ok()
        {
            // Arrange
            var newLog = new CartLog
            {
                CartLogId = 1,
                ReceiptNumber = "UPDATED",
                ReportedWeight = 10,
                ActualWeight = 10,
                DateWeighed = DateTime.Parse("2024-10-01T10:00:00Z"),
                CartId = 1,
                LocationId = 1,
                EmployeeId = 1
            };
            var content = GetJsonContent(newLog);

            // Act
            var url = BaseUrl + "/upsert";
            var response = await _client.PostAsync(url, content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedLog = JsonSerializer.Deserialize<CartLog>(responseContent, _jsonOptions);

            updatedLog.Should().NotBeNull();
            updatedLog.ReceiptNumber.Should().Be("UPDATED");
        }

        #endregion

        #region Delete Operation

        [Fact]
        public async Task DeleteCartLog_DeletesLog_Returns_Ok()
        {
            // Arrange
            var deleteId = 1;
            var employeeId = 1; 

            // Act
            var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/cartlogid?cartLogId={deleteId}&employeeId={employeeId}");

            // Assert
            deleteResponse.EnsureSuccessStatusCode();

            // Verify it's actually gone
            var url = BaseUrl + "/cartlogid?cartLogId=" + deleteId;
            var getResponse = await _client.GetAsync(url);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCartLog_DeletesLog_Returns_BadRequest()
        {
            // Arrange
            var deleteId = 101;
            var employeeId = 1; 

            // Act
            var deleteResponse = await _client.DeleteAsync($"{BaseUrl}/cartlogid?cartLogId={deleteId}&employeeId={employeeId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion
    }
}
