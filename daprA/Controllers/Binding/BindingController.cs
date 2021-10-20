using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace daprA.Controllers.Binding
{
    /*
     * 支持HTTP MQ MQTT  等详细看文档 有的仅支持输出、输入
     https://docs.dapr.io/zh-hans/reference/components-reference/supported-bindings/
     使用绑定，您可以使用来自外部系统的事件或与外部系统的接口来触发应用程序。 此构建块为您和您的代码提供了若干益处 :
        除去连接到消息传递系统 ( 如队列和消息总线 ) 并进行轮询的复杂性
        聚焦于业务逻辑，而不是如何与系统交互的实现细节
        使代码不受 SDK 或库的跟踪
        处理重试和故障恢复
        在运行时在绑定之间切换
        构建具有特定于环境的绑定的可移植应用程序，不需要进行代码更改
     */
    public class BindingController : ControllerBase
    {

        private readonly ILogger<BindingController> _logger;
        private readonly DaprClient _daprClient;
        public BindingController(DaprClient daprClient, ILogger<BindingController> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        /// <summary>
        /// 接收binding 此处使用rabbitmq 配置文件配置后 接收指定队列数据
        /// https://docs.dapr.io/zh-hans/reference/components-reference/supported-bindings/rabbitmq/
        /// </summary>
        /// <returns></returns>
        [HttpPost("InputBinding")]
        public ActionResult Post()
        {
            Stream stream = Request.Body;
            byte[] buffer = new byte[Request.ContentLength.Value];
            stream.Position = 0L;
            stream.ReadAsync(buffer, 0, buffer.Length);
            string content = Encoding.UTF8.GetString(buffer);
            _logger.LogInformation(".............binding............." + content);
            return Ok();
        }
        /// <summary>
        /// create get delete list 此处使用rabbitmq create 推送信息到队列
        /// </summary>
        /// <param name="daprClient"></param>
        /// <returns></returns>
        [HttpGet("OutputBinding")]
        public async Task<ActionResult> OutputAsync([FromServices] DaprClient daprClient)
        {
            await _daprClient.InvokeBindingAsync("InputBinding", "create", "tuanzi");
            return Ok();
        }

        /// <summary>
        /// 定时调用接口
        /// </summary>
        /// <returns></returns>
        [HttpPost("CronRabbitBinding")]
        public ActionResult Cron()
        {
            _logger.LogInformation(".............corn binding.............");
            return Ok();
        }

    }
}
