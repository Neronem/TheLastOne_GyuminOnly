using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.Shebot_Weapon
{
    public class Shebot_Shield : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            this.gameObject.SetActive(false);
        }

        public void EnableShield()
        {
            this.gameObject.SetActive(true);
            
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName("open"))
            {
                animator.SetTrigger("Shield_Open");
            }
        }

        public void DisableShieldAnimaton()
        {
            animator.SetTrigger("Shield_Close");
        }

        public void DisableShield()
        {
            this.gameObject.SetActive(false);
        }
    }
}
