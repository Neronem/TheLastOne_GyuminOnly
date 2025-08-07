using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Entity.Scripts.NPC.Data.ForRuntime;
using _1.Scripts.Entity.Scripts.NPC.Data.StatDataSO;
using _1.Scripts.Entity.Scripts.NPC.Shebot_Weapon;
using _1.Scripts.Entity.Scripts.NPC.StatControllers.Base;
using _1.Scripts.Manager.Core;
using _1.Scripts.Static;
using _1.Scripts.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.StatControllers.Shebots
{
    public class ShebotSwordStatController : BaseShebotStatController
    {
        private RuntimeShebotSwordStatData runtimeShebotSwordStatData;

        [Header("Weapons")]
        private Shebot_Shield shield;
        
        protected override void Awake()
        {
            base.Awake();
            var shebotSwordStatData = CoreManager.Instance.resourceManager.GetAsset<ShebotSwordStatData>("ShebotSwordStatData"); // 자신만의 데이터 가져오기
            runtimeShebotSwordStatData = new RuntimeShebotSwordStatData(shebotSwordStatData);
            runtimeStatData = runtimeShebotSwordStatData;

            shield = GetComponentInChildren<Shebot_Shield>(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            runtimeShebotSwordStatData.ResetStats();
            shield.DisableShield();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            animator.SetTrigger(ShebotAnimationData.Shebot_Idle);
        }
        
        protected override void PlayHitAnimation()
        {
            if (!IsStunned) behaviorTree.SetVariableValue(BehaviorNames.ShouldAlertNearBy, true);
        }
        
        protected override void HackingSuccess()
        {
            base.HackingSuccess();
            behaviorTree.SetVariableValue(BehaviorNames.ShieldUsedOnce, false);
        }
    }
}
