using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
	[TaskCategory("Every")]
	[TaskDescription("ReturnSuccessOnly")]
	public class ReturnSuccessOnly : Conditional
	{
		public override TaskStatus OnUpdate()
		{
			return TaskStatus.Success;
		}
	}
}