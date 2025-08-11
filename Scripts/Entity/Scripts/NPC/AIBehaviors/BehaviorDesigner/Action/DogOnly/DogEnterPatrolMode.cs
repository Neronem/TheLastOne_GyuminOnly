using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.DogOnly
{
    [TaskCategory("DogOnly")]
    [TaskDescription("DogEnterPatrolMode")]
    public class DogEnterPatrolMode : global::BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedBool shouldLookTarget;
        public SharedAnimator animator;
        public SharedBaseNpcStatController statController;
        
        public override TaskStatus OnUpdate()
        {
            if (!statController.Value.RuntimeStatData.IsAlly)
            {
                AnimatorStateInfo stateInfo = animator.Value.GetCurrentAnimatorStateInfo(0);
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
			
            shouldLookTarget.Value = false; // 보통 노드구조 맨 끝자락에 배회할때 쓰니까 추가
			
            return TaskStatus.Success;
        }
    }
}