using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _1.Scripts.Interfaces.Common;
using _1.Scripts.Manager.Core;
using _1.Scripts.Manager.Subs;
using _1.Scripts.Static;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace _1.Scripts.Util
{
    /// <summary>
    /// AI관련 헬퍼 모음집
    /// </summary>
    public static class NpcUtil
    {
        /// <summary>
        /// NPC 입장에서 지정된 타겟이 시야에 있는지 검사
        /// </summary>
        /// <param name="origin">NPC 위치</param>
        /// <param name="targetPosition">타겟 위치</param>
        /// <param name="maxViewDistance">최대 시야 거리</param>
        /// <param name="isAlly">자신이 아군이면 true, 적군이면 false</param>
        /// <returns>시야에 타겟이 있고, 올바른 레이어인지 여부</returns>
        public static bool IsTargetVisible(Vector3 origin, Vector3 targetPosition, float maxViewDistance,
            bool isAlly)
        {
            Vector3 direction = (targetPosition - origin).normalized;

            int layerMask = isAlly ? LayerConstants.EnemyLayerMask : LayerConstants.AllyLayerMask;
            int finallayerMask = LayerConstants.DefaultHittableLayerMask | layerMask;
            
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxViewDistance, finallayerMask))
            {
                int targetLayer = hit.collider.gameObject.layer;
                
                return isAlly
                    ? (LayerConstants.EnemyLayerMask & LayerConstants.ToLayerMask(targetLayer)) != 0   
                    : (LayerConstants.AllyLayerMask & LayerConstants.ToLayerMask(targetLayer)) != 0;
            }

            return false;
        }

        /// <summary>
        /// 타겟 바라보게 설정하는 메소드
        /// </summary>
        /// <param name="self"></param>
        /// <param name="target"></param>
        /// <param name="turnSpeed"></param>
        /// <param name="additionalYangle"></param>
        public static void LookAtTarget(Transform self, Vector3 target, float turnSpeed = 15f, float additionalYangle = 0f, bool pinY = true)
        {
            if (target == Vector3.zero || self == null) return;

            Vector3 dir = target - self.position;
            if (pinY) dir.y = 0; // 위/아래 회전 고정

            if (dir == Vector3.zero) return;

            Quaternion rot = Quaternion.LookRotation(dir);
            Quaternion additionalRotation = Quaternion.Euler(0, additionalYangle, 0);
            self.rotation = Quaternion.Slerp(self.rotation, rot * additionalRotation, Time.deltaTime * turnSpeed);
        }

        /// <summary>
        /// 두 지점 사이간에 라인렌더러 표시
        /// </summary>
        /// <param name="lineRenderer"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void DrawLineBetweenPoints(LineRenderer lineRenderer, Vector3 start, Vector3 end)
        {
            if (lineRenderer == null) return;
            if (!lineRenderer.enabled) lineRenderer.enabled = true;
            
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        /// <summary>
        /// 적에게 사격, 사거리 임의 조정 가능
        /// </summary>
        /// <param name="muzzlePosition"></param>
        /// <param name="direction"></param>
        /// <param name="isAlly"></param>
        /// <param name="damage"></param>
        /// <param name="maxDistance"></param>
        public static void FireToTarget(Vector3 muzzlePosition, Vector3 direction, bool isAlly, int damage, float maxDistance = 100f)
        {
            int targetMask = isAlly ? LayerConstants.EnemyLayerMask : LayerConstants.AllyLayerMask;
            int finalMask = LayerConstants.DefaultHittableLayerMask | targetMask;
            
            if (Physics.Raycast(muzzlePosition, direction, out RaycastHit hit, maxDistance, finalMask))
            {
                if (hit.collider.TryGetComponent(out IDamagable damagable))
                {
                    damagable.OnTakeDamage(damage);
                }
            }
        }
        
        /// <summary>
        /// 자식 레이어 전부 변환, 변환을 무시해야하는 레이어가 있다면 추가, 본인의 레이어도 포함시킬거라면 bool값 true
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layer"></param>
        /// <param name="ignoreLayerMask"></param>
        /// <param name="includeSelf"></param>
        public static void SetLayerRecursively(GameObject obj, int layer, int ignoreLayerMask = 0, bool includeSelf = true)
        {
            if (includeSelf && (ignoreLayerMask == 0 || (ignoreLayerMask & LayerConstants.ToLayerMask(obj.layer)) == 0))
            {
                obj.layer = layer;
            }
            
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer, ignoreLayerMask, true);
            }
        }

        /// <summary>
        /// 해킹 시 모든 레이어 아군레이어로 변환
        /// </summary>
        /// <param name="obj"></param>
        public static void SetLayerRecursively_Hacking(GameObject obj)
        {
            if (obj.layer == LayerConstants.Enemy) obj.layer = LayerConstants.Ally;
            else if (obj.layer == LayerConstants.StencilEnemy) obj.layer = LayerConstants.StencilAlly;
            else if (obj.layer == LayerConstants.Head_E) obj.layer = LayerConstants.Head_P;
            else if (obj.layer == LayerConstants.Chest_E) obj.layer = LayerConstants.Chest_P;
            else if (obj.layer == LayerConstants.Arm_E) obj.layer = LayerConstants.Arm_P;
            else if (obj.layer == LayerConstants.Belly_E) obj.layer = LayerConstants.Belly_P;
            else if (obj.layer == LayerConstants.Leg_E) obj.layer = LayerConstants.Leg_P;
            
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively_Hacking(child.gameObject);
            }
        }

        /// <summary>
        /// 오브젝트의 자식들 중 원하는 레이어의 콜라이더를 반환 (첫번째 대상만)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static Collider FindColliderOfLayerInChildren(GameObject obj, int layer)
        {
            foreach (Transform child in obj.GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.layer == layer)
                {
                    return child.gameObject.GetComponent<Collider>();
                }
            }
            return null;
        }
        
        /// <summary>
        /// Npc를 씬에서 사라지게 할 때 사용
        /// </summary>
        /// <param name="targetObj"></param>
        public static void DisableNpc(GameObject targetObj)
        {
            CoreManager.Instance.spawnManager.RemoveMeFromSpawnedEnemies(targetObj);
            CoreManager.Instance.objectPoolManager.Release(targetObj);
        }
        
        /// <summary>
        /// NpcCTS 토큰과 연결되는 토큰 생성
        /// </summary>
        /// <returns></returns>
        public static CancellationTokenSource CreateLinkedNpcToken()
        {
            return CancellationTokenSource.CreateLinkedTokenSource(CoreManager.Instance.NpcCTS.Token);
        }
    }
}
