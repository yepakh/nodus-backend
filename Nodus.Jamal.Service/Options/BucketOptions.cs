using Microsoft.AspNetCore.Components.Web;
using System.Text;

namespace Nodus.Jamal.Service.Options
{
    public class BucketOptions
    {
        public string BaseUrl { get; set; }
        public string Region { get; set; }
        public string BucketName { get; set; }

        public string GetBucketURL()
        {
            var builder = new StringBuilder();
            var url = builder.AppendFormat(BaseUrl, Region, BucketName).ToString();
            if(url.Last() != '/')
            {
                url += '/';
            }

            return url;
        }
    }
}
