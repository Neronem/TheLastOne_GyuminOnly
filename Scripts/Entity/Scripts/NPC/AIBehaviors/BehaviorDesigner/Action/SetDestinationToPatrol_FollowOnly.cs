using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Interfaces.NPC;
using _1.Scripts.Manager.Core;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action
{
	[TaskCategory("Every")]
	[TaskDescription("SetDestinationToPatrol_FollowOnly")]
	public class SetDestinationToPatrol_FollowOnly : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedTransform selfTransform;
		public SharedTransform leader;
		public SharedBaseNpcStatController statController;
		public SharedNavMeshAgent agent;
		public SharedAnimator animator;
		public bool isShebot;
		public bool isShebot_Rifle;
		public bool isDog;
		
		public SharedLight enemyLight;
		public SharedLight allyLight;
		public SharedFloat stoppingDistance;

		public override void OnStart()
		{
			agent.Value.speed = statController.Value.RuntimeStatData.IsAlly
				? statController.Value.RuntimeStatData.MoveSpeed + statController.Value.RuntimeStatData.RunMultiplier
				: statController.Value.RuntimeStatData.MoveSpeed;
			agent.Value.isStopped = false;
			enemyLight.Value.enabled = false;
			allyLight.Value.enabled = false;
		}

		public override TaskStatus OnUpdate()
		{
			Vector3 targetPosition;
			
			if (statController.Value.RuntimeStatData.IsAlly) // 1. 본인이 아군이라면
			{
				targetPosition = GetPlayerPosition();
			}
			else if (leader.Value == null) // 2. 리더가 없다면
			{ 
				targetPosition = GetWanderLocation();
			}
			else if (leader.Value.TryGetComponent(out BaseNpcStatController stat)) // 3. 리더가 있지만 그 리더가 적일 경우
			{
				if (stat.RuntimeStatData.IsAlly != statController.Value.RuntimeStatData.IsAlly)
				{
					targetPosition = GetWanderLocation();
				}
				else
				{
					targetPosition = GetLeaderPosition();
				}
			}
			else // 4. 그 외의 경우
			{
				targetPosition = GetLeaderPosition();
			}
			
			agent.Value.SetDestination(targetPosition);

			#region ShebotOnly
			
			if (isShebot)
			{
				if (statController.Value.RuntimeStatData.IsAlly || leader.Value != null)
				{
					AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);
					
					if (Vector3.Distance(selfTransform.Value.position, targetPosition) <= 0.1)
					{
						if (isShebot_Rifle && !stateInfo.IsName(ShebotAnimationData.Shebot_Rifle_idleStr))
						{
							animator.Value.SetTrigger(ShebotAnimationData.Shebot_Rifle_idle);
						}
						else if (!isShebot_Rifle && !stateInfo.IsName(ShebotAnimationData.Shebot_IdleStr))
						{
							animator.Value.SetTrigger(ShebotAnimationData.Shebot_Idle);
						}
					}
					else if (!stateInfo.IsName(ShebotAnimationData.Shebot_WalkStr))
					{ 
						animator.Value.SetTrigger(ShebotAnimationData.Shebot_Walk);
					}
				}
				else
				{
					animator.Value.SetTrigger(ShebotAnimationData.Shebot_Walk);
				}
			}
			#endregion
			
			#region DogOnly

			if (isDog)
			{
				if (statController.Value.RuntimeStatData.IsAlly || leader.Value != null)
				{
					AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);
				
					if (Vector3.Distance(selfTransform.Value.position, targetPosition) <= 0.1)
					{
						if (!stateInfo.IsName(DogAnimationData.Dog_Idle1Str) && !stateInfo.IsName(DogAnimationData.Dog_Idle2Str) && 
						    !stateInfo.IsName(DogAnimationData.Dog_Idle3Str) && !stateInfo.IsName(DogAnimationData.Dog_Idle4Str))
						{
							int[] idleHashes =
							{
								DogAnimationData.Dog_Idle1,
								DogAnimationData.Dog_Idle2,
								DogAnimationData.Dog_Idle3,
								DogAnimationData.Dog_Idle4,
							};
							animator.Value.SetTrigger(idleHashes[UnityEngine.Random.Range(0, idleHashes.Length)]);
						}
					}
					else if (!stateInfo.IsName(DogAnimationData.Dog_WalkStr))
					{
						animator.Value.SetTrigger(DogAnimationData.Dog_Walk);
					}
				}
				else
				{
					animator.Value.SetTrigger(DogAnimationData.Dog_Walk);
				}
			}

			#endregion
			
			return TaskStatus.Success;
		}
		
		private Vector3 GetWanderLocation()
		{
			NavMeshHit hit;
			int i = 0;

			statController.Value.TryGetRuntimeStatInterface<IPatrolable>(out var patrolable);
			statController.Value.TryGetRuntimeStatInterface<IDetectable>(out var detectable);
            
			do
			{
				NavMesh.SamplePosition(selfTransform.Value.position + (Random.onUnitSphere * Random.Range(patrolable.MinWanderingDistance, patrolable.MaxWanderingDistance)), out hit, patrolable.MaxWanderingDistance, NavMesh.AllAreas);
				i++;
				if (i == 30) break;
			} 
			while (Vector3.Distance(selfTransform.Value.position, hit.position) < detectable.DetectRange);
        
			return hit.position;
		}

		private Vector3 GetLeaderPosition()
		{
			Vector3 directionToLeader = (leader.Value.position - selfTransform.Value.position).normalized;
			Vector3 targetSpot = leader.Value.position - directionToLeader * stoppingDistance.Value;
			
			if (NavMesh.SamplePosition(targetSpot, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
			{
				return hit.position;
			}

			return selfTransform.Value.position;
		} 
		
		private Vector3 GetPlayerPosition()
		{
			Vector3 directionToPlayer = (CoreManager.Instance.gameManager.Player.transform.position - selfTransform.Value.position).normalized;
			Vector3 targetSpot = CoreManager.Instance.gameManager.Player.transform.position - directionToPlayer * (stoppingDistance.Value + 1.5f);
				
			if (NavMesh.SamplePosition(targetSpot, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
			{
				return hit.position;
			}

			return selfTransform.Value.position;
		}
	}
}