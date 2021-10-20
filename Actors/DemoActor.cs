using Dapr.Actors.Runtime;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Actors
{
    public class DemoActor : Actor, IDemoActor, IRemindable
    {
        private readonly DaprClient _daprClient;
        const string Redis_STATE_STORE = "redisstatestore";
        private readonly ILogger<DemoActor> _logger;

        public DemoActor(ActorHost host, DaprClient daprClient, ILogger<DemoActor> logger) : base(host)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        #region 单线程
        /// <summary>
        /// 初始化库存
        /// </summary>
        /// <returns></returns>
        public async Task Init()
        {
            RedisHelper.HSet("Skill", "InventoryNum", 15);
            RedisHelper.HSet("Skill", "OrderNum", 0);
        }

        /// <summary>
        /// 获取当前库存
        /// </summary>
        /// <returns></returns>
        public async Task<long> GetInventory()
        {
            var inventoryNum = await RedisHelper.HGetAsync<long>("Skill", "InventoryNum");
            return inventoryNum;
        }

        /// <summary>
        /// 秒杀订单
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SkillOrder()
        {
            //获取库存
            var inventoryNum = await GetInventory();
            if (inventoryNum > 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                RedisHelper.HIncrBy("Skill", "InventoryNum", -1);
                RedisHelper.HIncrBy("Skill", "OrderNum", 1);
                _logger.LogInformation("下单成功");
                return true;
            }
            return false;
        }
        #endregion
        #region 定时作业
        /*
         Actor 生命周期
         Dapr Actors 是虚拟的，意思是他们的生命周期与他们的 in - memory 表现不相关。 因此，它们不需要显式创建或销毁。 Dapr Actors 运行时在第一次接收到该 actor ID 的请求时自动激活 actor。 如果 actor 在一段时间内未被使用，那么 Dapr Actors 运行时将回收内存对象。 如果以后需要重新启动，它还将保持对 actor 的一切原有数据。
        
        调用 actor 方法和 reminders 将重置空闲时间，例如，reminders 触发将使 actor 保持活动状态。 
        
        不论 actor 是否处于活动状态或不活动状态 Actor reminders 都会触发，对不活动 actor ，那么会首先激活 actor。 
        
        Actor timers 不会重置空闲时间，因此 timer 触发不会使参与者保持活动状态。 Timer 仅在 actor 活跃时被触发。
         */

        #region Timer
        /// <summary>
        /// 注册一个timer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task RegisterTimer(string id)
        {
            var serializedTimerParams = JsonSerializer.SerializeToUtf8Bytes(id);
            //注册一个Timer  dueTime -1 不执行 0 立即执行 3 3秒后执行  period -1 执行一次 3每隔3秒执行一次
            //return this.RegisterTimerAsync("TestTimer", nameof(this.TimerCallback), serializedTimerParams, TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(-1));
            return this.RegisterTimerAsync($"TestTimer{id}", nameof(this.TimerCallback), serializedTimerParams, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
        }

        /// <summary>
        /// timer 定时执行的操作
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task TimerCallback(byte[] data)
        {
            var stateKey = "nowtime";
            var content = JsonSerializer.Deserialize<string>(data);
            _logger.LogInformation(" ---------" + content + DateTime.Now.ToString());
            await this.StateManager.SetStateAsync<string>(stateKey, content);
        }
        /// <summary>
        /// 注销 Timer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task UnregisterTimer(string id)
        {
            return this.UnregisterTimerAsync($"TestTimer{id}");
        }
        #endregion

        #region Reminder
        /// <summary>
        /// 注册 reminder
        /// </summary>
        /// <returns></returns>
        public async Task RegisterReminder(string id)
        {
            var serializedTimerParams = JsonSerializer.SerializeToUtf8Bytes(id);

            await this.RegisterReminderAsync($"TestReminder{id}", serializedTimerParams, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// reminder 执行操作 该方法为继承IRemindable 必须实现的方法
        /// </summary>
        /// <param name="reminderName"></param>
        /// <param name="state"></param>
        /// <param name="dueTime"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            var content = "now is " + DateTime.Now.ToString();
            _logger.LogInformation($"{reminderName} reminder---------" + content);
        }

        public Task UnregisterReminder(string id)
        {
            return this.UnregisterReminderAsync("TestReminder");
        }


        #endregion

        #endregion


    }
}
