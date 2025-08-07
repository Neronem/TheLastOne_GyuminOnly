using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action
{
	[TaskCategory("Every")]
	[TaskDescription("TimerIncrease")]
	public class TimerIncrease : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedFloat timer;

		public override TaskStatus OnUpdate()
		{
			timer.Value += Time.deltaTime;
			return TaskStatus.Success;
		}
	}
}