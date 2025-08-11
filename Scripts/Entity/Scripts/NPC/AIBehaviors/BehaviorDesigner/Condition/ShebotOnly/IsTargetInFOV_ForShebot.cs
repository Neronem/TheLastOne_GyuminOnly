using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition.ShebotOnly
{
    [TaskCategory("Every")]
    [TaskDescription("IsTargetInFOV_ForShebot")]
    public class IsTargetInFOV_ForShebot : Conditional
    {
        public SharedTransform selfTransform_Head;
        public SharedVector3 targetPos;
        public SharedFloat maxViewDistance;
        public SharedBaseNpcStatController statController;
		
        public override TaskStatus OnUpdate()
        {
            if (NpcUtil.IsTargetVisible(selfTransform_Head.Value.position, targetPos.Value, maxViewDistance.Value, statController.Value.RuntimeStatData.IsAlly))
            {
                return TaskStatus.Success;
            }
			
            return TaskStatus.Failure;
        }
    }
}