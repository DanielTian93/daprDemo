using Actors;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace daprA.Controllers.Actors
{
    /// <summary>
    /// actor 模式 阐述了 Actors 为最低级别的“计算单元”。 换句话说，您将代码写入独立单元 ( 称为actor) ，该单元接收消息并一次处理消息，而不进行任何类型的并行或线程处理。
    /// https://docs.dapr.io/zh-hans/developing-applications/building-blocks/actors/actors-overview/
    /// </summary>
    public class ActorsController : ControllerBase
    {
        private readonly ILogger<ActorsController> _logger;
        private readonly DaprClient _daprClient;
        private readonly IActorProxyFactory _actorProxyFactory;
        public ActorsController(DaprClient daprClient, IActorProxyFactory actorProxyFactory)
        {
            _daprClient = daprClient;
            _actorProxyFactory = actorProxyFactory;
        }

        /// <summary>
        /// 初始化库存
        /// </summary>
        /// <returns></returns>
        [HttpPost("Init")]
        public async Task<ActionResult> Init()
        {
            var actorId = new ActorId("INITTUANZI");
            var proxy = ActorProxy.Create<IDemoActor>(actorId, "DemoActor");
            await proxy.Init();

            //var proxy = _actorProxyFactory.CreateActorProxy<IDemoActor>(new ActorId("INITTUANZI"), "DemoActor");
            //await proxy.Init();
            return Ok(true);
        }

        /// <summary>
        /// 获取库存
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetInventory")]
        public async Task<ActionResult> GetInventory()
        {
            var actorId = new ActorId("GetInventoryTUANZI");
            var proxy = ActorProxy.Create<IDemoActor>(actorId, "DemoActor");
            var inventory = await proxy.GetInventory();
            return Ok(inventory);
        }

        /// <summary>
        /// 秒杀订单
        /// </summary>
        /// <returns></returns>
        [HttpPost("SkillOrder")]
        public async Task<ActionResult> SkillOrder()
        {
            var actorId = new ActorId("SkillOrderTUANZI");
            var proxy = ActorProxy.Create<IDemoActor>(actorId, "DemoActor");
            var res = await proxy.SkillOrder();
            return Ok(res);
        }

        /// <summary>
        /// 秒杀订单 并发 破坏Acotr
        /// </summary>
        /// <returns></returns>
        [HttpPost("SkillOrderRandom")]
        public async Task<ActionResult> SkillOrderRandom()
        {
            var actorId = new ActorId($"SkillOrderTUANZI{Guid.NewGuid():N}");
            var proxy = ActorProxy.Create<IDemoActor>(actorId, "DemoActor");
            var res = await proxy.SkillOrder();
            return Ok(res);
        }


        /// <summary>
        /// 注册一个timer
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("regist/timer/{orderId}")]
        public async Task<ActionResult> TimerAsync(string orderId)
        {
            var actorId = new ActorId("timer-" + orderId);
            var proxy = ActorProxy.Create<IDemoActor>(actorId, "DemoActor");
            await proxy.RegisterTimer(orderId);
            return Ok("done");
        }


        /// <summary>
        /// 注销一个timer
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("unregist/timer/{orderId}")]
        public async Task<ActionResult> UnregistTimerAsync(string orderId)
        {
            var actorId = new ActorId("timer-" + orderId);
            var proxy = ActorProxy.Create<IDemoActor>(actorId, "DemoActor");
            await proxy.UnregisterTimer(orderId);
            return Ok("done");
        }

        [HttpGet("reminder/{orderId}")]
        public async Task<ActionResult> ReminderAsync(string orderId)
        {
            var actorId = new ActorId("actorprifix-" + orderId);
            var proxy = ActorProxy.Create<IDemoActor>(actorId, "DemoActor");
            await proxy.RegisterReminder(orderId);
            return Ok("done");
        }
        [HttpGet("unregist/reminder/{orderId}")]
        public async Task<ActionResult> UnregistReminderAsync(string orderId)
        {
            var actorId = new ActorId("actorprifix-" + orderId);
            var proxy = ActorProxy.Create<IDemoActor>(actorId, "DemoActor");
            await proxy.UnregisterReminder(orderId);
            return Ok("done");
        }


    }
}
