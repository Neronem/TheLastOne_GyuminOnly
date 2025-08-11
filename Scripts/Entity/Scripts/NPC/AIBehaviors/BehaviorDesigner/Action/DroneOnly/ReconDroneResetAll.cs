using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.DroneOnly
{
	[TaskCategory("DroneOnly")]
	[TaskDescription("ReconDroneResetAll")]
	public class ReconDroneResetAll : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedTransform selfTransform;
		public SharedTransform targetTransform;
		public SharedVector3 targetPos;
		public SharedBool shouldLookTarget;
		public SharedBool isAlerted;
		public SharedFloat timer;
		public SharedNavMeshAgent agent;
		
		public override TaskStatus OnUpdate()
		{
			targetTransform.Value = null;
			targetPos.Value = Vector3.zero;
			shouldLookTarget.Value = false;
			isAlerted.Value = false;
			timer.Value = 0f;
			agent.Value.SetDestination(selfTransform.Value.position);
			
			return TaskStatus.Success;
		}
	}
}