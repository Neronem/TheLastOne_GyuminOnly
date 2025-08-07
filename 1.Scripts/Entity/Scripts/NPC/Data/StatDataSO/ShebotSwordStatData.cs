using System.Collections;
using System.Collections.Generic;
using _1.Scripts.Entity.Scripts.Common;
using _1.Scripts.Interfaces.NPC;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.Data.StatDataSO 
{
    [CreateAssetMenu(fileName = "Stats", menuName = "ScriptableObjects/Entity/Enemy/ShebotSwordStatData")]
    public class ShebotSwordStatData : EntityStatData, IDetectable, IAttackable, IAlertable, IPatrolable
    {
        [Header("Range")] 
        [SerializeField] private float detectRange;
        [SerializeField] private float attackRange;
        
        [Header("Alert")]
        [SerializeField] private float alertDuration;
        [SerializeField] private float alertRadius;
        
        [Header("Patrol")]
        [SerializeField] private float minWaitingDuration;
        [SerializeField] private float maxWaitingDuration;
        [SerializeField] private float minWanderingDistance;
        [SerializeField] private float maxWanderingDistance;
        
        public float DetectRange => detectRange;
        public float AttackRange => attackRange;
        public float AlertDuration => alertDuration;
        public float AlertRadius => alertRadius;
        public float MinWaitingDuration => minWaitingDuration;
        public float MaxWaitingDuration => maxWaitingDuration;
        public float MinWanderingDistance => minWanderingDistance;
        public float MaxWanderingDistance => maxWanderingDistance;
    }
}
