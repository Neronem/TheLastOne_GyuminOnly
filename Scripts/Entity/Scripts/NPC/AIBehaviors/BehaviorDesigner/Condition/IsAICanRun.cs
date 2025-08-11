using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
	[TaskCategory("Every")]
	[TaskDescription("IsAICanRun")]
	public class IsAICanRun : Conditional
	{
		public SharedBool canRun;
		
		public override TaskStatus OnUpdate()
		{
			return canRun.Value ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}
