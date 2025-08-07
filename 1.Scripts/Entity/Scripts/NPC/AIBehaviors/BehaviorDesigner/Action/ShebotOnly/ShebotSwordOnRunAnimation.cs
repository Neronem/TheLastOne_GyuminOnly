using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.ShebotOnly
{
	[TaskCategory("ShebotOnly")]
	[TaskDescription("ShebotSwordOnRunAnimation")]
	public class ShebotSwordOnRunAnimation : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedAnimator animator;
		
		public override void OnStart()
		{
			AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);

			if (!stateInfo.IsName(ShebotAnimationData.Shebot_Sword_Run_AnimationNameStr))
			{
				animator.Value.SetTrigger(ShebotAnimationData.ShebotSword_Run);
			}
		}

		public override TaskStatus OnUpdate()
		{
			return TaskStatus.Success;
		}
	}
}
