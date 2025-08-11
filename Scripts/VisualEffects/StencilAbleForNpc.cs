using System;
using System.Collections;
using System.Collections.Generic;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Static;
using _1.Scripts.Util;
using UnityEngine;

namespace _1.Scripts.VisualEffects
{
    public class StencilAbleForNpc : MonoBehaviour
    {
        private BaseNpcStatController statController;
        
        private void Awake()
        {
            statController = GetComponent<BaseNpcStatController>();
        }

        public void StencilLayerOnOrNot(bool isOn)
        {
            if (isOn)
            {
                int layerMask = statController.RuntimeStatData.IsAlly ? LayerConstants.StencilAlly : LayerConstants.StencilEnemy;
                NpcUtil.SetLayerRecursively(this.gameObject, layerMask, LayerConstants.IgnoreLayerMask_ForStencil, false);
            }
            else
            {
                int layerMask = statController.RuntimeStatData.IsAlly ? LayerConstants.Ally : LayerConstants.Enemy;
                NpcUtil.SetLayerRecursively(this.gameObject, layerMask, LayerConstants.IgnoreLayerMask_ForStencil, false);
            }
        }
    }
    
}
