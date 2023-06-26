using System.Text.Json.Serialization;

namespace Nodus.API.Models.Wrappers
{
    public class PagedResponse<T> : Response<T>
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int TotalRecords { get; set; }

        [JsonConstructor]
        public PagedResponse()
        {
            
        }

        public PagedResponse(T data, int offset, int limit, int total)
        {
            Offset = offset;
            Limit = limit;
            Data = data;
            TotalRecords = total;
        }
    }
}
