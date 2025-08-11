using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
	[TaskCategory("Every")]
	[TaskDescription("IsNotShouldAlertNearBy")]
	public class IsNotShouldAlertNearBy : Conditional
	{
		public SharedBool shouldAlertNearBy;

		public override TaskStatus OnUpdate()
		{
			return shouldAlertNearBy.Value ? TaskStatus.Failure : TaskStatus.Success;
		}
	}
}