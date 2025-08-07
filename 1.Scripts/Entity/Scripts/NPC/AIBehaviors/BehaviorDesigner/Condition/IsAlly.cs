using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Condition
{
    [TaskCategory("Every")]
    [TaskDescription("IsAlly")]
    public class IsAlly : Conditional
    {
        public SharedBaseNpcStatController statController;
		
        public override TaskStatus OnUpdate()
        {
            return statController.Value.RuntimeStatData.IsAlly ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}