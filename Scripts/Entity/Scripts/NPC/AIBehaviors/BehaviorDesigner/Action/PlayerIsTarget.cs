using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Manager.Core;
using _1.Scripts.Static;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action
{
	[TaskCategory("Every")]
	[TaskDescription("PlayerIsTarget")]
	public class PlayerIsTarget : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedTransform targetTransform;
		public SharedVector3 targetPosition;
		public SharedCollider playerCollider;
		public SharedBool shouldLookTarget;
		public SharedBaseNpcStatController statController;
		public bool isSniper;
		
		public override void OnStart()
		{
			if (playerCollider.Value == null)
			{
				if (!statController.Value.RuntimeStatData.IsAlly)
				{
					int layer = isSniper ? LayerConstants.Head_P : LayerConstants.Chest_P;
					playerCollider.Value = NpcUtil.FindColliderOfLayerInChildren(CoreManager.Instance.gameManager.Player.gameObject, layer);
				}
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (statController.Value.RuntimeStatData.IsAlly) return TaskStatus.Success;
			
			targetTransform.Value = CoreManager.Instance.gameManager.Player.transform;
			targetPosition.Value = playerCollider.Value.bounds.center;
			shouldLookTarget.Value = true;
			
			return TaskStatus.Success;
		}
	}
}