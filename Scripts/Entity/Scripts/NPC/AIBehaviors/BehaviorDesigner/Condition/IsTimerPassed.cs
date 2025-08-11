using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Interfaces.NPC;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
	[TaskCategory("Every")]
	[TaskDescription("IsTimerPassed")]
	public class IsTimerPassed : Conditional
	{
		public SharedFloat timer;
		public SharedBaseNpcStatController statController;
		
		public override TaskStatus OnUpdate()
		{
			if (!statController.Value.TryGetRuntimeStatInterface<IAlertable>(out var alertable))
			{
				return TaskStatus.Failure;
			}

			if (alertable.AlertDuration <= timer.Value)
			{
				return TaskStatus.Success;
			}
			
			return TaskStatus.Failure;
		}
	}
}