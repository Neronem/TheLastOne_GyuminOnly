using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
	[TaskCategory("Every")]
	[TaskDescription("IsAlive")]
	public class IsAlive : Conditional
	{
		public SharedBool isDead;
		
		public override TaskStatus OnUpdate()
		{
			return isDead.Value ? TaskStatus.Failure : TaskStatus.Success;
		}
	}
}