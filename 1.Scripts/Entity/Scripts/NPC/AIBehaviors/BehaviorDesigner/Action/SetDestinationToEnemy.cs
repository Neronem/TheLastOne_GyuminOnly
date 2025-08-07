using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action
{
	[TaskCategory("Every")]
	[TaskDescription("SetDestinationToEnemy")]
	public class SetDestinationToEnemy : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedTransform selfTransform;
		public SharedTransform targetTransform;
		public SharedVector3 targetPos;
		public SharedNavMeshAgent navMeshAgent;
		public SharedBaseNpcStatController statController;
		public SharedBool shouldLookTarget;
		public SharedFloat stoppingDistance;
		public bool isShebot_Rifle;
		
		public override TaskStatus OnUpdate()
		{
			if (targetTransform.Value == null || targetPos.Value == Vector3.zero)
			{
				shouldLookTarget.Value = false;
				return TaskStatus.Failure;
			}

			if (!targetTransform.Value.CompareTag("Player"))
			{
				var statCtrl = targetTransform.Value.GetComponent<BaseNpcStatController>();
				if (statCtrl == null || statCtrl.IsDead)
				{
					return TaskStatus.Failure;
				}
			}
			
			navMeshAgent.Value.speed = statController.Value.RuntimeStatData.MoveSpeed + statController.Value.RuntimeStatData.RunMultiplier;
			shouldLookTarget.Value = true;
			
			Vector3 directionToEnemy = (targetTransform.Value.position - selfTransform.Value.position).normalized;
			Vector3 targetSpot = targetTransform.Value.position - directionToEnemy * stoppingDistance.Value;
			
			if (NavMesh.SamplePosition(targetSpot, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
			{
				navMeshAgent.Value.SetDestination(hit.position);
			}

			if (isShebot_Rifle) NpcUtil.LookAtTarget(selfTransform.Value, targetTransform.Value.position, additionalYangle:45);
			else NpcUtil.LookAtTarget(selfTransform.Value, targetTransform.Value.position);
			
			return TaskStatus.Success;
		}
	}
}