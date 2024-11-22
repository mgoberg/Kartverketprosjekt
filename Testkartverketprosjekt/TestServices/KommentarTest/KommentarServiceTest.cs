using kartverketprosjekt.Repositories.Kommentar;
using kartverketprosjekt.Models;
using kartverketprosjekt.Services.Kommentar;
using Moq;

namespace Testkartverketprosjekt.TestServices.KommentarTest
{
    
    public class KommentarServiceTest
    {
        private readonly Mock<IKommentarRepository> _kommentarRepositoryMock;
        private readonly KommentarService _kmtService;

        public KommentarServiceTest()
        {
            _kommentarRepositoryMock = new Mock<IKommentarRepository>();
            _kmtService = new KommentarService(_kommentarRepositoryMock.Object);
        }

        [Fact]
        public async Task AddComment_ThrowsArgumentException_WhenSakIDIsInvalid()
        {
            // Arrange
            new KommentarService(_kommentarRepositoryMock.Object);

            int sakID = 0; // invalid
            string kommentar = "Testkommentar";
            string brukerEpost = "Test@test.no";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>( () => _kmtService.AddComment(sakID, kommentar, brukerEpost));
        }

        [Fact]
        public async Task AddComment_ShouldCallRepository_WhenInputIsValid()
        {
            // Arrange
            int sakID = 1; 
            string kommentar = "Testkommentar";
            string brukerEpost = "Test@test.no";

            // Act
            await _kmtService.AddComment(sakID, kommentar, brukerEpost);


            // Assert
            _kommentarRepositoryMock.Verify(repo => repo.AddCommentAsync(sakID, kommentar, brukerEpost), Times.Once);
        }

        [Fact]
        public async Task AddComment_ShouldThrowException_WhenKommentarIsNull()
        {
            // Arrange
            new KommentarService(_kommentarRepositoryMock.Object);

            int sakID = 1;
            string kommentar = null; // test med null
            string brukerEpost = "Test@test.no";

            //  Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _kmtService.AddComment(sakID, kommentar, brukerEpost));
        }

        [Fact]
        public async Task AddComment_ShouldThrowException_WhenKommentarIsEmpty()
        {
            // Arrange
            int sakID = 1;
            string kommentar = ""; // test med tom string
            string brukerEpost = "Test@test.no";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _kmtService.AddComment(sakID, kommentar, brukerEpost));
        }

        [Fact]
        public async Task AddComment_ShouldThrowException_WhenKommentarIsWhitespace()
        {
            // Arrange
            int sakID = 1;
            string kommentar = "   "; // test med whitespace
            string brukerEpost = "Test@test.no";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _kmtService.AddComment(sakID, kommentar, brukerEpost));
        }

        [Fact]
        public async Task GetComments_ShouldReturnComments_WhenSakIdIsValid()
        {
            // Arrange
            int validSakId = 1;

            var expectedComments = new List<KommentarModel> 
            {
                new() { Id = 1, Tekst = "Testkommentar 1", Epost = "test1@test.no" },
                new() { Id = 2, Tekst = "Testkommentar 2", Epost = "test2@test.no" }
            };

            _kommentarRepositoryMock
                .Setup(repo => repo.GetCommentsAsync(validSakId))
                .ReturnsAsync(expectedComments); // returnerer den falske listen ovenfor

            // Act
            var actualComments = await _kmtService.GetComments(validSakId);

            // Assert
            Assert.NotNull(actualComments); // Assert at listen ikke er null
            Assert.Equal(expectedComments.Count, actualComments.Count); // assert at listene har samme lengde
            Assert.Equal(expectedComments, actualComments); // assert at listene har likt innhold
            _kommentarRepositoryMock.Verify(repo => repo.GetCommentsAsync(validSakId), Times.Once); // verifiser at repository ble kalt 
        }


        [Fact]
        public async Task GetComments_ShouldThrowArgumentException_WhenSakIdIsInvalid()
        {
            // Arrange
            int invalidSakId = -1; 

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _kmtService.GetComments(invalidSakId)); // verifiser at exception blir kastet ("invalid SakID.")

            // verifiser at repositoriet ikke ble kalt
            _kommentarRepositoryMock.Verify(repo => repo.GetCommentsAsync(It.IsAny<int>()), Times.Never);
        }



    }
}
