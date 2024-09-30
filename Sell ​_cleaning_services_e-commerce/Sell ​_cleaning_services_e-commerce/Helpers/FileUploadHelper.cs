namespace Sell_​_cleaning_services_e_commerce.Helpers
{
    public class FileUploadHelper
    {
        private readonly IConfiguration _configuration;

        public FileUploadHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadFile(IFormFile file, string uploadPath)
        {
            if (file == null || file.Length == 0)
                return null;

            var maxFileSize = _configuration.GetValue<long>("FileUpload:MaxFileSize");
            var allowedExtensions = _configuration.GetSection("FileUpload:AllowedExtensions").Get<string[]>();

            if (file.Length > maxFileSize)
                throw new Exception("File is too large");

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
                throw new Exception("File type is not allowed");

            var fileName = Guid.NewGuid().ToString() + fileExtension;
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
