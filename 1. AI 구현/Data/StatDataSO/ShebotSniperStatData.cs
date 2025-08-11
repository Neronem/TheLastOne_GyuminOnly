using System.Collections;
using System.Collections.Generic;
using _1.Scripts.Entity.Scripts.Common;
using _1.Scripts.Interfaces.NPC;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.Data.StatDataSO
{
    [CreateAssetMenu(fileName = "Stats", menuName = "ScriptableObjects/Entity/Enemy/ShebotSniperStatData")]
    public class ShebotSniperStatData : EntityStatData, IDetectable, IAttackable, IAlertable
    {
        [Header("Range")] 
        [SerializeField] private float detectRange;
        [SerializeField] private float attackRange;
        
        [Header("Alert")]
        [SerializeField] private float alertDuration;
        [SerializeField] private float alertRadius;
        
        public float DetectRange => detectRange;
        public float AttackRange => attackRange;
        public float AlertDuration => alertDuration;
        public float AlertRadius => alertRadius;
    }
}