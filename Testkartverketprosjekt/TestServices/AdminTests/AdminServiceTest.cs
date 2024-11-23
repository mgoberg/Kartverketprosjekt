using kartverketprosjekt.Models;
using kartverketprosjekt.Repositories.Bruker;
using kartverketprosjekt.Repositories.Sak;
using kartverketprosjekt.Services.Admin;
using Moq;

namespace Testkartverketprosjekt.TestServices.AdminTests
{
    public class AdminServiceTest
    {
        private readonly Mock<IBrukerRepository> _mockBrukerRepository;
        private readonly Mock<ISakRepository> _mockSakRepository;
        private readonly AdminService _adminService;

        public AdminServiceTest()
        {
            _mockBrukerRepository = new Mock<IBrukerRepository>();
            _mockSakRepository = new Mock<ISakRepository>();
            _adminService = new AdminService(_mockBrukerRepository.Object, _mockSakRepository.Object);
        }

        [Fact]
        public async Task GetAdminViewStatsAsync_ShouldReturnCorrectStats()
        {
            // Arrange
            _mockBrukerRepository.Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(new List<BrukerModel> { new BrukerModel(), new BrukerModel() });

            _mockBrukerRepository.Setup(repo => repo.GetUserCountAsync()).ReturnsAsync(2);

            _mockSakRepository.Setup(repo => repo.GetCaseCountAsync()).ReturnsAsync(5);
            _mockSakRepository.Setup(repo => repo.GetCaseCountByStatusAsync("Ubehandlet")).ReturnsAsync(1);
            _mockSakRepository.Setup(repo => repo.GetCaseCountByStatusAsync("Under Behandling")).ReturnsAsync(2);
            _mockSakRepository.Setup(repo => repo.GetCaseCountByStatusAsync("Avvist")).ReturnsAsync(1);
            _mockSakRepository.Setup(repo => repo.GetCaseCountByStatusAsync("Arkivert")).ReturnsAsync(0);
            _mockSakRepository.Setup(repo => repo.GetCaseCountByStatusAsync("Løst")).ReturnsAsync(1);

            // Act
            var result = await _adminService.GetAdminViewStatsAsync();

            // Assert
            Assert.Equal(5, result.CaseCount);
            Assert.Equal(2, result.UserCount);
            Assert.Equal(1, result.OpenCasesUnbehandlet);
            Assert.Equal(2, result.OpenCasesUnderBehandling);
            Assert.Equal(1, result.OpenCasesAvvist);
            Assert.Equal(0, result.OpenCasesArkivert);
            Assert.Equal(1, result.ClosedCases);
            Assert.Equal(2, result.Users.Count);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUser_WhenInputIsValid()
        {
            // Arrange
            _mockBrukerRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((BrukerModel)null);

            // Act
            var result = await _adminService.CreateUserAsync("test@test.com", "password123", 1, "OrgName", "Test User");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Brukeren test@test.com ble opprettet.", result.Message);

            _mockBrukerRepository.Verify(repo => repo.AddUserAsync(It.IsAny<BrukerModel>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldReturnError_WhenEmailExists()
        {
            // Arrange
           
            _mockBrukerRepository.Setup(repo => repo.GetUserByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new BrukerModel());

            var adminService = new AdminService(_mockBrukerRepository.Object, Mock.Of<ISakRepository>());

            // Act
            var result = await adminService.CreateUserAsync("existing@test.com", "password123", 1, "OrgName", "Test User");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("En bruker med denne e-posten eksisterer allerede.", result.Message);

            _mockBrukerRepository.Verify(repo => repo.AddUserAsync(It.IsAny<BrukerModel>()), Times.Never);
        }
        [Fact]
        public async Task UpdateUserAccessAsync_ShouldUpdateAccessLevel_WhenUserExists()
        {
            // Arrange
            _mockBrukerRepository.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new BrukerModel());

            var adminService = new AdminService(_mockBrukerRepository.Object, Mock.Of<ISakRepository>());

            // Act
            var result = await adminService.UpdateUserAccessAsync("userId123", 2);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Endret tilgangsnivå for userId123 til: 2", result.Message);

            _mockBrukerRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenConditionsAreMet()
        {
            // Arrange
            var user = new BrukerModel { epost = "test@test.com" };
            _mockBrukerRepository.Setup(repo => repo.GetUserByEmailAsync("test@test.com")).ReturnsAsync(user);
            _mockSakRepository.Setup(repo => repo.GetAllCasesAsync())
                .ReturnsAsync(new List<SakModel>());

            var adminService = new AdminService(_mockBrukerRepository.Object, _mockSakRepository.Object);

            // Act
            var result = await adminService.DeleteUserAsync("test@test.com", "admin@test.com");

            // Assert
            Assert.True(result.Success);
            Assert.Equal("test@test.com ble slettet.", result.Message);

            _mockBrukerRepository.Verify(repo => repo.DeleteUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldReturnError_WhenTryingToDeleteSelf()
        {
            // Arrange
            _mockBrukerRepository.Setup(repo => repo.GetUserByEmailAsync("self@test.com"))
                .ReturnsAsync(new BrukerModel { epost = "self@test.com" });

            var adminService = new AdminService(_mockBrukerRepository.Object, Mock.Of<ISakRepository>());

            // Act
            var result = await adminService.DeleteUserAsync("self@test.com", "self@test.com");

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Du kan ikke slette din egen konto.", result.Message);

            _mockBrukerRepository.Verify(repo => repo.DeleteUserAsync(It.IsAny<BrukerModel>()), Times.Never);
        }

    }
}