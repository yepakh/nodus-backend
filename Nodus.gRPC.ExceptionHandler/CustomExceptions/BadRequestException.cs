using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.gRPC.ExceptionHandler.CustomExceptions
{
    public class BadRequestException : RpcException
    {
        public BadRequestException(string message) : base(new Status(StatusCode.InvalidArgument, message))
        {

        }
        public BadRequestException(Status status) : base(status)
        {

        }
    }
}
