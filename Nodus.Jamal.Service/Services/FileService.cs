using Microsoft.Extensions.Options;
using Nodus.Jamal.Service.Options;

namespace Nodus.Jamal.Service.Services
{
    public class FileService
    {
        private readonly BucketOptions _bucketOptions;

        public FileService(IOptions<BucketOptions> bucketOptions)
        {
            _bucketOptions = bucketOptions.Value;
        }

        public string GenerateFilePath(string fileExt)
        {
            if (fileExt.First() != '.')
            {
                fileExt = '.' + fileExt;
            }

            var bucketUrl = _bucketOptions.GetBucketURL();
            string fileName = Guid.NewGuid().ToString() + fileExt;

            return $"{bucketUrl}{fileName}";
        }
    }
}
