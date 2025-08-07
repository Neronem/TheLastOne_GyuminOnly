using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Interfaces.NPC;
using _1.Scripts.Manager.Core;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action
{
	[TaskCategory("Every")]
	[TaskDescription("SetDestinationToPatrolInPatrolZone")]
	public class SetDestinationToPatrolInPatrolZone : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedTransform selfTransform;
		public SharedBaseNpcStatController statController;
		public SharedNavMeshAgent agent;
		
		public SharedLight enemyLight;
		public SharedLight allyLight;
		public SharedFloat stoppingDistance;
		public SharedBool isAlertedOnce;
		
		private Collider patrolZoneCollider;
		
		public override void OnStart()
		{
			agent.Value.speed = statController.Value.RuntimeStatData.MoveSpeed;
			agent.Value.isStopped = false;
			enemyLight.Value.enabled = false;
			allyLight.Value.enabled = false;

			if (patrolZoneCollider == null)
			{
				patrolZoneCollider = GameObject.FindGameObjectWithTag("PatrolZone").GetComponent<Collider>();
			}
		}

		public override TaskStatus OnUpdate()
		{
			Vector3 targetPosition;
			if (statController.Value.RuntimeStatData.IsAlly)
			{
				targetPosition = GetPlayerPosition();
			}
			else
			{ 
				targetPosition = GetWanderLocation();
			}
			
			agent.Value.SetDestination(targetPosition);
			return TaskStatus.Success;
		}
		
		private Vector3 GetWanderLocation()
		{
			NavMeshHit hit = new NavMeshHit();
			hit.position = selfTransform.Value.position;
			int i = 0;

			statController.Value.TryGetRuntimeStatInterface<IPatrolable>(out var patrolable);
			statController.Value.TryGetRuntimeStatInterface<IDetectable>(out var detectable);


			while (i < 30)
			{
				Vector3 randomPos = selfTransform.Value.position + (Random.insideUnitSphere * Random.Range(patrolable.MinWanderingDistance, patrolable.MaxWanderingDistance));
				randomPos.y = selfTransform.Value.position.y;
				
				if (NavMesh.SamplePosition(randomPos, out hit, 2f, NavMesh.AllAreas))
				{
					// PatrolZone 안에 있는지 확인
					if (!isAlertedOnce.Value)
					{
						if (patrolZoneCollider != null && patrolZoneCollider.bounds.Contains(hit.position))
						{
							break;
						}
					}
					else
					{
						break;
					}
				}
				
				i++;
			}
        
			return hit.position;
		}

		private Vector3 GetPlayerPosition()
		{
			agent.Value.speed = statController.Value.RuntimeStatData.MoveSpeed + statController.Value.RuntimeStatData.RunMultiplier;
				
			Vector3 directionToPlayer = (CoreManager.Instance.gameManager.Player.transform.position - selfTransform.Value.position).normalized;
			Vector3 targetSpot = CoreManager.Instance.gameManager.Player.transform.position - directionToPlayer * stoppingDistance.Value;
				
			if (NavMesh.SamplePosition(targetSpot, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
			{
				return hit.position;
			}

			return selfTransform.Value.position;
		}
	}
}