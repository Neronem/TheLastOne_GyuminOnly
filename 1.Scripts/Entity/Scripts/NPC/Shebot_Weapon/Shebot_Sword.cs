using System;
using System.Collections;
using System.Collections.Generic;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Interfaces.Common;
using _1.Scripts.Manager.Core;
using _1.Scripts.Manager.Subs;
using _1.Scripts.Static;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.Shebot_Weapon
{
    public class Shebot_Sword : MonoBehaviour
    {
        private BaseNpcStatController statController;
        private bool canhit;
        
        private void Awake()
        {
            statController = GetComponentInParent<BaseNpcStatController>();
        }

        private void OnEnable()
        {
            canhit = false;
        }

        public void EnableHit()
        {
            canhit = true;
        }

        public void DisableHit()
        {
            canhit = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject == this.gameObject) return;
            if (!canhit) return;
            int layerMask = statController.RuntimeStatData.IsAlly
                ? LayerConstants.EnemyLayerMask
                : LayerConstants.AllyLayerMask;
            if ((LayerConstants.ToLayerMask(other.gameObject.layer) & layerMask) == 0) return;
            
            if (other.TryGetComponent(out IDamagable damagable))
            {
                canhit = false;
                CoreManager.Instance.soundManager.PlaySFX(SfxType.Shebot, transform.position, index:3); // 타격음 출력
                damagable.OnTakeDamage(statController.RuntimeStatData.BaseDamage);
            }
        }
    }
}
