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
        /// 通过HttpClient调用
        /// </summary>
        /// <returns></returns>
        [HttpGet("get1")]
        public async Task<ActionResult> GetAsync()
        {
            using var httpClient = DaprClient.CreateInvokeHttpClient();

            var result = await httpClient.GetAsync("http://daprb/WeatherForecast");
            var resultContent = string.Format("result is {0} {1}", result.StatusCode, await result.Content.ReadAsStringAsync());
            return Ok(resultContent);
        }
        /// <summary>
        /// 通过DaprClientBuild创建DaprClent调用
        /// </summary>
        /// <returns></returns>
        [HttpGet("get2")]
        public async Task<ActionResult> Get2Async()
        {
            using var daprClient = new DaprClientBuilder().Build();
            var result = await daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(HttpMethod.Get, "daprb", "WeatherForecast");
            return Ok(result);
        }
        /// <summary>
        /// 通过构造函数注入获取DaprClient调用
        /// </summary>
        /// <returns></returns>
        [HttpGet("get3")]
        public async Task<ActionResult> GetDIAsync()
        {
            var result = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(HttpMethod.Get, "daprb", "WeatherForecast");
            return Ok(result);
        }

        //使用Sidecar地址调用 http://localhost:3501/v1.0/invoke/daprb/method/WeatherForecast

        /*
         dapr run --dapr-http-port 3501 --app-port 5000 --app-id dapra --app-protocol grpc dotnet  .\daprA\bin\Debug\net5.0\daprA.dll --app-ssl
         使用grpc需要开启grpc协议端口 --app-protocol grpc

         */
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
    }
}
