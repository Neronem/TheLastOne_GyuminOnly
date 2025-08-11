using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _1.Scripts.Entity.Scripts.NPC.DamageConvert;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Static;
using _1.Scripts.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.StatControllers.Base
{
    public abstract class BaseShebotStatController : BaseNpcStatController
    {
        [Header("hackingFailPenalty")]
        [SerializeField] protected int hackingFailAttackIncrease = 3;
        [SerializeField] protected float hackingFailArmorIncrease = 3f;
        [SerializeField] protected float hackingFailPenaltyDuration = 10f;
        private CancellationTokenSource penaltyToken;
        
        protected override void PlayDeathAnimation()
        {
            base.PlayDeathAnimation();
            animator.SetTrigger(ShebotAnimationData.Shebot_Die);
        }
        
        protected override void HackingFailurePenalty() // 각 객체마다 패널티를 분리해야한다면 오버라이드를 자식으로 옮기기
        {
            int baseDamage = runtimeStatData.BaseDamage;
            float baseArmor = runtimeStatData.Armor;
            
            penaltyToken?.Cancel();
            penaltyToken?.Dispose();
            penaltyToken = NpcUtil.CreateLinkedNpcToken();
            
            _= DamageAndArmorIncrease(baseDamage, baseArmor, penaltyToken.Token);
            behaviorTree.SetVariableValue(BehaviorNames.ShouldAlertNearBy, true);
        }
        
        private async UniTaskVoid DamageAndArmorIncrease(int baseDamage, float baseArmor, CancellationToken token)
        {
            runtimeStatData.BaseDamage = baseDamage + hackingFailAttackIncrease;
            runtimeStatData.Armor = baseArmor + hackingFailArmorIncrease;

            await UniTask.WaitForSeconds(hackingFailPenaltyDuration, cancellationToken:token);

            runtimeStatData.BaseDamage = baseDamage;
            runtimeStatData.Armor = baseArmor;
        }

        public override void DisposeAllUniTasks()
        {
            base.DisposeAllUniTasks();
            penaltyToken?.Dispose();
            penaltyToken = null;
        }
    }
}