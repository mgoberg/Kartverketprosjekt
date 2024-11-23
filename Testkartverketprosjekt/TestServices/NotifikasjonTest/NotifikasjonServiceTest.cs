using System;
using System.Threading.Tasks;
using kartverketprosjekt.Repositories.Notifikasjon;
using kartverketprosjekt.Services.Notifikasjon;
using Moq;
using Xunit;

public class NotifikasjonServiceTests
{
    private readonly Mock<INotifikasjonRepository> _mockRepository;
    private readonly NotifikasjonService _service;

    public NotifikasjonServiceTests()
    {
        _mockRepository = new Mock<INotifikasjonRepository>();
        _service = new NotifikasjonService(_mockRepository.Object);
    }

    [Fact]
    public async Task HarEndretStatus_ReturnsTrue_WhenStatusHasChanged()
    {
        // Arrange
        string testEmail = "test@example.com";
        _mockRepository.Setup(repo => repo.HasStatusChangedAsync(testEmail)).ReturnsAsync(true);

        // Act
        bool result = await _service.HarEndretStatus(testEmail);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(repo => repo.HasStatusChangedAsync(testEmail), Times.Once);
    }

    [Fact]
    public async Task HarEndretStatus_ReturnsFalse_WhenStatusHasNotChanged()
    {
        // Arrange
        string testEmail = "test@example.com";
        _mockRepository.Setup(repo => repo.HasStatusChangedAsync(testEmail)).ReturnsAsync(false);

        // Act
        bool result = await _service.HarEndretStatus(testEmail);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(repo => repo.HasStatusChangedAsync(testEmail), Times.Once);
    }

    [Fact]
    public async Task HarEndretStatus_ReturnsFalse_WhenExceptionIsThrown()
    {
        // Arrange
        string testEmail = "test@example.com";
        _mockRepository.Setup(repo => repo.HasStatusChangedAsync(testEmail)).ThrowsAsync(new Exception("Database error"));

        // Act
        bool result = await _service.HarEndretStatus(testEmail);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(repo => repo.HasStatusChangedAsync(testEmail), Times.Once);
    }

    [Fact]
    public async Task ResetNotificationStatus_ReturnsTrue_WhenResetSuccessful()
    {
        // Arrange
        string testEmail = "test@example.com";
        _mockRepository.Setup(repo => repo.ResetNotificationStatusAsync(testEmail)).ReturnsAsync(true);

        // Act
        bool result = await _service.ResetNotificationStatus(testEmail);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(repo => repo.ResetNotificationStatusAsync(testEmail), Times.Once);
    }

    [Fact]
    public async Task ResetNotificationStatus_ReturnsFalse_WhenResetFails()
    {
        // Arrange
        string testEmail = "test@example.com";
        _mockRepository.Setup(repo => repo.ResetNotificationStatusAsync(testEmail)).ReturnsAsync(false);

        // Act
        bool result = await _service.ResetNotificationStatus(testEmail);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(repo => repo.ResetNotificationStatusAsync(testEmail), Times.Once);
    }

    [Fact]
    public async Task ResetNotificationStatus_ReturnsFalse_WhenExceptionIsThrown()
    {
        // Arrange
        string testEmail = "test@example.com";
        _mockRepository.Setup(repo => repo.ResetNotificationStatusAsync(testEmail)).ThrowsAsync(new Exception("Database error"));

        // Act
        bool result = await _service.ResetNotificationStatus(testEmail);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(repo => repo.ResetNotificationStatusAsync(testEmail), Times.Once);
    }
}
