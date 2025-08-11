using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.ShebotOnly
{
	[TaskCategory("ShebotOnly")]
	[TaskDescription("ShebotSwordAttacking")]
	public class ShebotSwordAttacking : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedAnimator animator;

		public SharedTransform selfTransform;
		public SharedTransform targetTransform;
		public SharedBaseNpcStatController statController;
		public SharedBool isAttacking;
		
		public override void OnStart()
		{
			if (targetTransform.Value != null)
			{
				if (!isAttacking.Value)
				{
					if (statController.Value.RuntimeStatData.IsAlly)
					{
						animator.Value.SetTrigger(ShebotAnimationData.Shebot_Sword_Attack_Full);
					}
					else
					{
						animator.Value.SetTrigger(ShebotAnimationData.Shebot_Sword_Attack3);
					}
					isAttacking.Value = true;
				}
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
			
			NpcUtil.LookAtTarget(selfTransform.Value, targetTransform.Value.position, 50f);
			return TaskStatus.Running;
		}
	}
}
