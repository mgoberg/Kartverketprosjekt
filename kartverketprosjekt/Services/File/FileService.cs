namespace kartverketprosjekt.Services.File
{
    using System.IO;

    public class FileService : IFileService
    {
        private readonly string _uploadsPath;

        public FileService(IWebHostEnvironment env)
        {
            // Define the path for file uploads inside the "wwwroot/uploads" directory
            _uploadsPath = Path.Combine(env.WebRootPath, "uploads");
            EnsureUploadsDirectoryExists();
        }

        private void EnsureUploadsDirectoryExists()
        {
            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file.");
            }

            // Generate a unique file name to prevent conflicts
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadsPath, uniqueFileName);

            // Save the file to the uploads directory
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName; // Return the file name for storing in the database
        }
    }

}
