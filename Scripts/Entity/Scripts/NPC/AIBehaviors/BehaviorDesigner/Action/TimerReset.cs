using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action
{
	[TaskCategory("Every")]
	[TaskDescription("TimerReset")]
	public class TimerReset : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedFloat timer;
		
		public override TaskStatus OnUpdate()
		{
			timer.Value = 0f;
			return TaskStatus.Success;
		}
	}
}