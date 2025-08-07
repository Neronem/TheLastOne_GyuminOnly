using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Manager.Core;
using _1.Scripts.Manager.Subs;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action
{
	[TaskCategory("Every")]
	[TaskDescription("LightOnWithSound")]
	public class LightOnWithSound : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedTransform targetTransform;
		public SharedVector3 targetPos;
		public SharedBool shouldLookTarget;
		public SharedLight enemyLight;
		public SharedLight allyLight;
		public SharedBaseNpcStatController statController;
		public SharedCollider myCollider;
		public SharedAnimator animator;
		
		public override TaskStatus OnUpdate()
		{
			if (targetTransform.Value == null || targetPos.Value == Vector3.zero)
			{
				shouldLookTarget.Value = false;
				return TaskStatus.Failure;
			}
			
			if (statController.Value.RuntimeStatData.IsAlly)
			{
				if (!allyLight.Value.enabled)
				{
					allyLight.Value.enabled = true;
				}
			}
			else
			{
				if (!enemyLight.Value.enabled)
				{
					enemyLight.Value.enabled = true;
				}
			}
			
			AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);
			if (!stateInfo.IsName(DroneAnimationData.StrafeLeftStr))
			{
				animator.Value.SetTrigger(DroneAnimationData.StrafeLeft);
			}
			
			CoreManager.Instance.soundManager.PlaySFX(SfxType.Drone, myCollider.Value.bounds.center, index:1);
			return TaskStatus.Success;
		}
	}
}