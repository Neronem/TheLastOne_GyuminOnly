using System.Collections;
using System.Collections.Generic;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Interfaces.Common;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.DamageConvert
{
    public class DamageConvertForNpc : MonoBehaviour, IDamagable
    {
        [field: Header("Damage Multiplier")] 
        [field: SerializeField] public float DamageMultiplier { get; private set; }

        private BaseNpcStatController _statController;

        public void Initialize(BaseNpcStatController statController)
        {
            this._statController = statController;
        }

        public void OnTakeDamage(int damage)
        { 
            _statController?.OnTakeDamage(Mathf.CeilToInt(damage *  DamageMultiplier));
        }
    }
}
