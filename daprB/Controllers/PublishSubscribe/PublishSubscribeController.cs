using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace daprB.Controllers.PublishSubscribe
{
    public class PublishSubscribeController : ControllerBase
    {
        private readonly ILogger<PublishSubscribeController> _logger;
        private readonly DaprClient _daprClient;

        public PublishSubscribeController(ILogger<PublishSubscribeController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }
        const string Redis_PUB_SUN = "pubsubredis";
        const string RBMQ_PUB_SUN = "pubsubrabbit";
        const string TOPIC_NAME = "TUANZI";
        const string Declarative_TOPIC_NAME = "DeclarativeTUANZI";
        /*
         申明式订阅
        需要在components中进行新增配置
        apiVersion: dapr.io/v1alpha1
        kind: Subscription
        metadata:
          name: dapra_subscription
        spec:
          topic: test_topic #此处是订阅的topic
          route: /api/PubSub # 此处是订阅的接口
          pubsubname: pubsubrabbit # pubsubrabbit 此处填写的是发布订阅组件的name
        scopes:
        - dapra
         */

        [HttpPost("DeclarativeSub")]
        public async Task<ActionResult> DeclarativeSubAsync()
        {
            Stream stream = Request.Body;
            byte[] buffer = new byte[Request.ContentLength.Value];
            stream.Position = 0L;
            await stream.ReadAsync(buffer, 0, buffer.Length);
            string content = Encoding.UTF8.GetString(buffer);
            _logger.LogInformation("Bsub" + content);
            return Ok(content);
        }

    }
}
