using System.Threading;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Entity.Scripts.NPC.Data.ForRuntime;
using _1.Scripts.Entity.Scripts.NPC.Data.StatDataSO;
using _1.Scripts.Entity.Scripts.NPC.StatControllers.Base;
using _1.Scripts.Manager.Core;
using _1.Scripts.Static;
using _1.Scripts.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace _1.Scripts.Entity.Scripts.NPC.StatControllers.Shebots
{
    public class ShebotSniperStatController : BaseShebotStatController
    {
        private RuntimeShebotSniperStatData runtimeShebotSniperStatData;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private VisualEffect muzzleVisualEffect;
        
        protected override void Awake()
        {
            base.Awake();
            var shebotSwordStatData = CoreManager.Instance.resourceManager.GetAsset<ShebotSniperStatData>("ShebotSniperStatData"); // 자신만의 데이터 가져오기
            runtimeShebotSniperStatData = new RuntimeShebotSniperStatData(shebotSwordStatData);
            runtimeStatData = runtimeShebotSniperStatData;
            
            lineRenderer ??= GetComponent<LineRenderer>();
            muzzleVisualEffect ??= GetComponentInChildren<VisualEffect>();
        }

        protected override void Reset()
        {
            base.Reset();
            lineRenderer = GetComponent<LineRenderer>();
            muzzleVisualEffect = GetComponentInChildren<VisualEffect>(true);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            runtimeShebotSniperStatData.ResetStats();
            lineRenderer.enabled = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            animator.SetTrigger(ShebotAnimationData.Shebot_Rifle_Aim);
            muzzleVisualEffect.Stop();
        }

        protected override void PlayHitAnimation()
        {
            if (!IsStunned) behaviorTree.SetVariableValue(BehaviorNames.ShouldAlertNearBy, true);
        }

        protected override void PlayDeathAnimation()
        {
            base.PlayDeathAnimation();
            lineRenderer.enabled = false;
        }
        
        protected override void HackingSuccess()
        {
            base.HackingSuccess();
            behaviorTree.SetVariableValue(BehaviorNames.ShootInterval, 0f);
        }
    }
}
