using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
	[TaskCategory("Every")]
	[TaskDescription("IsTargetInFOV")]
	public class IsTargetInFOV : Conditional
	{
		public SharedCollider myCollider;
		public SharedVector3 targetPos;
		public SharedFloat maxViewDistance;
		public SharedBaseNpcStatController statController;
		
		public override TaskStatus OnUpdate()
		{
			if (NpcUtil.IsTargetVisible(myCollider.Value.bounds.center, targetPos.Value, maxViewDistance.Value, statController.Value.RuntimeStatData.IsAlly))
			{
				return TaskStatus.Success;
			}
			
			return TaskStatus.Failure;
		}
	}
}