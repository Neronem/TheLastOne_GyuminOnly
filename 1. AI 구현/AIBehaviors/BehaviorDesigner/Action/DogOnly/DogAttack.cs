using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Static;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.DogOnly
{
    [TaskCategory("DogOnly")]
    [TaskDescription("DogAttack")]
    public class DogAttack : global::BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedAnimator animator;
        public SharedTransform selfTransform;
        public SharedTransform targetTransform;
        public SharedBaseNpcStatController statController;
        public SharedBool isAttacking;
    
        private Collider targetChset;
    
        public override void OnStart()
        {
            if (!isAttacking.Value)
            {
                int[] attackHashes =
                {
                    DogAnimationData.Dog_Attack1,
                    DogAnimationData.Dog_Attack2,
                    DogAnimationData.Dog_Attack3
                };
                
                animator.Value.SetTrigger(attackHashes[UnityEngine.Random.Range(0, attackHashes.Length)]);
                isAttacking.Value = true;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (targetTransform.Value == null)
            {
                isAttacking.Value = false;
                return TaskStatus.Failure;
            }
            
            AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);
        
            if (stateInfo.normalizedTime > 1.0)
            {
                isAttacking.Value = false;
                return TaskStatus.Success;
            }
        
            NpcUtil.LookAtTarget(selfTransform.Value, targetTransform.Value.position);
            return TaskStatus.Running;
        }
    }
}