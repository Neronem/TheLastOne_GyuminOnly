using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Entity.Scripts.NPC.Data.ForRuntime;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Entity.Scripts.Player.Data;
using _1.Scripts.Interfaces.Common;
using _1.Scripts.Interfaces.NPC;
using _1.Scripts.Manager.Core;
using _1.Scripts.Quests.Core;
using _1.Scripts.Static;
using _1.Scripts.Util;
using BehaviorDesigner.Runtime;
using Cysharp.Threading.Tasks;
using RaycastPro;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.StatControllers.Base
{
    public abstract class BaseDroneStatController : BaseNpcStatController
    {
        [Header("hackingFailPenalty")]
        [SerializeField] protected int hackingFailAttackIncrease = 3;
        [SerializeField] protected float hackingFailArmorIncrease = 3f;
        [SerializeField] protected float hackingFailPenaltyDuration = 10f;
        private CancellationTokenSource penaltyToken;
        
        private Dictionary<Transform, (Vector3 localPos, Quaternion localRot)> originalTransforms = new();
        
        protected override void Awake()
        {
            base.Awake();
            
            if (originalTransforms.Count == 0) CacheOriginalTransforms();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ResetTransformsToOriginal();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            animator.Rebind();
            animator.Update(0f);
        }

        protected override void PlayDeathAnimation()
        {
            base.PlayDeathAnimation();
            int[] deathHashes =
            {
                DroneAnimationData.Dead1,
                DroneAnimationData.Dead2,
                DroneAnimationData.Dead3
            };
            animator.SetTrigger(deathHashes[UnityEngine.Random.Range(0, deathHashes.Length)]);
        }

        protected override void PlayHitAnimation()
        {
            animator.speed = 1f;
            int[] hitHashes =
            {
                DroneAnimationData.Hit1,
                DroneAnimationData.Hit2,
                DroneAnimationData.Hit3,
                DroneAnimationData.Hit4
            };
            animator.SetTrigger(hitHashes[UnityEngine.Random.Range(0, hitHashes.Length)]);
        }

        protected override void HackingFailurePenalty()
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
        
        private void CacheOriginalTransforms(Transform parent = null)
        {
            if (parent == null) parent = this.transform;
            
            if (!originalTransforms.ContainsKey(parent))
            {
                originalTransforms.Add(parent, (parent.localPosition, parent.localRotation));
            }

            foreach (Transform child in parent)
            {
                CacheOriginalTransforms(child);
            }
        }

        private void ResetTransformsToOriginal()
        {
            foreach (var kvp in originalTransforms)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.localPosition = kvp.Value.localPos;
                    kvp.Key.localRotation = kvp.Value.localRot;
                }
            }
        }

        public override void DisposeAllUniTasks()
        {
            base.DisposeAllUniTasks();
            penaltyToken?.Dispose();
            penaltyToken = null;
        }
    }
}