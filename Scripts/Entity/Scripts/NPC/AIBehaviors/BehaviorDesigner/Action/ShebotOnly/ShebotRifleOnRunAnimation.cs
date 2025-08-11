using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.ShebotOnly
{
    [TaskCategory("ShebotOnly")]
    [TaskDescription("ShebotRifleOnRunAnimation")]
    public class ShebotRifleOnRunAnimation : global::BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedAnimator animator;
        
        public override void OnStart()
        {
            AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);

            if (!stateInfo.IsName(ShebotAnimationData.Shebot_Rifle_RunStr))
            {
                animator.Value.SetTrigger(ShebotAnimationData.Shebot_Rifle_Run);
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}