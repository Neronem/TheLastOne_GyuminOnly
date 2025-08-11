using System;
using UnityEngine;

namespace _1.Scripts.Entity.Scripts.NPC.Data.AnimationHashData
{
    public static class DroneAnimationData
    {
        public const string RepairStr = "DroneBot_Repair";
        public const string Dead3Str = "DroneBot_Dead3";
        public const string Dead2Str = "DroneBot_Dead2";
        public const string Dead1Str = "DroneBot_Dead1";
        public const string Hit4Str = "DroneBot_Hit4";
        public const string Hit3Str = "DroneBot_Hit3";
        public const string Hit2Str = "DroneBot_Hit2";
        public const string Hit1Str = "DroneBot_Hit1";
        public const string StrafeLeftStr = "DroneBot_StrafeLeft";
        public const string Idle1Str = "DroneBot_Idle1";
        public const string FireStr = "DroneBot_Fire";

        public static readonly int Repair = Animator.StringToHash(RepairStr);
        public static readonly int Dead3 = Animator.StringToHash(Dead3Str);
        public static readonly int Dead2 = Animator.StringToHash(Dead2Str);
        public static readonly int Dead1 = Animator.StringToHash(Dead1Str);
        public static readonly int Hit4 = Animator.StringToHash(Hit4Str);
        public static readonly int Hit3 = Animator.StringToHash(Hit3Str);
        public static readonly int Hit2 = Animator.StringToHash(Hit2Str);
        public static readonly int Hit1 = Animator.StringToHash(Hit1Str);
        public static readonly int StrafeLeft = Animator.StringToHash(StrafeLeftStr);
        public static readonly int Idle1 = Animator.StringToHash(Idle1Str);
        public static readonly int Fire = Animator.StringToHash(FireStr);
    }

    public static class ShebotAnimationData
    {
        public const string Shebot_Sword_RunStr = "ShebotSword_Run";
        public const string Shebot_Sword_Run_AnimationNameStr = "anim_f2_run";
        public const string Shebot_IdleStr = "Shebot_Idle";
        public const string Shebot_DieStr = "Shebot_Die";
        public const string Shebot_GuardStr = "Shebot_Guard";
        public const string Shebot_Sword_Attack3Str = "Shebot_Sword_Attack3";
        public const string Shebot_Sword_Attack_FullStr = "Shebot_Sword_Attack_Full";
        public const string Shebot_WalkStr = "Shebot_Walk";
        public const string Shebot_Guard_StayStr = "Shebot_Guard_Stay";
        public const string Shebot_Rifle_AimStr = "Shebot_Rifle_Aim";
        public const string Shebot_Rifle_fireStr = "Shebot_Rifle_fire";
        public const string Shebot_Rifle_idleStr = "Shebot_Rifle_idle";
        public const string Shebot_Rifle_RunStr = "Shebot_Rifle_Run";
        public const string Shebot_Rifle_fire_2Str = "Shebot_Rifle_fire_2";
        public const string GettingHit_IdleStr = "GettingHit_Idle";
        public const string GettingHit_AimStr = "GettingHit_Aim";

        public static readonly int ShebotSword_Run = Animator.StringToHash(Shebot_Sword_RunStr);
        public static readonly int Shebot_Idle = Animator.StringToHash(Shebot_IdleStr);
        public static readonly int Shebot_Die = Animator.StringToHash(Shebot_DieStr);
        public static readonly int Shebot_Guard = Animator.StringToHash(Shebot_GuardStr);
        public static readonly int Shebot_Sword_Attack3 = Animator.StringToHash(Shebot_Sword_Attack3Str);
        public static readonly int Shebot_Sword_Attack_Full = Animator.StringToHash(Shebot_Sword_Attack_FullStr);
        public static readonly int Shebot_Walk = Animator.StringToHash(Shebot_WalkStr);
        public static readonly int Shebot_Guard_Stay = Animator.StringToHash(Shebot_Guard_StayStr);
        public static readonly int Shebot_Rifle_Aim = Animator.StringToHash(Shebot_Rifle_AimStr);
        public static readonly int Shebot_Rifle_fire = Animator.StringToHash(Shebot_Rifle_fireStr);
        public static readonly int Shebot_Rifle_idle = Animator.StringToHash(Shebot_Rifle_idleStr);
        public static readonly int Shebot_Rifle_Run = Animator.StringToHash(Shebot_Rifle_RunStr);
        public static readonly int Shebot_Rifle_fire_2 = Animator.StringToHash(Shebot_Rifle_fire_2Str);
        public static readonly int GettingHit_Idle = Animator.StringToHash(GettingHit_IdleStr);
        public static readonly int GettingHit_Aim = Animator.StringToHash(GettingHit_AimStr);
    }

    public static class DogAnimationData
    {
        public const string Dog_WalkStr = "Dog_Walk";
        public const string Dog_Idle1Str = "Dog_Idle1";
        public const string Dog_Idle2Str = "Dog_Idle2";
        public const string Dog_Idle3Str = "Dog_Idle3";
        public const string Dog_Idle4Str = "Dog_Idle4";
        public const string Dog_RunStr = "Dog_Run";
        public const string Dog_Death1Str = "Dog_Death1";
        public const string Dog_Death2Str = "Dog_Death2";
        public const string Dog_Attack1Str = "Dog_Attack1";
        public const string Dog_Attack2Str = "Dog_Attack2";
        public const string Dog_Attack3Str = "Dog_Attack3";
        
        public static readonly int Dog_Walk = Animator.StringToHash(Dog_WalkStr);
        public static readonly int Dog_Idle1 = Animator.StringToHash(Dog_Idle1Str);
        public static readonly int Dog_Idle2 = Animator.StringToHash(Dog_Idle2Str);
        public static readonly int Dog_Idle3 = Animator.StringToHash(Dog_Idle3Str);
        public static readonly int Dog_Idle4 = Animator.StringToHash(Dog_Idle4Str);
        public static readonly int Dog_Run = Animator.StringToHash(Dog_RunStr);
        public static readonly int Dog_Death1 = Animator.StringToHash(Dog_Death1Str);
        public static readonly int Dog_Death2 = Animator.StringToHash(Dog_Death2Str);
        public static readonly int Dog_Attack1 = Animator.StringToHash(Dog_Attack1Str);
        public static readonly int Dog_Attack2 = Animator.StringToHash(Dog_Attack2Str);
        public static readonly int Dog_Attack3 = Animator.StringToHash(Dog_Attack3Str);
    }
}