using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace daprA.Controllers.StateManagement.Controller
{
    public class StateManagementController : ControllerBase
    {

        private readonly ILogger<StateManagementController> _logger;
        private readonly DaprClient _daprClient;
        public StateManagementController(ILogger<StateManagementController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }
        /// <summary>
        /// 存储组件名称
        /// </summary>
        const string Redis_STATE_STORE = "redisstatestore";
        //const string Mysql_STATE_STORE = "mysqlstatestore";
        const string KEY_NAME = "TUANZI";

        /// <summary>
        /// 强一致性保存一个值
        /// </summary>
        /// <returns></returns>
        [HttpPost("RedisStateStoreStrong")]
        public async Task<ActionResult> RedisStateStoreStrongAsync()
        {
            Console.Write(Redis_STATE_STORE);
            await _daprClient.SaveStateAsync(Redis_STATE_STORE, KEY_NAME, Guid.NewGuid().ToString(), new StateOptions() { Consistency = ConsistencyMode.Strong });
            return Ok();
        }

        /// <summary>
        /// 通过tag防止并发冲突，保存一个值
        /// </summary>
        /// <returns></returns>
        [HttpPost("RedisStateStoreTag")]
        public async Task<ActionResult> RedisStateStoreTagAsync()
        {
            var (_, etag) = await _daprClient.GetStateAndETagAsync<string>(Redis_STATE_STORE, KEY_NAME);
            await _daprClient.TrySaveStateAsync(Redis_STATE_STORE, KEY_NAME, Guid.NewGuid().ToString(), etag);
            return Ok();
        }

        ///// <summary>
        ///// 强一致性保存一个值
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("MysqlStateStoreStrong")]
        //public async Task<ActionResult> MysqlStateStoreStrongAsync()
        //{
        //    await _daprClient.SaveStateAsync(Mysql_STATE_STORE, KEY_NAME, Guid.NewGuid().ToString(), new StateOptions() { Consistency = ConsistencyMode.Strong });
        //    return Ok();
        //}

        ///// <summary>
        ///// 通过tag防止并发冲突，保存一个值
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost("MysqlStateStoreTag")]
        //public async Task<ActionResult> MysqlStateStoreTagAsync()
        //{
        //    var (value, etag) = await _daprClient.GetStateAndETagAsync<string>(Mysql_STATE_STORE, KEY_NAME);
        //    await _daprClient.TrySaveStateAsync(Mysql_STATE_STORE, KEY_NAME, Guid.NewGuid().ToString(), etag);
        //    return Ok();
        //}
    }
}
