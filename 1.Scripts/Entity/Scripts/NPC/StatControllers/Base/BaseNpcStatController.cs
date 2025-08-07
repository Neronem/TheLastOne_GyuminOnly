using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _1.Scripts.Entity.Scripts.Common;
using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.DamageConvert;
using _1.Scripts.Entity.Scripts.NPC.Data.ForRuntime;
using _1.Scripts.Entity.Scripts.Player.Data;
using _1.Scripts.Interfaces;
using _1.Scripts.Interfaces.Common;
using _1.Scripts.Interfaces.NPC;
using _1.Scripts.Manager.Core;
using _1.Scripts.Manager.Subs;
using _1.Scripts.Quests.Core;
using _1.Scripts.Static;
using _1.Scripts.Util;
using _1.Scripts.UI.InGame.HackingProgress;
using BehaviorDesigner.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace _1.Scripts.Entity.Scripts.Npc.StatControllers.Base
{
    [Serializable] public enum EnemyType
    {
        ReconDrone,
        SuicideDrone,
        ReconDroneNotHackable,
        SuicideDroneNotHackable,
        BattleRoomReconDrone,
        ShebotSniper,
        ShebotSword,
        ShebotRifle,
        ShebotRifleDuo,
        ShebotSwordDogDuo,
        DogSolo,
    }
    
    /// <summary>
    /// Npc 스텟 공통로직 정의
    /// </summary>
    public abstract class BaseNpcStatController : MonoBehaviour, IStunnable, IHackable, IBleedable
    {
        // 자식마다 들고있는 런타임 스탯을 부모가 가지고 있도록 함
        [Header("StatData")]
        protected RuntimeEntityStatData runtimeStatData;
        public RuntimeEntityStatData RuntimeStatData => runtimeStatData;
        public bool IsDead { get; private set; }
        
        // 조절할 컴포넌트
        [Header("Components")]
        [SerializeField] protected Animator animator;
        [SerializeField] protected BehaviorTree behaviorTree;
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] private Collider[] colliders;
        [SerializeField] private Light[] lights;
        [SerializeField] private DamageConvertForNpc[] damageConvertForNpc;
        [SerializeField] protected ParticleSystem onStunParticle;
        
        // 스턴 관련 플래그
        [Header("Stunned")] 
        private bool isStunned;
        public bool IsStunned => isStunned;
        
        // 해킹 관련 정보
        [Header("Hacking_Process")]
        public bool isHacking;
        [SerializeField] private bool canBeHacked = true;
        [SerializeField] private bool useSelfDefinedChance = true;
        [SerializeField] protected float hackingDuration = 3f;
        [SerializeField] protected float successChance = 0.7f;
        protected virtual bool CanBeHacked => canBeHacked; // 오버라이드해서 false로 바꾸거나, 인스펙터에서 설정
        private HackingProgressUI hackingProgressUI;
        private Dictionary<Transform, int> originalLayers = new();
        
        // 해킹 성공 시 올려야할 퀘스트 진행도들
        [Header("Hacking_Quest")]  
        [SerializeField] private bool shouldCountHackingQuest;
        [SerializeField] private int[] hackingQuestIndex;
        
        // 사망 시 올려야할 퀘스트 진행도들
        [Header("Kill_Quest")] 
        [SerializeField] private bool shouldCountKillQuest;
        [SerializeField] private int[] killQuestIndex;
        private static bool hasFirstkilled = false;

        private const int FirstKillDialogueKey = 1;
        
        // 이 스크립트에서 사용하는 토큰들
        private CancellationTokenSource hackCts;
        private CancellationTokenSource stunCts;
        private CancellationTokenSource bleedCts;

        // 사망 시 이벤트 (외부에서 등록)
        public event Action OnDeath;
        
        protected virtual void Awake()
        {
            animator ??= GetComponent<Animator>();
            behaviorTree ??= GetComponent<BehaviorTree>();
            agent ??= GetComponent<NavMeshAgent>();
            onStunParticle ??= this.TryGetChildComponent<ParticleSystem>("PlasmaExplosionEffect");
            if (damageConvertForNpc == null || damageConvertForNpc.Length == 0) damageConvertForNpc = GetComponentsInChildren<DamageConvertForNpc>();
            if (lights == null || lights.Length == 0) lights = GetComponentsInChildren<Light>();
            if (colliders == null || colliders.Length == 0) colliders = GetComponentsInChildren<Collider>();
            IsDead = false;
            
            foreach (DamageConvertForNpc convert in damageConvertForNpc) { convert.Initialize(this); }
            CacheOriginalLayers(this.transform);
        }
        
        /// <summary>
        /// Components 초기화 용도
        /// </summary>
        protected virtual void Reset()
        {
            animator = GetComponent<Animator>();
            behaviorTree = GetComponent<BehaviorTree>();
            agent = GetComponent<NavMeshAgent>();
            lights = GetComponentsInChildren<Light>();
            colliders = GetComponentsInChildren<Collider>();
            damageConvertForNpc = GetComponentsInChildren<DamageConvertForNpc>();
            onStunParticle = this.TryGetChildComponent<ParticleSystem>("PlasmaExplosionEffect");
        }
        
        /// <summary>
        /// 풀링 사용하므로 반드시 반환될때마다 초기화 해야함
        /// </summary>
        protected virtual void OnDisable()
        {
            // 사망 초기화
            IsDead = false;
            behaviorTree.SetVariableValue(BehaviorNames.IsDead, false);
            
            // 스탯 초기화
            isHacking = false;
            isStunned = false;
            if (agent != null) agent.enabled = false;
            
            if (onStunParticle != null && onStunParticle.IsAlive())
            {
                onStunParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            } 
            
            ResetLayersToOriginal();
            DisposeAllUniTasks();
        }

        protected virtual void OnEnable()
        { 
            if (CoreManager.Instance.spawnManager.IsVisible) 
                NpcUtil.SetLayerRecursively(this.gameObject, RuntimeStatData.IsAlly ? LayerConstants.StencilAlly : LayerConstants.StencilEnemy, LayerConstants.IgnoreLayerMask_ForStencil, false);
            foreach (Collider coll in colliders) { coll.enabled = true; }
 
            animator.speed = 1f;
        }

        protected abstract void PlayHitAnimation();
        protected virtual void PlayDeathAnimation() { animator.speed = 1f; }
        protected abstract void HackingFailurePenalty();

        public virtual void OnTakeDamage(int damage)
        {
            if (IsDead) return;
           
            float armorRatio = RuntimeStatData.Armor / RuntimeStatData.MaxArmor;
            float reducePercent = Mathf.Clamp01(armorRatio);
            damage = (int)(damage * (1f - reducePercent));

            RuntimeStatData.MaxHealth -= damage;

            if (RuntimeStatData.MaxHealth <= 0) // 사망
            {
                if (!hasFirstkilled)
                {
                    hasFirstkilled = true;
                    CoreManager.Instance.dialogueManager.TriggerDialogue(FirstKillDialogueKey);
                }

                if (CoreManager.Instance.sceneLoadManager.CurrentScene == SceneType.Stage2)
                {
                    GameEventSystem.Instance.RaiseEvent(runtimeStatData.SpawnIndex);
                }
                else
                {
                    if (shouldCountKillQuest && !RuntimeStatData.IsAlly)
                    {
                        foreach (int index in killQuestIndex) GameEventSystem.Instance.RaiseEvent(index);
                    }   
                }

                foreach (Light objlight in lights) { objlight.enabled = false; }
                foreach (Collider coll in colliders) { coll.enabled = false; }
                
                behaviorTree.SetVariableValue(BehaviorNames.IsDead, true);
                PlayDeathAnimation();
                if (!runtimeStatData.IsAlly)
                    CoreManager.Instance.gameManager.Player.PlayerCondition.OnRecoverFocusGauge(FocusGainType.Kill);
                OnDeath?.Invoke();
                IsDead = true;
            }
            else
            {
                PlayHitAnimation();
            }
        }

        public void OnBleed(int totalTick, float tickInterval, int damagePerTick)
        {
            bleedCts?.Cancel();
            bleedCts?.Dispose();
            bleedCts = NpcUtil.CreateLinkedNpcToken();
            
            _= BleedAsync(totalTick, tickInterval, damagePerTick, bleedCts.Token);
        }

        private async UniTaskVoid BleedAsync(int totalTick, float tickInterval, int damagePerTick, CancellationToken token)
        {
            for (int i = 0; i < totalTick; i++)
            {
                OnTakeDamage(damagePerTick);
                await UniTask.WaitForSeconds(tickInterval, cancellationToken:token);
            }
        }

        #region 상호작용
        public void Hacking(float chance)
        {
            if (IsDead || !CanBeHacked || isHacking || RuntimeStatData.IsAlly) return;

            var obj = CoreManager.Instance.objectPoolManager.Get("HackingProgressUI");
            hackingProgressUI = obj.GetComponent<HackingProgressUI>();
            hackingProgressUI.SetTarget(transform);
            hackingProgressUI.gameObject.SetActive(true);
            hackingProgressUI.SetProgress(0f);
            
            if (isStunned)
            {
                hackingProgressUI.SetProgress(1f);
                HackingSuccess();
                return;
            }

            float finalChance = useSelfDefinedChance ? successChance : chance;
            
            hackCts?.Cancel();
            hackCts?.Dispose();
            hackCts = NpcUtil.CreateLinkedNpcToken();
            _ = HackingProcessAsync(finalChance, hackCts.Token);
        }

        private async UniTaskVoid HackingProcessAsync(float chance, CancellationToken token)
        {
            try
            {
                isHacking = true;

                // 1. 드론 멈추기
                float stunDurationOnHacking = hackingDuration + 1f; // 스턴 중 해킹결과가 영향 끼치지 않게 더 길게 설정
                OnStunned(stunDurationOnHacking);

                // 2. 해킹 시도 시간 기다림
                float time = 0f;
                while (time < hackingDuration)
                {
                    time += Time.deltaTime;
                    hackingProgressUI.SetProgress(time / hackingDuration);
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
                }

                // 3. 확률 판정
                bool success = UnityEngine.Random.value < chance;

                if (success)
                {
                    HackingSuccess();
                }
                else
                {
                    hackingProgressUI.OnFail();
                    HackingFailurePenalty();
                }
            }
            catch (Exception ex)
            {
                hackingProgressUI.OnCanceled();
            }
            finally
            {
                isHacking = false;
            }
        }
        
        protected virtual void HackingSuccess()
        {
            RuntimeStatData.IsAlly = true;
            NpcUtil.SetLayerRecursively_Hacking(this.gameObject);
            
            if (CoreManager.Instance.sceneLoadManager.CurrentScene == SceneType.Stage2)
            {
                GameEventSystem.Instance.RaiseEvent(runtimeStatData.SpawnIndex);
            }
            else
            {
                if (shouldCountHackingQuest)
                {
                    foreach (int index in hackingQuestIndex) {GameEventSystem.Instance.RaiseEvent(index);}
                }
            }
            
            CoreManager.Instance.gameManager.Player.PlayerCondition.OnRecoverFocusGauge(FocusGainType.Hack);
            hackingProgressUI.OnSuccess();
        }
        
        public void OnStunned(float duration = 3f)
        {
            if (IsDead) return;
            
            stunCts?.Cancel();
            stunCts?.Dispose();
            stunCts = NpcUtil.CreateLinkedNpcToken();
            _ = OnStunnedAsync(duration, stunCts.Token);
        }
        
        private async UniTaskVoid OnStunnedAsync(float duration, CancellationToken token)
        {
            isStunned = true;
            behaviorTree.SetVariableValue(BehaviorNames.CanRun, false);
            ResetAIState();
            animator.speed = 0f;
            
            onStunParticle.Play();
            await UniTask.WaitForSeconds(duration, cancellationToken: token); // 원하는 시간만큼 유지
            
            if (onStunParticle != null && onStunParticle.IsAlive())
            {
                onStunParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            isStunned = false;
            animator.speed = 1f;
            behaviorTree.SetVariableValue(BehaviorNames.CanRun, true);
        }
        
        protected virtual void ResetAIState()
        {
            behaviorTree.SetVariableValue(BehaviorNames.TargetTransform, null);
            behaviorTree.SetVariableValue(BehaviorNames.TargetPos, Vector3.zero);
            behaviorTree.SetVariableValue(BehaviorNames.ShouldLookTarget, false);
            behaviorTree.SetVariableValue(BehaviorNames.IsAlerted, false);
            behaviorTree.SetVariableValue(BehaviorNames.Timer, 0f);

            var enemyLight = behaviorTree.GetVariable(BehaviorNames.EnemyLight) as SharedLight;
            var allyLight = behaviorTree.GetVariable(BehaviorNames.AllyLight) as SharedLight;
            var agent = behaviorTree.GetVariable(BehaviorNames.Agent) as SharedNavMeshAgent;
            var selfTransform = behaviorTree.GetVariable(BehaviorNames.SelfTransform) as SharedTransform;

            if (enemyLight != null && enemyLight.Value != null)
            {
                enemyLight.Value.enabled = false;
            }

            if (allyLight != null && allyLight.Value != null)
            {
                allyLight.Value.enabled = false;
            }

            if (agent != null && agent.Value != null && selfTransform != null && selfTransform.Value != null)
            {
                agent.Value.SetDestination(selfTransform.Value.position);
            }
        }
        #endregion
        
        /// <summary>
        /// 자식마다 들고있는 런타임 스탯에 특정 인터페이스가 있는지 검사 후, 그 인터페이스를 반환
        /// </summary>
        /// <param name="result"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TryGetRuntimeStatInterface<T>(out T result) where T : class
        {
            result = null;

            result = RuntimeStatData as T;
            if (result == null)
            {
                Debug.LogWarning($"{GetType().Name}의 RuntimeStatData에 {typeof(T).Name} 인터페이스가 없음");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 외부에서 사망 처리 해야한다면 사용
        /// </summary>
        public void Dead()
        {
            IsDead = true;
        }
        
        /// <summary>
        /// 첫 레이어 상태들을 재귀적으로 저장
        /// </summary>
        /// <param name="parent"></param>
        private void CacheOriginalLayers(Transform parent = null)
        {
            if (parent == null) parent = transform;
            
            if (!originalLayers.ContainsKey(parent))
            {
                originalLayers.Add(parent, parent.gameObject.layer);
            }

            foreach (Transform child in parent)
            {
                CacheOriginalLayers(child);
            }
        }
        
        /// <summary>
        /// OnEnable 시 레이어 기본으로 되돌림
        /// </summary>
        private void ResetLayersToOriginal()
        {
            foreach (var kvp in originalLayers)
            {
                if (kvp.Key != null) // 혹시 파괴된 오브젝트가 있을 수 있으니 체크
                {
                    kvp.Key.gameObject.layer = kvp.Value;
                }
            }
        }

        public virtual void DisposeAllUniTasks()
        {
            hackCts?.Dispose();
            hackCts = null;
            stunCts?.Dispose();
            stunCts = null;
            bleedCts?.Dispose();
            bleedCts = null;
        }
    }
}