namespace kartverketprosjekt.Services.File
{
    using System.IO;

    public class FileService : IFileService
    {
        private readonly string _uploadsPath;

        public FileService(IWebHostEnvironment env)
        {
            // Fil sti for opplastinger (ikke med i versjonskontroll)
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

            // Genererer unikt filnavn 
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadsPath, uniqueFileName);

            // Lagrer filen
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName; // Returnerer filnavn for lagring i database
        }
    }

}
