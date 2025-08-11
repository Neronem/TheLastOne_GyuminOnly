using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Static;
using _1.Scripts.Util;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.ShebotOnly
{
    [TaskCategory("ShebotOnly")]
    [TaskDescription("ShebotRifleFire")]
    public class ShebotRifleFire : global::BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedAnimator animator;
        public SharedTransform selfTransform;
        public SharedTransform targetTransform;
        public SharedTransform rifleTransform;
        public SharedVector3 targetPosition;
        public SharedBaseNpcStatController statController;
        public SharedBool isAttacking;
        
        private Collider targetChset;
        
        public override void OnStart()
        {
            if (targetTransform.Value != null)
            {
                if (!isAttacking.Value)
                {
                    animator.Value.SetTrigger(ShebotAnimationData.Shebot_Rifle_fire_2);
                    isAttacking.Value = true;
                }
                int layer = statController.Value.RuntimeStatData.IsAlly ? LayerConstants.Chest_E : LayerConstants.Chest_P;
                targetChset = NpcUtil.FindColliderOfLayerInChildren(targetTransform.Value.gameObject, layer);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (targetTransform.Value == null) return TaskStatus.Failure;
            
            AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);
            
            if (stateInfo.normalizedTime > 1.0)
            {
                isAttacking.Value = false;
                return TaskStatus.Success;
            }
            
            targetPosition.Value = targetChset.bounds.center;
            NpcUtil.LookAtTarget(selfTransform.Value, targetTransform.Value.position, additionalYangle:50);
            NpcUtil.LookAtTarget(rifleTransform.Value, targetPosition.Value, pinY:false);
            return TaskStatus.Running;
        }
    }
}