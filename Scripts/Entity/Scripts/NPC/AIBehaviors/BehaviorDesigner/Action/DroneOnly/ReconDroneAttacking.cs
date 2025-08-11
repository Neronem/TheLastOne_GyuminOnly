using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.DroneOnly
{
	[TaskCategory("DroneOnly")]
	[TaskDescription("ReconDroneAttacking")]
	public class ReconDroneAttacking : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedTransform targetTransform;
		public SharedVector3 targetPos;
		public SharedAnimator animator;
		
		public override TaskStatus OnUpdate()
		{
			if (targetTransform.Value == null || targetPos.Value == Vector3.zero)
			{
				return TaskStatus.Failure;
			}
			
			if (!targetTransform.Value.CompareTag("Player"))
			{
				var statController = targetTransform.Value.GetComponent<BaseNpcStatController>();
				if (statController == null || statController.IsDead)
				{
					return TaskStatus.Failure;
				}
			}

			if (animator.Value.GetCurrentAnimatorStateInfo(0).IsName(DroneAnimationData.FireStr))
			{
				return TaskStatus.Failure;
			}
			
			animator.Value.SetTrigger(DroneAnimationData.Fire);
			
			return TaskStatus.Success;
		}
	}
}
