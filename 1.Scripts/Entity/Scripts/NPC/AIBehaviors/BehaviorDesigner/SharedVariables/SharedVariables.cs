using System.Collections;
using System.Collections.Generic;
using _1.Scripts.Entity.Scripts.NPC.Gizmo;
using _1.Scripts.Entity.Scripts.NPC.Shebot_Weapon;
using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace _1.Scripts.Entity.Scripts.NPC.AIBehaviors.BehaviorDesigner.SharedVariables
{
    // 여기있는 클래스들은 전부 비헤이비어 디자이너의 Shared 변수 추가지정을 위해 존재
    
    [System.Serializable] // 이걸 정의해야 인스펙터에 보임
    public class SharedNavMeshAgent : SharedVariable<NavMeshAgent>
    {
        // static implicit operator = 암시적 형변환
        // 이 생성자는 지워도 되지만, sharedNavMeshAgent = GetComponent<NavMeshAgent>(); 이런게 가능해짐
        // 만약 지운다면 sharedNavMeshAgent = new SharedNavMeshAgent { Value = GetComponent<NavMeshAgent>() }; 이렇게 하면 됨
        public static implicit operator SharedNavMeshAgent(NavMeshAgent value)         
        {
            return new SharedNavMeshAgent { Value = value };
        }
    }
    
    [System.Serializable]
    public class SharedAnimator : SharedVariable<Animator>
    {
        public static implicit operator SharedAnimator(Animator value)
        {
            return new SharedAnimator { Value = value };
        }
    }

    [System.Serializable]
    public class SharedBaseNpcStatController : SharedVariable<BaseNpcStatController>
    {
        public static implicit operator SharedBaseNpcStatController(BaseNpcStatController value)
        {
            return new SharedBaseNpcStatController { Value = value };
        }
    }

    [System.Serializable]
    public class SharedLight : SharedVariable<Light>
    {
        public static implicit operator SharedLight(Light value)
        {
            return new SharedLight { Value = value };
        }
    }
    
    [System.Serializable]
    public class SharedParticleSystem : SharedVariable<ParticleSystem>
    {
        public static implicit operator SharedParticleSystem(ParticleSystem value)
        {
            return new SharedParticleSystem { Value = value };
        }
    }
    
    [System.Serializable]
    public class SharedShebot_Shield : SharedVariable<Shebot_Shield>
    {
        public static implicit operator SharedShebot_Shield(Shebot_Shield value)
        {
            return new SharedShebot_Shield { Value = value };
        }
    }
    
    [System.Serializable]
    public class SharedLineRenderer : SharedVariable<LineRenderer>
    {
        public static implicit operator SharedLineRenderer(LineRenderer value)
        {
            return new SharedLineRenderer { Value = value };
        }
    }
    
    [System.Serializable]
    public class SharedDetectionGizmo : SharedVariable<DetectionGizmo>
    {
        public static implicit operator SharedDetectionGizmo(DetectionGizmo value)
        {
            return new SharedDetectionGizmo { Value = value };
        }
    }    
    
    [System.Serializable]
    public class SharedVisualEffect : SharedVariable<VisualEffect>
    {
        public static implicit operator SharedVisualEffect(VisualEffect value)
        {
            return new SharedVisualEffect { Value = value };
        }
    }
}