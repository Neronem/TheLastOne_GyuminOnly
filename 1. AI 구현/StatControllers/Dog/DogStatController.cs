using System.Threading;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Entity.Scripts.NPC.Data.ForRuntime;
using _1.Scripts.Entity.Scripts.NPC.Data.StatDataSO;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Entity.Scripts.NPC.StatControllers.Base;
using _1.Scripts.Manager.Core;
using _1.Scripts.Static;
using _1.Scripts.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.StatControllers.Dog
{
    public class DogStatController : BaseNpcStatController
    {
        private RuntimeDogStatData runtimeDogStatData;
        
        [Header("hackingFailPenalty")]
        [SerializeField] protected int hackingFailAttackIncrease = 3;
        [SerializeField] protected float hackingFailArmorIncrease = 3f;
        [SerializeField] protected float hackingFailPenaltyDuration = 10f;
        private CancellationTokenSource penaltyToken;
        
        protected override void Awake()
        {
            base.Awake();
            var dogStatData = CoreManager.Instance.resourceManager.GetAsset<DogStatData>("DogStatData"); // 자신만의 데이터 가져오기
            runtimeDogStatData = new RuntimeDogStatData(dogStatData);
            runtimeStatData = runtimeDogStatData;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            runtimeDogStatData.ResetStats();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            animator.SetTrigger(DogAnimationData.Dog_Idle1);
        }
        
        protected override void PlayHitAnimation()
        {
            // Dog는 해당사항 없음
        }

        protected override void PlayDeathAnimation()
        {
            base.PlayDeathAnimation();
            int[] deathHashes =
            {
                DogAnimationData.Dog_Death1,
                DogAnimationData.Dog_Death2
            };
            animator.SetTrigger(deathHashes[UnityEngine.Random.Range(0, deathHashes.Length)]);
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