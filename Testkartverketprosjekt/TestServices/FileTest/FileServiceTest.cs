using kartverketprosjekt.Services.File;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Testkartverketprosjekt.TestServices.FileTest;

public class FileServiceTests
{
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly FileService _fileService;
    private readonly string _uploadsPath;

    public FileServiceTests()
    {
        // Arrange
        _uploadsPath = Path.Combine(Path.GetTempPath(), "uploads");
        _envMock = new Mock<IWebHostEnvironment>();
        _envMock.Setup(env => env.WebRootPath).Returns(Path.GetTempPath()); // mock filsti
        _fileService = new FileService(_envMock.Object);
    }

    [Fact]
    public async Task UploadFileAsync_ShouldReturnUniqueFileName_WhenFileIsValid()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = "Test file content";
        var fileName = "testfile.txt";

        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(ms.Length);

        // Act
        var result = await _fileService.UploadFileAsync(fileMock.Object);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(result)); // Assert unikt filnavn
        Assert.True(result.EndsWith(Path.GetExtension(fileName))); // sjekk fil extension
        Assert.True(File.Exists(Path.Combine(_uploadsPath, result))); // sjekk at filen ble lagret på disk

        // Cleanup
        File.Delete(Path.Combine(_uploadsPath, result));
    }

    [Fact]
    public async Task UploadFileAsync_ShouldThrowArgumentException_WhenFileIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fileService.UploadFileAsync(null));
    }

    [Fact]
    public async Task UploadFileAsync_ShouldThrowArgumentException_WhenFileLengthIsZero()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fileService.UploadFileAsync(fileMock.Object));
    }

    [Fact]
    public void EnsureUploadsDirectoryExists_ShouldCreateUploadsDirectory_WhenItDoesNotExist()
    {
        // Arrange
        var directoryPath = Path.Combine(_uploadsPath);
        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, true);
        }

        // Act
        new FileService(_envMock.Object);

        // Assert
        Assert.True(Directory.Exists(directoryPath));
    }
}