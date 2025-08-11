using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
	[TaskCategory("Every")]
	[TaskDescription("IsNavMeshAgentHasPath")]
	public class IsNavMeshAgentHasPath : Conditional
	{
		public SharedNavMeshAgent agent;
		public SharedVector3 targetPosition;
		public SharedBaseNpcStatController statController;
		
		public override TaskStatus OnUpdate()
		{
			if (statController.Value.RuntimeStatData.IsAlly && targetPosition.Value == Vector3.zero)
			{
				return TaskStatus.Success;
			}
			
			if (!agent.Value.hasPath && targetPosition.Value == Vector3.zero)
			{
				return TaskStatus.Success;
			}

			return TaskStatus.Failure;
		}
	}
}	