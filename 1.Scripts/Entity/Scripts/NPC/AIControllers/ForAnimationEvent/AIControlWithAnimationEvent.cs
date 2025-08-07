using System;
using System.Collections;
using System.Collections.Generic;
using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Shebot_Weapon;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Interfaces.Common;
using _1.Scripts.Interfaces.NPC;
using _1.Scripts.Manager.Core;
using _1.Scripts.Manager.Subs;
using _1.Scripts.Quests.Core;
using _1.Scripts.Static;
using _1.Scripts.Util;
using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace _1.Scripts.Entity.Scripts.NPC.AIControllers.ForAnimationEvent
{
    public class AIControlWithAnimationEvent : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private BehaviorDesigner.Runtime.BehaviorTree behaviorTree;
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Shebot_Sword sword;
        [SerializeField] private Shebot_Shield shield;
        [SerializeField] private BaseNpcStatController statController;
        [SerializeField] private ParticleSystem p_hit, p_dead, p_smoke;
        [SerializeField] private VisualEffect muzzleVisualEffect;

        [Header("Settings")]
        [SerializeField] private float gotDamagedParticleDuration = 0.5f;
        [SerializeField] private bool isDuo;

        private Coroutine gotDamagedCoroutine;
        private CoreManager coreManager;
        
        private void Awake()
        {
            behaviorTree ??= GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            animator ??= GetComponent<Animator>();
            agent ??= GetComponent<NavMeshAgent>();
            sword ??= GetComponentInChildren<Shebot_Sword>(true);
            shield ??= GetComponentInChildren<Shebot_Shield>(true);
            statController ??= GetComponent<BaseNpcStatController>();
            p_hit ??= this.TryGetChildComponent<ParticleSystem>("PlasmaExplosionEffect");
            p_dead ??= this.TryGetChildComponent<ParticleSystem>("SmallExplosionEffect");
            p_smoke ??= this.TryGetChildComponent<ParticleSystem>("SmokeEffect");
            muzzleVisualEffect ??= this.TryGetChildComponent<VisualEffect>("vfxgraph_MuzzleFlash05");
        }

        private void Reset()
        {
            behaviorTree = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            sword = GetComponentInChildren<Shebot_Sword>(true);
            shield = GetComponentInChildren<Shebot_Shield>(true);
            statController = GetComponent<BaseNpcStatController>();
            p_hit = this.TryGetChildComponent<ParticleSystem>("PlasmaExplosionEffect");
            p_dead = this.TryGetChildComponent<ParticleSystem>("SmallExplosionEffect");
            p_smoke = this.TryGetChildComponent<ParticleSystem>("SmokeEffect");
            muzzleVisualEffect = this.TryGetChildComponent<VisualEffect>("vfxgraph_MuzzleFlash05");
        }

        private void Start()
        {
            coreManager = CoreManager.Instance;
        }

        public void f_hit() //hit
        {
            // 0번 : 공격, 1번 : 삐빅 시그널. 2번 : 사망, 3번 : 맞았을때
            coreManager.soundManager.PlaySFX(SfxType.Drone, transform.position, index: 3);
            if (gotDamagedCoroutine != null)
            {
                StopCoroutine(gotDamagedCoroutine);
            }

            if (statController != null && statController is IStunnable stunnable)
            {
                if (!stunnable.IsStunned)
                {
                    gotDamagedCoroutine = StartCoroutine(DamagedParticleCoroutine());
                }
            }
        }

        private IEnumerator DamagedParticleCoroutine()
        {
            p_hit?.Play();
            yield return new WaitForSeconds(gotDamagedParticleDuration);
            p_hit?.Stop();
        }
    
        public void f_prevDead()
        {
            coreManager.soundManager.PlaySFX(SfxType.Drone, transform.position, index:1);
        }

        public void f_Dead()
        {
            p_dead?.Play();
            p_smoke?.Play();
            coreManager.soundManager.PlaySFX(SfxType.Drone, transform.position, index:2);
        }
        
        public void AIOffForAnimationEvent()
        {
            behaviorTree.SetVariableValue(BehaviorNames.CanRun, false);
        }

        public void AIOnForAnimationEvent()
        {
            if (statController != null && statController is IStunnable stunnable)
            {
                if (!stunnable.IsStunned)
                {
                    behaviorTree.SetVariableValue(BehaviorNames.CanRun, true);
                }
            }
            else
            {
                behaviorTree.SetVariableValue(BehaviorNames.CanRun, true);
            }
        }

        public void SetDestinationNullForAnimationEvent()
        {
            if (agent != null) agent.SetDestination(transform.position);
        }

        public void AgentEnabledFalseForAnimationEvent()
        {
            if (agent != null) agent.enabled = false;
        }
        
        public void DestroyObjectForAnimationEvent()
        {
            if (isDuo) this.gameObject.SetActive(false);
            else NpcUtil.DisableNpc(this.gameObject);
        }
        
        public void EnableApplyRootMotion()
        {
            animator.applyRootMotion = true;
        }

        public void InterruptBehaviorForAnimationEvent()
        {
            behaviorTree.SetVariableValue(BehaviorNames.IsInterrupted, true);
        }

        public void PlaySoundSignalForAnimationEvent()
        {
            coreManager.soundManager.PlaySFX(SfxType.Drone, transform.position, index: 1);
        }
        
        public void PlaySoundBumForAnimationEvent()
        {
            coreManager.soundManager.PlaySFX(SfxType.Drone, transform.position, index: 3);
        }
        
        public void FireForAnimationEvent() // 애니메이션 이벤트로 분리해야 원하는 타이밍에 사격가능
        {
            var muzzleTransform = behaviorTree.GetVariable(BehaviorNames.MuzzleTransform) as SharedTransform;
            var targetPos = behaviorTree.GetVariable(BehaviorNames.TargetPos) as SharedVector3;

            if (muzzleTransform != null && targetPos != null && statController != null)
            {
                Vector3 muzzlePosition = muzzleTransform.Value.position;
                Vector3 direction = (targetPos.Value - muzzlePosition).normalized;
                bool isAlly = statController.RuntimeStatData.IsAlly;
                int damage = statController.RuntimeStatData.BaseDamage;
    
                NpcUtil.FireToTarget(muzzlePosition, direction, isAlly, damage);
            }
        }

        public void PlayFootStepSoundForAnimationEvent()
        {
            coreManager.soundManager.PlaySFX(SfxType.Shebot, transform.position, index: 4);
        }
        
        #region Sword전용
        public void SwordEnableHitForAnimationEvent()
        {
            sword?.EnableHit();
        }
        
        public void SwordDisableHitForAnimationEvent()
        {
            sword?.DisableHit();
        }

        public void ShieldEnableForAnimationEvent()
        {
            shield?.EnableShield();
        }
        
        public void ShieldDisableForAnimationEvent()
        {
            shield?.DisableShield();
        }
        #endregion

        #region Sniper전용

        public void FireSniperForAnimationEvent()
        { 
            if (muzzleVisualEffect != null) muzzleVisualEffect.Play();
            FireForAnimationEvent();
        }
        #endregion
        
        #region Rifle전용

        public void FireRifleForAnimationEvent()
        { 
            if (muzzleVisualEffect != null) muzzleVisualEffect.Play();
            coreManager.soundManager.PlaySFX(SfxType.Shebot, transform.position, index: 2);
            FireForAnimationEvent();
        }
        
        #endregion

        #region Dog전용

        public void BleedingTargetForAnimationEvent()
        { 
            if (statController.TryGetRuntimeStatInterface<IBleeder>(out IBleeder bleeder))
            {
                if (behaviorTree.GetVariable(BehaviorNames.TargetTransform) is SharedTransform target && target.Value != null)
                {
                    if (target.Value.TryGetComponent(out IBleedable bleedable))
                    {
                        coreManager.soundManager.PlaySFX(SfxType.Dog, transform.position, index: 1);
                        
                        int damagePerTick = statController.RuntimeStatData.BaseDamage / bleeder.TotalBleedTick; // 정수간 연산만 가능. 데미지를 TotalTick보다 낮게하지 말 것 (CeilToint를 쓰면 데미지가 폭등)
                        bleedable.OnBleed(bleeder.TotalBleedTick, bleeder.TickInterval, damagePerTick);
                    }
                }
            }
        }

        #endregion
    }
}
