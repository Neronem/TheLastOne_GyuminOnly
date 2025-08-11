using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _1.Scripts.Static
{
    public static class PoolableGameObjects_Common
    {
        public static readonly HashSet<string> prefabs = new()
        {
            "Bullet",
            "BulletHole_Wall",
            "BulletHole_Ground",
            "EmpGrenadeExplosion",
            "Grenade",
            "SoundPlayer",
            "Pistol_Dummy",
            "Rifle_Dummy",
            "GrenadeLauncher_Dummy",
            "HackGun_Dummy",
            "Medkit_Prefab",
            "NanoAmple_Prefab",
            "Shield_Prefab",
            "EnergyBar_Prefab",
        };
    }

    public static class PoolableGameObjects_Stage1
    {
        public static readonly HashSet<string> prefabs = new()
        {
            "ReconDroneNotHackable",
            "BattleRoomReconDrone",
            "SuicideDroneNotHackable",
            "HackingProgressUI",
        };
    }

    public static class PoolableGameObjects_Stage2
    {
        public static readonly HashSet<string> prefabs = new()
        {
            "ShebotRifle",
            "ShebotRifleDuo",
            "ShebotSniper",
            "ShebotSword",
            "ShebotSwordDogDuo",
            "DogSolo",
            "WeaponPart_Dummy",
        };
    }

    /// <summary>
    /// SharedVariable 이름 보관 클래스
    /// </summary>
    public static class BehaviorNames
    {
        public const string TargetTransform = "target_Transform";
        public const string TargetPos = "target_Pos";
        public const string ShouldLookTarget = "shouldLookTarget";
        public const string IsAlerted = "IsAlerted";
        public const string Timer = "timer";
        public const string EnemyLight = "Enemy_Light";
        public const string AllyLight = "Ally_Light";
        public const string Agent = "agent";
        public const string SelfTransform = "self_Transform";
        public const string CanRun = "CanRun";
        public const string MaxViewDistance = "maxViewDistance";
        public const string Animator = "animator";
        public const string SelfCollider = "self_Collider";
        public const string StoppingDistance = "stoppingDistance";
        public const string IsDead = "IsDead";
        public const string ShouldAlertNearBy = "shouldAlertNearBy";
        public const string PlayerCollider = "playerCollider";
        public const string IsAlertedOnce = "isAlertedOnce";
        public const string ExplosionParticle = "ExplosionParticle";
        public const string StatController = "statController";
        public const string IsInterrupted = "isInterrupted";
        public const string ShieldUsedOnce = "shieldUsedOnce";
        public const string HasEnteredShield = "hasEnteredShield";
        public const string MuzzleLineRenderer = "muzzleLineRenderer";
        public const string MuzzleTransform = "muzzle_Transform";
        public const string BaseRotation = "baseRotation";
        public const string DetectionGizmo = "detectionGizmo";
        public const string ShootInterval = "shootInterval";
        public const string MuzzleVisualEffect = "muzzleVisualEffect";
        public const string RifleTransform = "RifleTransform";
        public const string SwordTransform = "SwordTransform";
        public const string IsAttacking = "isAttacking";
    }
}