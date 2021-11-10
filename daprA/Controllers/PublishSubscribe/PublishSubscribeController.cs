using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace daprA.Controllers.PublishSubscribe
{
    /// <summary>
    /// dapr发布订阅 https://docs.dapr.io/zh-hans/developing-applications/building-blocks/pubsub/howto-publish-subscribe/#%E4%BB%8B%E7%BB%8D
    /// ACK-ing 消息
    /// 为了告诉Dapr 消息处理成功，返回一个 200 OK 响应。 如果 Dapr 收到超过 200 的返回状态代码，或者你的应用崩溃，Dapr 将根据 At-Least-Once 语义尝试重新传递消息
    /// </summary>
    /*
        要使用这个主题范围，可以设置一个 pub/sub 组件的三个元数据属性：
        spec.metadata.publishingScopes
        分号分隔应用程序列表& 逗号分隔的主题列表允许该 app 发布信息到主题列表
        如果在 publishingScopes (缺省行为) 中未指定任何内容，那么所有应用程序可以发布到所有主题
        要拒绝应用程序发布信息到任何主题，请将主题列表留空 (app1=;app2=topic2)
        例如， app1=topic1;app2=topic2,topic3;app3= 允许 app1 发布信息至 topic1 ，app2 允许发布信息到 topic2 和 topic3 ，app3 不允许发布信息到任何主题。
        spec.metadata.subscriptionScopes
        分号分隔应用程序列表& 逗号分隔的主题列表允许该 app 订阅主题列表
        如果在 subscriptionScopes (缺省行为) 中未指定任何内容，那么所有应用程序都可以订阅所有主题
        例如， app1=topic1;app2=topic2,topic3 允许 app1 订阅 topic1 ，app2 可以订阅 topic2 和 topic3
        spec.metadata.allowedTopics
        一个逗号分隔的允许主题列表，对所有应用程序。
        如果未设置 allowedTopics (缺省行为) ，那么所有主题都有效。 subscriptionScopes 和 publishingScopes 如果存在则仍然生效。
        publishingScopes 或 subscriptionScopes 可用于与 allowedTopics 的 conjuction ，以添加限制粒度
     */
    public class PublishSubscribeController : ControllerBase
    {
        private readonly ILogger<PublishSubscribeController> _logger;
        private readonly DaprClient _daprClient;

        public PublishSubscribeController(ILogger<PublishSubscribeController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }
        const string Redis_PUB_SUN = "pubsub";
        //const string RBMQ_PUB_SUN = "pubsubrabbit";
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
            //await _daprClient.PublishEventAsync(RBMQ_PUB_SUN, Declarative_TOPIC_NAME, $"RBMQ_Declarative:{Guid.NewGuid()}");
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
            //Dapr 允许对每个消息设置生存时间(TTL)。 这意味着应用程序可以设置每条消息的生存时间，并且这些消息过期后订阅者不会收到。
            var dictionary = new Dictionary<string, string>()
            {
                { "ttlInseconds","120"}
            };
            //await _daprClient.PublishEventAsync(RBMQ_PUB_SUN, Programmatic_TOPIC_NAME, $"RBMQ_Programmatic:{Guid.NewGuid()}", metadata: dictionary);
            await _daprClient.PublishEventAsync(Redis_PUB_SUN, Programmatic_TOPIC_NAME, $"Redis_Programmatic:{Guid.NewGuid()}");
            return Ok();
        }

        ///// <summary>
        ///// 编程式订阅
        ///// </summary>
        ///// <returns></returns>
        //[Topic(RBMQ_PUB_SUN, Programmatic_TOPIC_NAME)]
        //[HttpPost("ProgrammaticRBMQSub")]
        //public async Task<ActionResult> RBMQProgrammaticSubAsync()
        //{
        //    Stream stream = Request.Body;
        //    byte[] buffer = new byte[Request.ContentLength.Value];
        //    stream.Position = 0L;
        //    await stream.ReadAsync(buffer);
        //    string content = Encoding.UTF8.GetString(buffer);
        //    _logger.LogInformation("RBMQProgrammaticSub:" + content);
        //    return Ok(content);
        //}

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
