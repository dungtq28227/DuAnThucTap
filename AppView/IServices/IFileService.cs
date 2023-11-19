namespace AppView.IServices
{
    public interface IFileService
    {
        Task<string> AddFile(IFormFile file,string wwwRootPath);
    }
}
