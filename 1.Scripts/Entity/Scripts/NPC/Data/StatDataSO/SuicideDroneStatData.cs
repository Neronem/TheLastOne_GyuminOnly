using System.Collections;
using System.Collections.Generic;
using _1.Scripts.Entity.Scripts.Common;
using _1.Scripts.Interfaces;
using _1.Scripts.Interfaces.NPC;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.Data.StatDataSO
{
    [CreateAssetMenu(fileName = "Stats", menuName = "ScriptableObjects/Entity/Enemy/SuicideDroneData")]
    public class SuicideDroneStatData : EntityStatData, IAttackable, IAlertable, IBoomable, IDetectable
    {
        [Header("Range")] 
        [SerializeField] private float attackRange;
        [SerializeField] private float boomRange;
        [SerializeField] private float detectRange;
        
        [Header("Alert")]
        [SerializeField] private float alertDuration;
        [SerializeField] private float alertRadius;
        
        public float AttackRange => attackRange;
        public float AlertDuration => alertDuration;
        public float AlertRadius => alertRadius;
        public float BoomRange => boomRange;
        public float DetectRange => detectRange;
    }
}