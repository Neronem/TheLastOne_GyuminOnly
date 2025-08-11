using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using _1.Scripts.Static;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.ShebotOnly
{
    [TaskCategory("ShebotOnly")]
    [TaskDescription("ShebotSniperAiming")]
    public class ShebotSniperAiming : global::BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedTransform targetTransform;
        public SharedTransform selfTransform;
        public SharedTransform muzzleTransform;
        public SharedTransform rifleTransform;
        public SharedVector3 targetPos;
        public SharedBool isAlerted;
        public SharedBaseNpcStatController statController;
        public SharedLineRenderer lineRenderer;
        public SharedFloat aimingDuration;
        
        private float timer;
        private Collider targetHead;
        
        public override void OnStart()
        {
            timer = 0f;
            int layer = statController.Value.RuntimeStatData.IsAlly ? LayerConstants.Head_E : LayerConstants.Head_P;
            if (targetTransform.Value != null) targetHead = NpcUtil.FindColliderOfLayerInChildren(targetTransform.Value.gameObject, layer);
        }

        public override TaskStatus OnUpdate()
        {
            if (timer >= aimingDuration.Value) return TaskStatus.Success;
            timer += Time.deltaTime;
            
            if (targetTransform.Value == null) // 예외처리 1 : 타겟이 사라졌을 시
            {
                targetPos.Value = Vector3.zero;
                targetTransform.Value = null;
                isAlerted.Value = false;

                return TaskStatus.Failure;
            }
			
            if (!targetTransform.Value.CompareTag("Player")) // 예외처리 2 : 타겟이 사망했을 시 & 해킹당하는 중일때 & 지정했던 타겟이 아군이 됐을 때
            {
                var stat = targetTransform.Value.GetComponent<BaseNpcStatController>();
                if (stat == null || stat.IsDead || stat.isHacking || stat.RuntimeStatData.IsAlly == statController.Value.RuntimeStatData.IsAlly)
                {
                    targetPos.Value = Vector3.zero;
                    targetTransform.Value = null;
                    isAlerted.Value = false;
                    
                    return TaskStatus.Failure;
                }
            }
            
            targetPos.Value = targetHead.bounds.center;
            
            int layerMask = statController.Value.RuntimeStatData.IsAlly ? LayerConstants.EnemyLayerMask : LayerConstants.AllyLayerMask;
            int finallayerMask = LayerConstants.DefaultHittableLayerMask | layerMask;
            Vector3 direction = (targetPos.Value - muzzleTransform.Value.position).normalized;
            
            Physics.Raycast(muzzleTransform.Value.position, direction, out RaycastHit hit, 1000, finallayerMask);
            
            NpcUtil.DrawLineBetweenPoints(lineRenderer.Value, muzzleTransform.Value.position, hit.point);
            NpcUtil.LookAtTarget(selfTransform.Value, targetPos.Value, additionalYangle:45);
            NpcUtil.LookAtTarget(rifleTransform.Value, targetPos.Value, pinY:false);
            
            return TaskStatus.Running;
        }
    }
}