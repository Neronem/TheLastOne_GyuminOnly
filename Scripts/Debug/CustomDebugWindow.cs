using _1.Scripts.Manager.Core;
using _1.Scripts.UI.Common;
using _1.Scripts.UI.InGame.Modification;
using _1.Scripts.VisualEffects;

namespace _6.Debug
{ 
#if UNITY_EDITOR
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using _1.Scripts.Entity.Scripts.Npc.StatControllers.Base;
    using _1.Scripts.Entity.Scripts.Player.Core;
    using _1.Scripts.Entity.Scripts.Player.Data;
    using _1.Scripts.Item.Common;
    using _1.Scripts.MiniGame.WireConnection;
    using _1.Scripts.Util;
    using _1.Scripts.Weapon.Scripts.Common;
    using UnityEditor;
    using UnityEngine;
    
    public class CustomDebugWindow : EditorWindow
    {
        private GameObject playerObj;

        [MenuItem("Window/Custom Debug Window")]
        public static void ShowWindow()
        {
            GetWindow<CustomDebugWindow>("Debug Window");
        }

        private void OnGUI()
        {
            GUILayout.Label("Custom Debug Tool", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Recover Focus Gauge"))
            {
                playerObj = GameObject.FindWithTag("Player");
                if (!playerObj.TryGetComponent(out Player playerComponent)) return;
                playerComponent.PlayerCondition.OnRecoverFocusGauge(FocusGainType.Debug);
            }

            if (GUILayout.Button("Recover Instinct Gauge"))
            {
                playerObj = GameObject.FindWithTag("Player");
                if (!playerObj.TryGetComponent(out Player playerComponent)) return;
                playerComponent.PlayerCondition.OnRecoverInstinctGauge(InstinctGainType.Debug);
            }

            if (GUILayout.Button("Find Spawn Points & Create S.O."))
            {
                var itemSpawnObjects = GameObject.FindGameObjectsWithTag("ItemSpawnPoint");
                var medkitSpawnPoints = itemSpawnObjects
                    .Where(obj => obj.name.Contains("_Medkit", StringComparison.OrdinalIgnoreCase)).Select(obj =>
                        new CustomTransform(obj.transform.position, obj.transform.rotation));
                var nanoAmpleSpawnPoints = itemSpawnObjects
                    .Where(obj => obj.name.Contains("_NanoAmple", StringComparison.OrdinalIgnoreCase)).Select(obj =>
                        new CustomTransform(obj.transform.position, obj.transform.rotation));
                var staminaPillSpawnPoints = itemSpawnObjects
                    .Where(obj => obj.name.Contains("_EnergyBar", StringComparison.OrdinalIgnoreCase)).Select(obj =>
                        new CustomTransform(obj.transform.position, obj.transform.rotation));
                var shieldSpawnPoints = itemSpawnObjects
                    .Where(obj => obj.name.Contains("_Shield", StringComparison.OrdinalIgnoreCase)).Select(obj =>
                        new CustomTransform(obj.transform.position, obj.transform.rotation));

                var weaponSpawnObjects = GameObject.FindGameObjectsWithTag("WeaponSpawnPoint");
                var pistolSpawnPoints = weaponSpawnObjects
                    .Where(obj => obj.name.Contains("_Pistol", StringComparison.OrdinalIgnoreCase))
                    .Select(obj => new CustomTransform(obj.transform.position, obj.transform.rotation));
                var rifleSpawnPoints = weaponSpawnObjects
                    .Where(obj => obj.name.Contains("_Rifle", StringComparison.OrdinalIgnoreCase))
                    .Select(obj => new CustomTransform(obj.transform.position, obj.transform.rotation));
                var glSpawnPoints = weaponSpawnObjects
                    .Where(obj => obj.name.Contains("_GL", StringComparison.OrdinalIgnoreCase))
                    .Select(obj => new CustomTransform(obj.transform.position, obj.transform.rotation));
                var crossbowSpawnPoints = weaponSpawnObjects
                    .Where(obj => obj.name.Contains("_Crossbow", StringComparison.OrdinalIgnoreCase))
                    .Select(obj => new CustomTransform(obj.transform.position, obj.transform.rotation));

                // 아이템들과는 달리 DroneSpawnPoints_indexone 이런식으로 하나하나 선언해야 함 (위치들만 찾는게 아니라 인덱스도 검사해야 함)
                var enemySpawnObjects = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");
                var enemyDict = new Dictionary<(int, EnemyType), List<CustomTransform>>();

                foreach (var obj in enemySpawnObjects)
                {
                    String[] parts = obj.name.Split('_');
                    if (parts.Length < 3) continue;
                    if (!Enum.TryParse(parts[1], true, out EnemyType enemyType)) continue;
                    if (!int.TryParse(parts[2], out int index)) continue;

                    ValueTuple<int, EnemyType> key = (index, enemyType);
                    if (!enemyDict.ContainsKey(key))
                    {
                        enemyDict[key] = new List<CustomTransform>();
                    }

                    enemyDict[key].Add(new CustomTransform(obj.transform.position, obj.transform.rotation));
                }

                var data = CreateInstance<SpawnData>();
                
                data.SetSpawnPoints(ItemType.Medkit, medkitSpawnPoints.ToArray());
                data.SetSpawnPoints(ItemType.NanoAmple, nanoAmpleSpawnPoints.ToArray());
                data.SetSpawnPoints(ItemType.EnergyBar, staminaPillSpawnPoints.ToArray());
                data.SetSpawnPoints(ItemType.Shield, shieldSpawnPoints.ToArray());

                data.SetSpawnPoints(WeaponType.Pistol, pistolSpawnPoints.ToArray());
                data.SetSpawnPoints(WeaponType.Rifle, rifleSpawnPoints.ToArray());
                data.SetSpawnPoints(WeaponType.GrenadeLauncher, glSpawnPoints.ToArray());
                data.SetSpawnPoints(WeaponType.HackGun, crossbowSpawnPoints.ToArray());

                foreach (var ((index, type), list) in enemyDict)
                {
                    Debug.Log($"{index}: {type}: {list}");
                    data.SetSpawnPoints(index, type, list.ToArray());
                }

                AssetDatabase.CreateAsset(data, "Assets/8.ScriptableObjects/SpawnPoint/SpawnPoints.asset");
                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button("Get All Available Parts"))
            {
                playerObj = GameObject.FindWithTag("Player");
                if (!playerObj.TryGetComponent(out Player player)) return;
                
                player.PlayerWeapon.GetAllAvailableParts();
            }

            if (GUILayout.Button("Forge Weapon"))
            {
                playerObj = GameObject.FindWithTag("Player");
                if (!playerObj.TryGetComponent(out Player player)) return;

                player.PlayerWeapon.ForgeWeapon();
            }
            
            if (GUILayout.Button("Clear PlayerPrefs"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Service.Log("PlayerPrefs 초기화 완료!");
            }

            if (GUILayout.Button("Show ModificationUI"))
            {
                CoreManager.Instance.uiManager.ShowUI<ModificationUI>();
                Service.Log("ModificationUI Show");
            }

            if (GUILayout.Button("Hide ModificationUI"))
            {
                CoreManager.Instance.uiManager.HideUI<ModificationUI>();
                Service.Log("ModificationUI Hide");
            }

            if (GUILayout.Button("Show Ending Credit"))
            {
                CoreManager.Instance.uiManager.ShowUI<EndingCreditUI>();
            }

            if (GUILayout.Button("Show BleedOverlay"))
            {
                CoreManager.Instance.gameManager.Player.PlayerCondition.OnBleed(3,2,1);
            }

            if (GUILayout.Button("FocusMode On"))
            {
                var switcher = FindObjectOfType<PostProcessEditForFocus>();
                switcher.FocusModeOnOrNot(true);
            }
            
            if (GUILayout.Button("FocusMode Off"))
            {
                var switcher = FindObjectOfType<PostProcessEditForFocus>();
                switcher.FocusModeOnOrNot(false);
            }
        }
    }
    #endif
}