using System;
using System.Collections;
using System.Collections.Generic;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Manager.Core;
using _1.Scripts.Static;
using _1.Scripts.Util;
using UnityEngine;

namespace _1.Scripts.VisualEffects
{
    public class FocusAbleForNpc : MonoBehaviour
    {
        [SerializeField] private BaseNpcStatController statController;
        [SerializeField] private Renderer body;
        [SerializeField] private Material originalMaterial;
        [SerializeField] private Material focusMaterial;
        
        private void Awake()
        {
            statController = GetComponent<BaseNpcStatController>();
        }

        private void OnDisable()
        {
            body.material = originalMaterial;
        }

        private void OnEnable()
        {
            if (CoreManager.Instance.spawnManager.IsFocused) body.material = focusMaterial;
        }
        
        public void FocusOnOrNot(bool isOn)
        {
            if (statController == null || body == null || originalMaterial == null || focusMaterial == null) return;
            
            if (isOn && !statController.RuntimeStatData.IsAlly)
            {
                body.material = focusMaterial;
            }
            else if (!isOn)
            {
                body.material = originalMaterial;
            }
        }
    }
    
}