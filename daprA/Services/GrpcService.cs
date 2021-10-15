using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client.Autogen.Grpc.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcHello;
using System;
using System.Threading.Tasks;

namespace daprA.Services
{
    public class GrpcService: AppCallback.AppCallbackBase
    {
        public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
        {
            var response = new InvokeResponse();
            switch (request.Method)
            {
                case "sayhi":
                    var input = request.Data.Unpack<HelloRequest>();
                    Console.WriteLine(input.Name);
                    response.Data = Any.Pack(new HelloReply { Message = "ok" });
                    break;
            }
            return response;
        }

    }
}
