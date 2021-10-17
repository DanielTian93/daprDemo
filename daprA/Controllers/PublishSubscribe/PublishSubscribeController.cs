using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace daprA.Controllers.PublishSubscribe
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
        const string Programmatic_TOPIC_NAME = "ProgrammaticTUANZI";
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
            await stream.ReadAsync(buffer);
            string content = Encoding.UTF8.GetString(buffer);
            _logger.LogInformation("Asub" + content);
            return Ok(content);
        }

        [HttpGet("DeclarativePub")]
        public async Task<ActionResult> DeclarativePubAsync()
        {
            await _daprClient.PublishEventAsync(RBMQ_PUB_SUN, Declarative_TOPIC_NAME, $"RBMQ_Declarative:{Guid.NewGuid()}");
            await _daprClient.PublishEventAsync(Redis_PUB_SUN, Declarative_TOPIC_NAME, $"Redis_Declarative:{Guid.NewGuid()}");
            return Ok();
        }

        /// <summary>
        /// 编程式发布
        /// </summary>
        /// <returns></returns>
        [HttpGet("ProgrammaticPub")]
        public async Task<ActionResult> ProgrammaticPubAsync()
        {
            await _daprClient.PublishEventAsync(RBMQ_PUB_SUN,Programmatic_TOPIC_NAME, $"RBMQ_Programmatic:{Guid.NewGuid()}");
            await _daprClient.PublishEventAsync(Redis_PUB_SUN, Programmatic_TOPIC_NAME, $"Redis_Programmatic:{Guid.NewGuid()}");
            return Ok();
        }

        /// <summary>
        /// 编程式订阅
        /// </summary>
        /// <returns></returns>
        [Topic(RBMQ_PUB_SUN, Programmatic_TOPIC_NAME)]
        [HttpPost("ProgrammaticRBMQSub")]
        public async Task<ActionResult> RBMQProgrammaticSubAsync()
        {
            Stream stream = Request.Body;
            byte[] buffer = new byte[Request.ContentLength.Value];
            stream.Position = 0L;
            await stream.ReadAsync(buffer);
            string content = Encoding.UTF8.GetString(buffer);
            _logger.LogInformation("RBMQProgrammaticSub:" + content);
            return Ok(content);
        }

        /// <summary>
        /// 编程式订阅
        /// </summary>
        /// <returns></returns>
        [Topic(Redis_PUB_SUN, Programmatic_TOPIC_NAME)]
        [HttpPost("ProgrammaticRedisSub")]
        public async Task<ActionResult> RedisProgrammaticSubAsync()
        {
            Stream stream = Request.Body;
            byte[] buffer = new byte[Request.ContentLength.Value];
            stream.Position = 0L;
            await stream.ReadAsync(buffer);
            string content = Encoding.UTF8.GetString(buffer);
            _logger.LogInformation("RedisProgrammaticSub:" + content);
            return Ok(content);
        }

    }
}
