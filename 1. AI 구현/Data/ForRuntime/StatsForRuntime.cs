using _1.Scripts.Entity.Scripts.Common;
using _1.Scripts.Entity.Scripts.NPC.Data;
using _1.Scripts.Entity.Scripts.NPC.Data.StatDataSO;
using _1.Scripts.Interfaces;
using _1.Scripts.Interfaces.Common;
using _1.Scripts.Interfaces.NPC;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.Data.ForRuntime
{
    /// <summary>
    /// SO를 조작하면 전체가 바뀌기에, 복사본을 생성하는 클래스들 모음
    /// </summary>
    public class RuntimeEntityStatData
    {
        private readonly EntityStatData originalSO;
        
        public int SpawnIndex { get; set; }
        public string EntityName { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsAlly { get; set; }
        public float MaxHealth { get; set; }
        public int BaseDamage { get; set; }
        public float BaseAttackRate { get; set; }
        public float Armor { get; set; }
        public float MaxArmor { get; set; }
        public float MoveSpeed { get; set; }
        public float RunMultiplier { get; set; }
        public float WalkMultiplier { get; set; } 

        public AudioClip[] footStepSounds;
        public AudioClip[] hitSounds;
        public AudioClip[] deathSounds;
        
        protected RuntimeEntityStatData(EntityStatData so)
        {
            originalSO = so;
            
            SpawnIndex = so.spawnIndex;
            
            EntityName = so.entityName;
            IsPlayer = so.isPlayer;
            IsAlly = so.isAlly;

            MaxHealth = so.maxHealth;
            BaseDamage = so.baseDamage;
            BaseAttackRate = so.baseAttackRate;
            Armor = so.armor;
            MaxArmor = so.maxArmor;
            
            MoveSpeed = so.moveSpeed;
            RunMultiplier = so.runMultiplier;
            WalkMultiplier = so.walkMultiplier;

            footStepSounds = so.footStepSounds;
            hitSounds = so.hitSounds;
            deathSounds = so.deathSounds;
        }
        
        /// <summary>
        /// 공통 스탯 리셋
        /// </summary>
        public virtual void ResetStats()
        {
            EntityName = originalSO.entityName;
            IsPlayer = originalSO.isPlayer;
            IsAlly = originalSO.isAlly;
            
            MaxHealth = originalSO.maxHealth;
            BaseDamage = originalSO.baseDamage;
            BaseAttackRate = originalSO.baseAttackRate;
            Armor = originalSO.armor;
            MaxArmor = originalSO.maxArmor;
            
            MoveSpeed = originalSO.moveSpeed;
            RunMultiplier = originalSO.runMultiplier;
            WalkMultiplier = originalSO.walkMultiplier;
            
            footStepSounds = originalSO.footStepSounds;
            hitSounds = originalSO.hitSounds;
            deathSounds = originalSO.deathSounds;
        }
    }
    
    public class RuntimeReconDroneStatData : RuntimeEntityStatData, IDetectable, IAttackable, IAlertable, IPatrolable
    {
        private readonly ReconDroneStatData reconSO;
        
        public float DetectRange { get; set; }
        public float AttackRange { get; set; }
        public float AlertDuration { get; set; }
        public float AlertRadius { get; set; }
        public float MinWaitingDuration { get; set; }
        public float MaxWaitingDuration { get; set;  }
        public float MinWanderingDistance { get; set; }
        public float MaxWanderingDistance { get; set; }

        // 생성자: ReconDroneData(SO)에서 값 복사 + 베이스 생성자 호출
        public RuntimeReconDroneStatData(ReconDroneStatData so) : base(so)
        {
            reconSO = so;
            
            DetectRange = so.DetectRange;
            AttackRange = so.AttackRange;
            AlertDuration = so.AlertDuration;
            AlertRadius = so.AlertRadius;
            MinWaitingDuration = so.MinWaitingDuration;
            MaxWaitingDuration = so.MaxWaitingDuration;
            MinWanderingDistance = so.MinWanderingDistance;
            MaxWanderingDistance = so.MaxWanderingDistance;
        }

        public override void ResetStats()
        {
            base.ResetStats();
            
            DetectRange = reconSO.DetectRange;
            AttackRange = reconSO.AttackRange;
            AlertDuration = reconSO.AlertDuration;
            AlertRadius = reconSO.AlertRadius;
            MinWaitingDuration = reconSO.MinWaitingDuration;
            MaxWaitingDuration = reconSO.MaxWaitingDuration;
            MinWanderingDistance = reconSO.MinWanderingDistance;
            MaxWanderingDistance = reconSO.MaxWanderingDistance;
        }
    }

    public class RuntimeSuicideDroneStatData : RuntimeEntityStatData, IAlertable, IAttackable, IBoomable, IDetectable
    {
        private readonly SuicideDroneStatData suicideSO; // 원본 SO 저장

        public float AttackRange { get; set; }
        public float AlertDuration { get; set; }
        public float AlertRadius { get; set; }
        public float BoomRange { get; set; }
        public float DetectRange { get; set; }

        public RuntimeSuicideDroneStatData(SuicideDroneStatData so) : base(so)
        {
            suicideSO = so;
            AttackRange = so.AttackRange;
            AlertDuration = so.AlertDuration;
            AlertRadius = so.AlertRadius;
            BoomRange = so.BoomRange;
            DetectRange = so.DetectRange;
        }

        public override void ResetStats()
        {
            base.ResetStats(); // 부모 공통 스탯 리셋

            AttackRange = suicideSO.AttackRange;
            AlertDuration = suicideSO.AlertDuration;
            AlertRadius = suicideSO.AlertRadius;
            BoomRange = suicideSO.BoomRange;
            DetectRange = suicideSO.DetectRange;
        }
    }
    
    public class RuntimeShebotSwordStatData : RuntimeEntityStatData, IDetectable, IAttackable, IAlertable, IPatrolable
    {
        private readonly ShebotSwordStatData swordSO;
        
        public float DetectRange { get; set; }
        public float AttackRange { get; set; }
        public float AlertDuration { get; set; }
        public float AlertRadius { get; set; }
        public float MinWaitingDuration { get; set; }
        public float MaxWaitingDuration { get; set;  }
        public float MinWanderingDistance { get; set; }
        public float MaxWanderingDistance { get; set; }

        public RuntimeShebotSwordStatData(ShebotSwordStatData so) : base(so)
        {
            swordSO = so;
            
            DetectRange = so.DetectRange;
            AttackRange = so.AttackRange;
            AlertDuration = so.AlertDuration;
            AlertRadius = so.AlertRadius;
            MinWaitingDuration = so.MinWaitingDuration;
            MaxWaitingDuration = so.MaxWaitingDuration;
            MinWanderingDistance = so.MinWanderingDistance;
            MaxWanderingDistance = so.MaxWanderingDistance;
        }

        public override void ResetStats()
        {
            base.ResetStats();
            
            DetectRange = swordSO.DetectRange;
            AttackRange = swordSO.AttackRange;
            AlertDuration = swordSO.AlertDuration;
            AlertRadius = swordSO.AlertRadius;
            MinWaitingDuration = swordSO.MinWaitingDuration;
            MaxWaitingDuration = swordSO.MaxWaitingDuration;
            MinWanderingDistance = swordSO.MinWanderingDistance;
            MaxWanderingDistance = swordSO.MaxWanderingDistance;
        }
    }
    
    public class RuntimeShebotSniperStatData : RuntimeEntityStatData, IDetectable, IAttackable, IAlertable
    {
        private readonly ShebotSniperStatData sniperSO;
        
        public float DetectRange { get; set; }
        public float AttackRange { get; set; }
        public float AlertDuration { get; set; }
        public float AlertRadius { get; set; }

        public RuntimeShebotSniperStatData(ShebotSniperStatData so) : base(so)
        {
            sniperSO = so;

            DetectRange = so.DetectRange;
            AttackRange = so.AttackRange;
            AlertDuration = so.AlertDuration;
            AlertRadius = so.AlertRadius;
        }

        public override void ResetStats()
        {
            base.ResetStats();
            
            DetectRange = sniperSO.DetectRange;
            AttackRange = sniperSO.AttackRange;
            AlertDuration = sniperSO.AlertDuration;
            AlertRadius = sniperSO.AlertRadius;
        }
    }
    
    public class RuntimeShebotRifleStatData : RuntimeEntityStatData, IDetectable, IAttackable, IAlertable, IPatrolable
    {
        private readonly ShebotRifleStatData rifleSO;
        
        public float DetectRange { get; set; }
        public float AttackRange { get; set; }
        public float AlertDuration { get; set; }
        public float AlertRadius { get; set; }
        public float MinWaitingDuration { get; set; }
        public float MaxWaitingDuration { get; set;  }
        public float MinWanderingDistance { get; set; }
        public float MaxWanderingDistance { get; set; }

        public RuntimeShebotRifleStatData(ShebotRifleStatData so) : base(so)
        {
            rifleSO = so;
            
            DetectRange = so.DetectRange;
            AttackRange = so.AttackRange;
            AlertDuration = so.AlertDuration;
            AlertRadius = so.AlertRadius;
            MinWaitingDuration = so.MinWaitingDuration;
            MaxWaitingDuration = so.MaxWaitingDuration;
            MinWanderingDistance = so.MinWanderingDistance;
            MaxWanderingDistance = so.MaxWanderingDistance;
        }

        public override void ResetStats()
        {
            base.ResetStats();
            
            DetectRange = rifleSO.DetectRange;
            AttackRange = rifleSO.AttackRange;
            AlertDuration = rifleSO.AlertDuration;
            AlertRadius = rifleSO.AlertRadius;
            MinWaitingDuration = rifleSO.MinWaitingDuration;
            MaxWaitingDuration = rifleSO.MaxWaitingDuration;
            MinWanderingDistance = rifleSO.MinWanderingDistance;
            MaxWanderingDistance = rifleSO.MaxWanderingDistance;
        }
    }   
    
    public class RuntimeDogStatData : RuntimeEntityStatData, IDetectable, IAttackable, IAlertable, IPatrolable, IBleeder
    {
        private readonly DogStatData dogSO;
        
        public float DetectRange { get; set; }
        public float AttackRange { get; set; }
        public float AlertDuration { get; set; }
        public float AlertRadius { get; set; }
        public float MinWaitingDuration { get; set; }
        public float MaxWaitingDuration { get; set;  }
        public float MinWanderingDistance { get; set; }
        public float MaxWanderingDistance { get; set; }
        public float TickInterval { get; set; }
        public int TotalBleedTick { get; set; }

        public RuntimeDogStatData(DogStatData so) : base(so)
        {
            dogSO = so;
            
            DetectRange = so.DetectRange;
            AttackRange = so.AttackRange;
            AlertDuration = so.AlertDuration;
            AlertRadius = so.AlertRadius;
            MinWaitingDuration = so.MinWaitingDuration;
            MaxWaitingDuration = so.MaxWaitingDuration;
            MinWanderingDistance = so.MinWanderingDistance;
            MaxWanderingDistance = so.MaxWanderingDistance;
            TickInterval = so.TickInterval;
            TotalBleedTick = so.TotalBleedTick;
        }

        public override void ResetStats()
        {
            base.ResetStats();
            
            DetectRange = dogSO.DetectRange;
            AttackRange = dogSO.AttackRange;
            AlertDuration = dogSO.AlertDuration;
            AlertRadius = dogSO.AlertRadius;
            MinWaitingDuration = dogSO.MinWaitingDuration;
            MaxWaitingDuration = dogSO.MaxWaitingDuration;
            MinWanderingDistance = dogSO.MinWanderingDistance;
            MaxWanderingDistance = dogSO.MaxWanderingDistance;
            TickInterval = dogSO.TickInterval;
            TotalBleedTick = dogSO.TotalBleedTick;
        }
    }
}