using Xunit;
using Moq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using kartverketprosjekt.Services.API;
using kartverketprosjekt.API_Models;
using RichardSzalay.MockHttp;

public class KommuneInfoServiceTests
{
    private readonly Mock<ILogger<KommuneInfoService>> _loggerMock;
    private readonly Mock<IOptions<ApiSettings>> _optionsMock;
    private readonly MockHttpMessageHandler _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly KommuneInfoService _kommuneInfoService;

    public KommuneInfoServiceTests()
    {
        _loggerMock = new Mock<ILogger<KommuneInfoService>>();
        _optionsMock = new Mock<IOptions<ApiSettings>>();

        _httpMessageHandlerMock = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_httpMessageHandlerMock)
        {
            BaseAddress = new Uri("http://example.com/")
        };

        _optionsMock.Setup(opt => opt.Value).Returns(new ApiSettings
        {
            KommuneInfoApiBaseUrl = "http://example.com/"
        });

        _kommuneInfoService = new KommuneInfoService(_httpClient, _loggerMock.Object, _optionsMock.Object);
    }

    [Fact]
    public async Task GetKommuneInfoAsync_ReturnsKommuneInfo_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var expectedKommuneInfo = new KommuneInfo
        {
            Kommunenavn = "Oslo",
            Fylkesnavn = "Oslo",
            Kommunenummer = "0301",
            Fylkesnummer = "03"
        };

        var jsonResponse = JsonSerializer.Serialize(expectedKommuneInfo);

        _httpMessageHandlerMock.When("http://example.com/punkt*")
                               .Respond("application/json", jsonResponse);

        // Act
        var result = await _kommuneInfoService.GetKommuneInfoAsync(1000, 2000, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedKommuneInfo.Kommunenavn, result.Kommunenavn);
        Assert.Equal(expectedKommuneInfo.Fylkesnavn, result.Fylkesnavn);
        Assert.Equal(expectedKommuneInfo.Kommunenummer, result.Kommunenummer);
        Assert.Equal(expectedKommuneInfo.Fylkesnummer, result.Fylkesnummer);
    }

    [Fact]
    public async Task GetKommuneInfoAsync_ReturnsNull_WhenApiResponseFails()
    {
        // Arrange
        _httpMessageHandlerMock.When("http://example.com/punkt*")
                               .Respond(HttpStatusCode.InternalServerError);

        // Act
        var result = await _kommuneInfoService.GetKommuneInfoAsync(1000, 2000, 1);

        // Assert
        Assert.Null(result);

        _loggerMock.Verify(
            log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once);
    }
}
