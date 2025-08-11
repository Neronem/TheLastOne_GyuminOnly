using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.DogOnly
{
    [TaskCategory("DogOnly")]
    [TaskDescription("DogOnRunAnimation")]
    public class DogOnRunAnimation : global::BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedAnimator animator;
        
        public override void OnStart()
        {
            AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);

            if (!stateInfo.IsName(DogAnimationData.Dog_RunStr))
            {
                animator.Value.SetTrigger(DogAnimationData.Dog_Run);
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}