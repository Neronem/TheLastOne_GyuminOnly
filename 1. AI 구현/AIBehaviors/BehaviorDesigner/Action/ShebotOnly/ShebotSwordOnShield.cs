using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Manager.Core;
using _1.Scripts.Manager.Subs;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.ShebotOnly
{
	[TaskCategory("ShebotOnly")]
	[TaskDescription("ShebotSwordOnShield")]
	public class ShebotSwordOnShield : global::BehaviorDesigner.Runtime.Tasks.Action
	{
		public SharedTransform selfTransform;
		public SharedTransform targetTransform;
		public SharedAnimator animator;
		public SharedBool isInterrupted;
		public SharedBool shieldUsedOnce; // 쉴드는 한번밖에 사용하지못함
		public SharedBool hasEnteredShield; // 쉴드를 발동한 단 한번만 Update가 실행하도록 하는 플래그
		public SharedShebot_Shield shield; // 직접 쉴드 끄기위해 참조
		public SharedFloat shieldDuration; // 쉴드 지속시간
		public SharedNavMeshAgent agent;
		
		private float elapsedTime = 0f;
		private bool switchedToStayAnimation = false;
		
		public override void OnStart()
		{
			if (targetTransform.Value != null)
			{
				agent.Value.SetDestination(selfTransform.Value.position);
				elapsedTime = 0f;
				switchedToStayAnimation = false;
				
				if (!shieldUsedOnce.Value)
				{
					shieldUsedOnce.Value = true;
					hasEnteredShield.Value = true;
					CoreManager.Instance.soundManager.PlaySFX(SfxType.Shebot, selfTransform.Value.position, index:5);
					animator.Value.SetTrigger(ShebotAnimationData.Shebot_Guard);
				}
				else
				{
					hasEnteredShield.Value = false;
				}
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (!hasEnteredShield.Value) return TaskStatus.Success;
			if (targetTransform.Value == null) return TaskStatus.Failure;
			
			if (isInterrupted.Value)
			{
				isInterrupted.Value = false;
				shield.Value.DisableShieldAnimaton();
				return TaskStatus.Success;
			}

			elapsedTime += Time.deltaTime;
			
			if (!switchedToStayAnimation)
			{
				AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);
				if (stateInfo.IsName(ShebotAnimationData.Shebot_GuardStr) && stateInfo.normalizedTime >= 1f)
				{
					animator.Value.SetTrigger(ShebotAnimationData.Shebot_Guard_Stay);
					switchedToStayAnimation = true;
				}
			}

			if (elapsedTime >= shieldDuration.Value)
			{
				shield.Value.DisableShieldAnimaton();
				return TaskStatus.Success;
			}
			
			if (targetTransform.Value != null)
			{
				NpcUtil.LookAtTarget(selfTransform.Value, targetTransform.Value.position, additionalYangle: 55);
			}
			
			return TaskStatus.Running;
		}
	}
}