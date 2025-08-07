    using System.Collections.Generic;
    using UnityEngine;

    namespace _1.Scripts.Static
    {
        public static class LayerConstants
        {
            public static readonly int Wall = LayerMask.NameToLayer("Wall");
            public static readonly int Ground = LayerMask.NameToLayer("Ground");
            public static readonly int Interactable = LayerMask.NameToLayer("Interactable");
            
            public static readonly int Ally = LayerMask.NameToLayer("Ally");
            public static readonly int Enemy = LayerMask.NameToLayer("Enemy");
            public static readonly int StencilAlly = LayerMask.NameToLayer("Stencil_Ally");
            public static readonly int StencilEnemy = LayerMask.NameToLayer("Stencil_E");
            
            // 부위별 데미지 목록
            public static readonly int Head_P = LayerMask.NameToLayer("Head_P");
            public static readonly int Chest_P = LayerMask.NameToLayer("Chest_P");
            public static readonly int Belly_P = LayerMask.NameToLayer("Belly_P");
            public static readonly int Arm_P = LayerMask.NameToLayer("Arm_P");
            public static readonly int Leg_P = LayerMask.NameToLayer("Leg_P");
            
            public static readonly int Head_E = LayerMask.NameToLayer("Head_E");
            public static readonly int Chest_E = LayerMask.NameToLayer("Chest_E");
            public static readonly int Belly_E = LayerMask.NameToLayer("Belly_E");
            public static readonly int Arm_E = LayerMask.NameToLayer("Arm_E");
            public static readonly int Leg_E = LayerMask.NameToLayer("Leg_E");
            
            public static readonly int DefaultHittableLayerMask =
                (1 << Wall) | (1 << Ground) | (1 << Interactable);
        
            public static readonly int AllyLayerMask =
                (1 << Head_P) | (1 << Chest_P) | (1 << Belly_P) | (1 << Arm_P) | (1 << Leg_P);

            public static readonly int EnemyLayerMask =
                (1 << Head_E) | (1 << Chest_E) | (1 << Belly_E) | (1 << Arm_E) | (1 << Leg_E);
        
            public static readonly int IgnoreLayerMask_ForStencil =
                (1 << Head_P) | (1 << Chest_P) | (1 << Belly_P) | (1 << Arm_P) | (1 << Leg_P) |
                (1 << Head_E) | (1 << Chest_E) | (1 << Belly_E) | (1 << Arm_E) | (1 << Leg_E);

            public static int ToLayerMask(int layer)
            {
                return 1 << layer;
            }
        }
    }

