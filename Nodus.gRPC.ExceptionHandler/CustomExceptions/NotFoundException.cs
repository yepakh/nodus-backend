using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.gRPC.ExceptionHandler.CustomExceptions
{
    public class NotFoundException : RpcException
    {
        public NotFoundException(string message) : base(new Status(StatusCode.NotFound, message))
        {
            
        }
        public NotFoundException(Status status) : base(status)
        {

        }
    }
}
