using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables;
using _1.Scripts.Interfaces.Common;
using _1.Scripts.Interfaces.NPC;
using _1.Scripts.Manager.Core;
using _1.Scripts.Manager.Subs;
using _1.Scripts.Static;
using _1.Scripts.Util;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cysharp.Threading.Tasks;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.Action.DroneOnly
{
    [TaskCategory("DroneOnly")]
    [TaskDescription("SuicideDroneAttacking")]
    public class SuicideDroneAttacking : global::BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedTransform selfTransform;
        public SharedBaseNpcStatController statController;
        public SharedParticleSystem explosionParticle;
        public SharedCollider myCollider;
        public SharedNavMeshAgent agent;
        public SharedBool isDead;
        
        private bool isExploded = false;
        private CancellationTokenSource destroyCts;
        
        public override TaskStatus OnUpdate()
        {
            if (isExploded) // 이미 폭발했으면 return
            {
                return TaskStatus.Running;
            }
            
            if (!statController.Value.TryGetRuntimeStatInterface<IBoomable>(out var boomable))
            {
                return TaskStatus.Failure;
            }
            
            // 1. 파티클 생성 및 사운드 출력
            explosionParticle.Value.Play();
            CoreManager.Instance.soundManager.PlaySFX(SfxType.Drone, selfTransform.Value.position, index:4);
                
            // 2. 데미지 주기
            bool isAlly = statController.Value.RuntimeStatData.IsAlly;
            Vector3 selfPos = selfTransform.Value.position;
            float range = boomable.BoomRange;
            
            int layerMask = isAlly ? 1 << LayerConstants.Chest_E : 1 << LayerConstants.Chest_P;
            Collider[] colliders = Physics.OverlapSphere(selfPos, range, layerMask);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject == selfTransform.Value.gameObject)
                {
                    continue;   
                }
                
                if (collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.OnTakeDamage(statController.Value.RuntimeStatData.BaseDamage);
                }
            }

            // 3. 드론 렌더러 전부 끄기
            foreach (var rend in selfTransform.Value.GetComponentsInChildren<Renderer>())
            {
                if (rend.gameObject.name.Contains("Explosion"))
                {
                    continue;
                }
                rend.enabled = false;
            }
            
            // 4. 드론 콜라이더 비활성화
            myCollider.Value.enabled = false;
            
            // 5. isDead = true
            isDead.Value = true;
            statController.Value.Dead();
            
            // 6. 기다린 후 파괴
            destroyCts = NpcUtil.CreateLinkedNpcToken();
            _ = DelayedDestroy(selfTransform.Value.gameObject, destroyCts);
            
            // 7. 기존경로 비활성화
            agent.Value.SetDestination(selfTransform.Value.position);
            
            isExploded = true;
            return TaskStatus.Running;
        }
        
        private async UniTaskVoid DelayedDestroy(GameObject destroyTarget, CancellationTokenSource token)
        {
            try
            {
                await UniTask.WaitForSeconds(2.5f, cancellationToken: token.Token, cancelImmediately: true);
            }
            finally
            {
                NpcUtil.DisableNpc(destroyTarget);
                destroyCts?.Dispose();
                destroyCts = null;
            }
        }
    }
}