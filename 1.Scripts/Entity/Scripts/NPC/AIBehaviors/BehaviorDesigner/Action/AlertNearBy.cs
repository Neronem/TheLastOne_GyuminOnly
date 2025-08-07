using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Interfaces.NPC;
using _1.Scripts.Manager.Core;
using _1.Scripts.Manager.Subs;
using _1.Scripts.Static;
using _1.Scripts.UI.InGame.Dialogue;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.Localization;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action
{	
	[TaskCategory("Every")]
	[TaskDescription("AlertNearBy")]
	public class AlertNearBy : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedTransform selfTransform;
		public SharedTransform targetTransform;
		public SharedVector3 targetPos;
		
		public SharedCollider myCollider;
		public SharedBaseNpcStatController statController;
		public SharedLight enemylight;
		public SharedLight allyLight;
		public SharedBool isAlerted;
		public SharedBool shouldAlertNearBy;

		public bool isDog;
		
		public override TaskStatus OnUpdate()
		{
			shouldAlertNearBy.Value = false;
			if (isAlerted.Value) return TaskStatus.Success;
			if (targetTransform.Value == null) return TaskStatus.Success;
			
			if (!statController.Value.TryGetRuntimeStatInterface<IAlertable>(out var alertable)) // 있을 시 변환
			{
				return TaskStatus.Failure;
			}
			
			(statController.Value.RuntimeStatData.IsAlly ? allyLight.Value : enemylight.Value).enabled = true; // 경고등 On
			if (isDog) CoreManager.Instance.soundManager.PlaySFX(SfxType.Dog, myCollider.Value.bounds.center, index:0); // 사운드 출력
			else CoreManager.Instance.soundManager.PlaySFX(SfxType.Drone, myCollider.Value.bounds.center, index:1); 
			isAlerted.Value = true;

			int droneAlertKey = 13;
			CoreManager.Instance.dialogueManager.TriggerDialogue(droneAlertKey);
			
			bool isAlly = statController.Value.RuntimeStatData.IsAlly; 
			Vector3 selfPos = selfTransform.Value.position;
			float range = alertable.AlertRadius;

			int layerMask = isAlly ? 1 << LayerConstants.Ally : 1 << LayerConstants.Enemy;
			Collider[] colliders = Physics.OverlapSphere(selfPos, range, layerMask); // 주변 콜라이더 모음

			foreach (var col in colliders)
			{
				if (col == myCollider.Value)
				{
					continue;
				}
				
				var BT = col.GetComponent<global::BehaviorDesigner.Runtime.BehaviorTree>();
				if (BT != null)
				{
					BT.SetVariableValue(BehaviorNames.TargetTransform, targetTransform.Value);
					BT.SetVariableValue(BehaviorNames.TargetPos, targetPos.Value);
					BT.SetVariableValue(BehaviorNames.IsAlerted, true);
				}
			}
			
			return TaskStatus.Success;
		}
	}
}