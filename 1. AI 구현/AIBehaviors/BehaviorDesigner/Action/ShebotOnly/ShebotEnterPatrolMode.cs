using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.ShebotOnly
{
	[TaskCategory("ShebotOnly")]
	[TaskDescription("ShebotEnterPatrolMode")]
	public class ShebotEnterPatrolMode : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedBool shouldLookTarget;
		public SharedAnimator animator;
		public SharedBaseNpcStatController statController;
		public bool isRifle;
		
		public override TaskStatus OnUpdate()
		{
			if (!statController.Value.RuntimeStatData.IsAlly)
			{
				AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);
				if (!isRifle && !stateInfo.IsName(ShebotAnimationData.Shebot_IdleStr))
				{
					animator.Value.SetTrigger(ShebotAnimationData.Shebot_Idle);
				}
				else if (isRifle && !stateInfo.IsName(ShebotAnimationData.Shebot_Rifle_idleStr))
				{
					animator.Value.SetTrigger(ShebotAnimationData.Shebot_Rifle_idle);
				}
			}
			
			shouldLookTarget.Value = false; // 보통 노드구조 맨 끝자락에 배회할때 쓰니까 추가
			
			return TaskStatus.Success;
		}
	}
}