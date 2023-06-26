using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Nodus.API.Mapping
{
    public class TimestampToDateTimeConverter : ITypeConverter<Timestamp, DateTime>
    {
        public DateTime Convert(Timestamp source, DateTime destination, ResolutionContext context)
        {
            return source.ToDateTime();
        }
    }
}
