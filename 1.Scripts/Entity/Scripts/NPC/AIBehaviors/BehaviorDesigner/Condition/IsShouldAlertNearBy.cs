using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
	[TaskCategory("Every")]
	[TaskDescription("IsShouldAlertNearBy")]
	public class IsShouldAlertNearBy : Conditional
	{
		public SharedBool isShouldAlertNearBy;
		
		public override TaskStatus OnUpdate()
		{
			if (isShouldAlertNearBy.Value)
			{
				return TaskStatus.Success;
			}
			
			return TaskStatus.Failure;
		}
	}
}