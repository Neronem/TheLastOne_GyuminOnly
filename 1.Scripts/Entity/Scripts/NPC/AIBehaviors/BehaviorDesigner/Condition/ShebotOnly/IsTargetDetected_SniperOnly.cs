using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Interfaces.NPC;
using _1.Scripts.Static;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition.ShebotOnly
{
	[TaskCategory("ShebotOnly")]
	[TaskDescription("IsTargetDetected_SniperOnly")]
	public class IsTargetDetected_SniperOnly : Conditional
	{
		public SharedTransform selfTransform;
		public SharedTransform targetTransform;
		public SharedTransform muzzleTransform;
		public SharedVector3 targetPos;
		public SharedFloat maxViewDistance;
		public SharedBool shouldLookTarget;
		public SharedBool isShouldAlertNearBy;
		public SharedBaseNpcStatController statController;
		public SharedDetectionGizmo detectionGizmo;
		public SharedQuaternion baseRotation;
		public SharedAnimator animator;
		
		private float rotationSpeed = 15f;
		private float centerAngle = 0f;
		private float rotationRange = 45f;

		public override void OnStart()
		{
			baseRotation.Value = selfTransform.Value.rotation;
			
			AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);
			if (!stateInfo.IsName(ShebotAnimationData.Shebot_Rifle_AimStr)) animator.Value.SetTrigger(ShebotAnimationData.Shebot_Rifle_Aim);
		}

		public override TaskStatus OnUpdate()
		{
			if (isShouldAlertNearBy.Value) return TaskStatus.Failure;
			
			bool ally = statController.Value.RuntimeStatData.IsAlly;
			Vector3 selfPos = selfTransform.Value.position;
			float range = 0f;
			if (statController.Value.TryGetRuntimeStatInterface<IDetectable>(out var detectable))
			{
				range = detectable.DetectRange;
			}
			else
			{
				return TaskStatus.Failure;
			}

			int layerMask = ally ? (1 << LayerConstants.Enemy) : (1 << LayerConstants.Ally);

			Collider[] colliders = Physics.OverlapSphere(selfPos, range, layerMask);
			detectionGizmo.Value.range = range;
			
			System.Array.Sort(colliders, (a, b) =>
			{
				float distA = (a.bounds.center - selfPos).sqrMagnitude;
				float distB = (b.bounds.center - selfPos).sqrMagnitude;
				return distA.CompareTo(distB);
			});
			
			foreach (var collider in colliders)
			{
				if (!collider.CompareTag("Player"))
				{
					var otherStatController = collider.GetComponent<BaseNpcStatController>();
					if (otherStatController == null || otherStatController.IsDead || otherStatController.isHacking)
					{
						continue;
					}
				}	

				int layer = ally ? LayerConstants.Head_E : LayerConstants.Head_P;
				Collider targetCol = NpcUtil.FindColliderOfLayerInChildren(collider.gameObject, layer);
				Vector3 colliderPos = targetCol.bounds.center;

				if (NpcUtil.IsTargetVisible(muzzleTransform.Value.position, colliderPos, maxViewDistance.Value, ally))
				{
					targetTransform.Value = collider.transform;
					targetPos.Value = colliderPos;
					shouldLookTarget.Value = true;
					return TaskStatus.Success;
				}
			}
			
			// 정찰모션
			float angle = Mathf.PingPong(Time.time * rotationSpeed, rotationRange * 2) - rotationRange;
			Quaternion offset = Quaternion.Euler(0f, angle, 0f);
			selfTransform.Value.rotation = baseRotation.Value * offset;
			
			return TaskStatus.Running;
		}
	}
}