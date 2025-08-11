using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.DroneOnly
{
    [TaskCategory("DroneOnly")]
    [TaskDescription("SuicideDroneResetAll")]
    public class SuicideDroneResetAll : global::BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedTransform selfTransform;
        public SharedTransform targetTransform;
        public SharedVector3 targetPos;
        public SharedBool shouldLookTarget;
        public SharedBool isAlerted;
        public SharedFloat timer;
        public SharedNavMeshAgent agent;
		public SharedAnimator animator;
        public SharedLight enemyLight;
        public SharedLight allyLight;
        
        public override TaskStatus OnUpdate()
        {
            targetTransform.Value = null;
            targetPos.Value = Vector3.zero;
            shouldLookTarget.Value = false;
            timer.Value = 0f;
            agent.Value.SetDestination(selfTransform.Value.position);
			
            AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName(DroneAnimationData.Hit1Str) &&
                !stateInfo.IsName(DroneAnimationData.Hit2Str) &&
                !stateInfo.IsName(DroneAnimationData.Hit3Str) &&
                !stateInfo.IsName(DroneAnimationData.Hit4Str) &&
                !stateInfo.IsName(DroneAnimationData.RepairStr))
            {
                animator.Value.SetTrigger(DroneAnimationData.Repair);
            }
            
            enemyLight.Value.enabled = false;
            allyLight.Value.enabled = false;
            isAlerted.Value = false;
            
            return TaskStatus.Success;
        }
    }
}