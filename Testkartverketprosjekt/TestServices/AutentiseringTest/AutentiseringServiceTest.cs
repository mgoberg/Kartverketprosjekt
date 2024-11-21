using Xunit;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using kartverketprosjekt.Models;
using kartverketprosjekt.Repositories.Bruker;
using kartverketprosjekt.Services.Autentisering;
using Microsoft.AspNetCore.Identity;

public class AutentiseringServiceTests
{
    private readonly Mock<IBrukerRepository> _brukerRepositoryMock;
    private readonly PasswordHasher<BrukerModel> _passwordHasher;
    private readonly AutentiseringService _authService;

    public AutentiseringServiceTests()
    {
        _brukerRepositoryMock = new Mock<IBrukerRepository>();
        _passwordHasher = new PasswordHasher<BrukerModel>();
        _authService = new AutentiseringService(_brukerRepositoryMock.Object, _passwordHasher);
    }

    [Fact]
    public async Task LoginAsync_ReturnsSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new BrukerModel
        {
            epost = "test@example.com",
            passord = _passwordHasher.HashPassword(null, "password123"),
            tilgangsnivaa_id = 1
        };

        _brukerRepositoryMock
            .Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        string email = "test@example.com";
        string password = "password123";

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.ErrorMessage);
        Assert.NotNull(result.Principal);

        var claims = result.Principal?.Claims;
        Assert.Contains(claims, c => c.Type == ClaimTypes.Name && c.Value == email);
        Assert.Contains(claims, c => c.Type == ClaimTypes.Role && c.Value == user.tilgangsnivaa_id.ToString());
    }

    [Fact]
    public async Task LoginAsync_ReturnsFailure_WhenUserNotFound()
    {
        // Arrange
        _brukerRepositoryMock
            .Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((BrukerModel?)null);

        string email = "invalid@example.com";
        string password = "password123";

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Feil epost eller passord", result.ErrorMessage);
        Assert.Null(result.Principal);
    }

    [Fact]
    public async Task LoginAsync_ReturnsFailure_WhenPasswordIsInvalid()
    {
        // Arrange
        var user = new BrukerModel
        {
            epost = "test@example.com",
            passord = _passwordHasher.HashPassword(null, "correctpassword"),
            tilgangsnivaa_id = 1
        };

        _brukerRepositoryMock
            .Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        string email = "test@example.com";
        string password = "wrongpassword";

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Feil epost eller passord", result.ErrorMessage);
        Assert.Null(result.Principal);
    }
}
