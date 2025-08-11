using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.Gizmo
{
    public class DetectionGizmo : MonoBehaviour
    {
        public float range = 300;
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}
