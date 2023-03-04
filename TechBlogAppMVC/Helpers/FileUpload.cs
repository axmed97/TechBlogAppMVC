namespace TechBlogAppMVC.Helpers
{
    public static class FileUpload
    {
        public static async Task<string> SaveFileAsync(this IFormFile file, string path)
        {
            var filePath = "/uploads/" + Guid.NewGuid().ToString() + file.FileName;
            using FileStream fileStream = new(path + filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return filePath;
        }
    }
}
