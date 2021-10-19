using Dapr.Actors.Runtime;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Actors
{
    public class DemoActor : Actor, IDemoActor
    {
        private readonly DaprClient _daprClient;
        const string Redis_STATE_STORE = "redisstatestore";
        private readonly ILogger<DemoActor> _logger;
      
        public DemoActor(ActorHost host, DaprClient daprClient, ILogger<DemoActor> logger) : base(host)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

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
    }
}
