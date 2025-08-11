using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.DroneOnly
{
	[TaskCategory("DroneOnly")]
	[TaskDescription("DroneEnterPatrolMode")]
	public class DroneEnterPatrolMode : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedBool shouldLookTarget;
		public SharedAnimator animator;
		
		public override TaskStatus OnUpdate()
		{
			AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);
			if (!stateInfo.IsName(DroneAnimationData.Hit1Str) &&
			    !stateInfo.IsName(DroneAnimationData.Hit2Str) &&
			    !stateInfo.IsName(DroneAnimationData.Hit3Str) &&
			    !stateInfo.IsName(DroneAnimationData.Hit4Str) &&
			    !stateInfo.IsName(DroneAnimationData.Idle1Str))
			{
				animator.Value.SetTrigger(DroneAnimationData.Idle1);
			}
			
			shouldLookTarget.Value = false; // 보통 노드구조 맨 끝자락에 배회할때 쓰니까 추가
			
			return TaskStatus.Success;
		}
	}
}