using System.Collections;
using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Manager.Core;
using _1.Scripts.Manager.Subs;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.ShebotOnly
{
    [TaskCategory("ShebotOnly")]
    [TaskDescription("ShebotSniperFire")]
    public class ShebotSniperFire : global::BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedTransform selfTransform;
        public SharedFloat shootInterval;
        public SharedFloat reloadingDuration;
        public SharedLineRenderer lineRenderer;
        public SharedAnimator animator;
        public SharedBool isAlerted;
        public SharedTransform targetTransform;
        public SharedVector3 targetPos;
        public SharedQuaternion baseRotation;
        public SharedLight enemyLight;
        public SharedLight allyLight;
        
        private float timer;
        private bool hasFired;
        private bool soundPlayed;
        
        public override void OnStart()
        {
            timer = 0f;
            hasFired = false;
            soundPlayed = false;
        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;

            if (targetTransform.Value == null)
            {
                targetPos.Value = Vector3.zero;
                targetTransform.Value = null;
                isAlerted.Value = false;
            }
            
            if (!hasFired)
            {
                if (!soundPlayed)
                {
                    if (shootInterval.Value > 0f) CoreManager.Instance.soundManager.PlaySFX(SfxType.Shebot, selfTransform.Value.position, index: 0);
                    else CoreManager.Instance.soundManager.PlaySFX(SfxType.Shebot, selfTransform.Value.position, index: 1);
                    soundPlayed = true;
                }
                
                if (timer >= shootInterval.Value)
                {
                    animator.Value.SetTrigger(ShebotAnimationData.Shebot_Rifle_fire);
                    lineRenderer.Value.enabled = false;
                    hasFired = true;
                    timer = 0f;
                }
            }
            else if (timer >= reloadingDuration.Value)
            {
                enemyLight.Value.enabled = false;
                allyLight.Value.enabled = false;
                isAlerted.Value = false;
                targetTransform.Value = null;
                targetPos.Value = Vector3.zero;
                baseRotation.Value = selfTransform.Value.rotation;
                
                return TaskStatus.Success;
            }
            
            return TaskStatus.Running;
        }
    }
}