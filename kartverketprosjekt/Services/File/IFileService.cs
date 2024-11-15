namespace kartverketprosjekt.Services.File
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
