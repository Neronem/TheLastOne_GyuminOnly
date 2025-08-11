using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Interfaces.NPC;
using _1.Scripts.Static;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
	[TaskCategory("Every")]
	[TaskDescription("IsTargetInDetectRange")]
	public class IsTargetInDetectRange : Conditional
	{
		public SharedTransform selfTransform;
		public SharedTransform selfTransform_Head;
		public SharedTransform targetTransform;
		public SharedVector3 targetPos;
		public SharedFloat maxViewDistance;
		public SharedBool shouldLookTarget;
		public SharedBaseNpcStatController statController;

		public override TaskStatus OnUpdate()
		{
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

				int layer = ally ? LayerConstants.Chest_E : LayerConstants.Chest_P;
				Collider targetCol = NpcUtil.FindColliderOfLayerInChildren(collider.gameObject, layer);
				Vector3 colliderPos = targetCol.bounds.center;

				if (NpcUtil.IsTargetVisible(selfTransform_Head.Value.position, colliderPos, maxViewDistance.Value, ally))
				{
					targetTransform.Value = collider.transform;
					targetPos.Value = colliderPos;
					shouldLookTarget.Value = true;
					return TaskStatus.Success;
				}
			}
			
			return TaskStatus.Failure;
		}	
	}
}