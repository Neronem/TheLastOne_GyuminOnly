using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
    [TaskCategory("Every")]
    [TaskDescription("IsTargetNull")]
    public class IsTargetNull : Conditional
    {
        public SharedTransform targetTransform;
		
        public override TaskStatus OnUpdate()
        {
            if (targetTransform.Value == null) return TaskStatus.Success;
            
            return TaskStatus.Failure;
        }
    }
}