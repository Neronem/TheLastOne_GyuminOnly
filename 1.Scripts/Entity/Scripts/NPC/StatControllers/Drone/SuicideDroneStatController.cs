using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Entity.Scripts.NPC.Data.ForRuntime;
using _1.Scripts.Entity.Scripts.NPC.Data.StatDataSO;
using _1.Scripts.Entity.Scripts.NPC.StatControllers.Base;
using _1.Scripts.Manager.Core;

namespace _1.Scripts.Entity.Scripts.NPC.StatControllers.Drone
{
    public class SuicideDroneStatController : BaseDroneStatController
    {
        private RuntimeSuicideDroneStatData runtimeSuicideDroneStatData;
        
        protected override void Awake()
        {
            base.Awake();
            var suicideDroneStatData = CoreManager.Instance.resourceManager.GetAsset<SuicideDroneStatData>("SuicideDroneStatData"); // 자신만의 데이터 가져오기
            runtimeSuicideDroneStatData = new RuntimeSuicideDroneStatData(suicideDroneStatData);
            runtimeStatData = runtimeSuicideDroneStatData;
        }
        // 이후 runtimeSuicideDroneStatData 수정 시 베이스에 반영
        
        protected override void OnDisable()
        {
            base.OnDisable();
            runtimeSuicideDroneStatData.ResetStats();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            animator.SetTrigger(DroneAnimationData.Repair);
        }

    }
}
