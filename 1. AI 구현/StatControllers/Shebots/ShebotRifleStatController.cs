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
    public class ShebotRifleStatController : BaseShebotStatController
    {
        private RuntimeShebotRifleStatData runtimeShebotRifleStatData;
        [SerializeField] private VisualEffect muzzleVisualEffect;
        
        protected override void Awake()
        {
            base.Awake();
            var shebotSwordStatData = CoreManager.Instance.resourceManager.GetAsset<ShebotRifleStatData>("ShebotRifleStatData"); // 자신만의 데이터 가져오기
            runtimeShebotRifleStatData = new RuntimeShebotRifleStatData(shebotSwordStatData);
            runtimeStatData = runtimeShebotRifleStatData;
            
            muzzleVisualEffect ??= GetComponentInChildren<VisualEffect>();
        }

        protected override void Reset()
        {
            base.Reset();
            muzzleVisualEffect = GetComponentInChildren<VisualEffect>(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            runtimeShebotRifleStatData.ResetStats();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            animator.SetTrigger(ShebotAnimationData.Shebot_Rifle_idle);
            muzzleVisualEffect.Stop();
        }

        protected override void PlayHitAnimation()
        {
            if (!IsStunned)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName(ShebotAnimationData.Shebot_Rifle_idleStr) ||
                    stateInfo.IsName(ShebotAnimationData.Shebot_WalkStr))
                {
                    animator.SetTrigger(ShebotAnimationData.GettingHit_Idle);
                }
                else animator.SetTrigger(ShebotAnimationData.GettingHit_Aim);
            }
        }
    }
}
