using Dapr.Actors;
using System.Threading.Tasks;

namespace Actors
{
    public interface IDemoActor : IActor
    {
        /// <summary>
        /// 初始化库存
        /// </summary>
        /// <returns></returns>
        Task Init();


        /// <summary>
        /// 获取当前库存
        /// </summary>
        /// <returns></returns>
        Task<long> GetInventory();


        /// <summary>
        /// 秒杀订单
        /// </summary>
        /// <returns></returns>
        Task<bool> SkillOrder();


        Task RegisterTimer(string id);
        Task UnregisterTimer(string id);

        Task RegisterReminder(string id);
        Task UnregisterReminder(string id);



    }
}
