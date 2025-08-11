using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
	[TaskCategory("Every")]
	[TaskDescription("IsNotAlerted")]
	public class IsNotAlerted : Conditional
	{
		public SharedBool isAlerted;

		public override TaskStatus OnUpdate()
		{
			return isAlerted.Value ? TaskStatus.Failure : TaskStatus.Success;
		}
	}
}