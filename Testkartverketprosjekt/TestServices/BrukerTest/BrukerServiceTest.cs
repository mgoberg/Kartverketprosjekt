using kartverketprosjekt.Models;
using kartverketprosjekt.Repositories.Bruker;
using kartverketprosjekt.Services.Bruker;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Testkartverketprosjekt.TestServices.BrukerTest
{
    public class BrukerServiceTest
    {
        private readonly Mock<IBrukerRepository> _mockBrukerRepository;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<IPasswordHasher<BrukerModel>> _mockPasswordHasher;
        private readonly Mock<IAuthenticationService> _mockAuthenticationService;
        private readonly BrukerService _brukerService;

        public BrukerServiceTest()
        {
            _mockBrukerRepository = new Mock<IBrukerRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockPasswordHasher = new Mock<IPasswordHasher<BrukerModel>>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _brukerService = new BrukerService(
                _mockBrukerRepository.Object,
                _mockHttpContextAccessor.Object,
                _mockPasswordHasher.Object
            );
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldRegisterUser_WhenUserDoesNotExist()
        {
            // Arrange
            var model = new RegistrerModel
            {
                epost = "test@example.com",
                passord = "password123",
                navn = "Test User"
            };

            _mockBrukerRepository.Setup(repo => repo.GetUserByEmailAsync(model.epost))
                .ReturnsAsync((BrukerModel)null); // Brukeren finnes ikke

            _mockBrukerRepository.Setup(repo => repo.AddUserAsync(It.IsAny<BrukerModel>()))
                .Returns(Task.CompletedTask);

            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(null, model.passord))
                .Returns("hashedpassword123");

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(_mockAuthenticationService.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = mockServiceProvider.Object // Sikre tilgang til tjenestene
            };

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            _mockAuthenticationService
                .Setup(auth => auth.SignInAsync(
                    It.IsAny<HttpContext>(),
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    It.IsAny<ClaimsPrincipal>(),
                    null))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _brukerService.RegisterUserAsync(model);

            // Assert
            Assert.True(result);
            _mockBrukerRepository.Verify(repo => repo.AddUserAsync(It.IsAny<BrukerModel>()), Times.Once);
            _mockAuthenticationService.Verify(auth => auth.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<ClaimsPrincipal>(),
                null), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFalse_WhenUserAlreadyExists()
        {
            // Arrange
            var model = new RegistrerModel
            {
                epost = "existinguser@example.com",
                passord = "password123",
                navn = "Existing User"
            };

            _mockBrukerRepository.Setup(repo => repo.GetUserByEmailAsync(model.epost))
                .ReturnsAsync(new BrukerModel()); // Brukeren finnes allerede

            // Act
            var result = await _brukerService.RegisterUserAsync(model);

            // Assert
            Assert.False(result);
            _mockBrukerRepository.Verify(repo => repo.AddUserAsync(It.IsAny<BrukerModel>()), Times.Never);
        }

        [Fact]
        public async Task UpdateNameAsync_ShouldUpdateName_WhenUserExists()
        {
            // Arrange
            var navn = "New Name";
            var epost = "test@example.com";

            var bruker = new BrukerModel { epost = epost, navn = "Old Name" };

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User.Identity.Name)
                .Returns(epost);

            _mockBrukerRepository.Setup(repo => repo.GetUserByEmailAsync(epost))
                .ReturnsAsync(bruker);

            _mockBrukerRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _brukerService.UpdateNameAsync(navn);

            // Assert
            Assert.True(result);
            Assert.Equal(navn, bruker.navn);
            _mockBrukerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePasswordAsync_ShouldUpdatePassword_WhenUserExists()
        {
            // Arrange
            var password = "newpassword123";
            var epost = "test@example.com";

            var bruker = new BrukerModel { epost = epost, passord = "oldhashedpassword" };

            _mockHttpContextAccessor.Setup(x => x.HttpContext.User.Identity.Name)
                .Returns(epost);

            _mockBrukerRepository.Setup(repo => repo.GetUserByEmailAsync(epost))
                .ReturnsAsync(bruker);

            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(bruker, password))
                .Returns("newhashedpassword");

            _mockBrukerRepository.Setup(repo => repo.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _brukerService.UpdatePasswordAsync(password);

            // Assert
            Assert.True(result);
            Assert.Equal("newhashedpassword", bruker.passord);
            _mockBrukerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
