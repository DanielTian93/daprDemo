using Dapr.Client;
using GrpcHello;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace daprA.Controllers.ServiceInvocation
{
    public class ServiceInvoacationController : ControllerBase
    {
        private readonly ILogger<ServiceInvoacationController> _logger;
        private readonly DaprClient _daprClient;
        public ServiceInvoacationController(ILogger<ServiceInvoacationController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        /// <summary>
        /// GRPC调用测试
        /// </summary>
        /// <returns></returns>
        [HttpGet("grpc")]
        public async Task<ActionResult> GrpcAsync()
        {
            using var daprClient = new DaprClientBuilder().Build();
            var result = await daprClient.InvokeMethodGrpcAsync<HelloRequest, HelloReply>("dapra", "sayhi", new HelloRequest { Name = "tuanzi" });
            return Ok(result);
        }
        /// <summary>
        /// GRPC调用测试
        /// </summary>
        /// <returns></returns>
        [HttpGet("grpc2")]
        public async Task<ActionResult> Grpc2Async()
        {
            var result = await _daprClient.InvokeMethodGrpcAsync<HelloRequest, HelloReply>("dapra", "sayhi", new HelloRequest { Name = "tuanzi" });
            return Ok(result);
        }

    }
}
