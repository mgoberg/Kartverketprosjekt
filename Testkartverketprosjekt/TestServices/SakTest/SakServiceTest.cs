using kartverketprosjekt.API_Models;
using kartverketprosjekt.Models;
using kartverketprosjekt.Repositories.Sak;
using kartverketprosjekt.Services.API;
using kartverketprosjekt.Services.File;
using kartverketprosjekt.Services.Sak;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Xunit;

namespace kartverketprosjekt.Tests.Services
{
    public class SakServiceTests
    {
        private readonly Mock<ISakRepository> _mockSakRepository;
        private readonly Mock<IKommuneInfoService> _mockKommuneInfoService;
        private readonly Mock<IFileService> _mockFileService;
        private readonly Mock<IDiscordBot> _mockDiscordBot;
        private readonly SakService _sakService;

        public SakServiceTests()
        {
            _mockSakRepository = new Mock<ISakRepository>();
            _mockKommuneInfoService = new Mock<IKommuneInfoService>();
            _mockFileService = new Mock<IFileService>();
            _mockDiscordBot = new Mock<IDiscordBot>();

            _sakService = new SakService(
                _mockSakRepository.Object,
                _mockKommuneInfoService.Object,
                _mockFileService.Object,
                _mockDiscordBot.Object
            );
        }

        [Fact]
        public async Task GetUserCasesAsync_ReturnsListOfUserCases()
        {
            // Arrange
            var userEmail = "test@example.com";
            var cases = new List<SakModel>
            {
                new SakModel { id = 1, beskrivelse = "Test case 1", epost_bruker = userEmail },
                new SakModel { id = 2, beskrivelse = "Test case 2", epost_bruker = userEmail }
            };

            _mockSakRepository.Setup(repo => repo.GetUserCasesAsync(userEmail))
                .ReturnsAsync(cases);

            // Act
            var result = await _sakService.GetUserCasesAsync(userEmail);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Test case 1", result[0].beskrivelse);
        }

        [Fact]
        public async Task RegisterCaseAsync_RegistersCaseSuccessfully()
        {
            // Arrange
            var sak = new SakModel { beskrivelse = "New case" };
            var vedleggMock = new Mock<IFormFile>();
            var kommuneInfo = new KommuneInfo
            {
                Kommunenavn = "Oslo",
                Kommunenummer = "0301",
                Fylkesnavn = "Oslo",
                Fylkesnummer = "03"
            };

            _mockKommuneInfoService.Setup(service => service.GetKommuneInfoAsync(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<int>()))
                .ReturnsAsync(kommuneInfo);
            _mockSakRepository.Setup(repo => repo.RegisterCaseAsync(It.IsAny<SakModel>()))
                .ReturnsAsync(1);
            _mockDiscordBot.Setup(bot => bot.SendMessageToDiscord(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var caseId = await _sakService.RegisterCaseAsync(sak, vedleggMock.Object, 0.0, 0.0, 0, "test@example.com");

            // Assert
            Assert.Equal(1, caseId);
            Assert.Equal("Oslo", sak.Kommunenavn);
            _mockSakRepository.Verify(repo => repo.RegisterCaseAsync(It.Is<SakModel>(s => s.beskrivelse == "New case")), Times.Once);
            _mockDiscordBot.Verify(bot => bot.SendMessageToDiscord(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCaseAsync_DeletesCaseSuccessfully()
        {
            // Arrange
            var caseId = 1;

            _mockSakRepository.Setup(repo => repo.DeleteCaseAsync(caseId))
                .ReturnsAsync(true);

            // Act
            var result = await _sakService.DeleteCaseAsync(caseId);

            // Assert
            Assert.True(result);
            _mockSakRepository.Verify(repo => repo.DeleteCaseAsync(caseId), Times.Once);
        }

        [Fact]
        public async Task GetCaseworkersAsync_ReturnsCaseworkers()
        {
            // Arrange
            var caseworkers = new List<SelectListItem>
            {
                new SelectListItem { Text = "User1", Value = "user1@example.com" },
                new SelectListItem { Text = "User2", Value = "user2@example.com" }
            };

            _mockSakRepository.Setup(repo => repo.GetCaseworkersAsync())
                .ReturnsAsync(caseworkers);

            // Act
            var result = await _sakService.GetCaseworkersAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("User1", result.First().Text);
        }
    }
}
