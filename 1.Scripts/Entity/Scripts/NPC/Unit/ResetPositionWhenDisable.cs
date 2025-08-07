using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.Unit
{
    /// <summary>
    /// Disable시 본래 위치로 포지션 지정
    /// RifleDuo는 두 객체 위에 따로 부모가 존재하므로, 각자의 localPosition을 다시 갱신해야함 (스폰매니저는 최상위 부모 위치만 지정)
    /// </summary>
    public class ResetPositionWhenDisable : MonoBehaviour
    {
        private Vector3 originalPos;

        private void Awake()
        {
            originalPos = transform.localPosition;
        }

        private void OnEnable()
        {
            transform.localPosition = originalPos;
        }
    }
}
    
