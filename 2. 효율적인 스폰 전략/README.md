# 🧐 효율적인 스폰 전략

<br>

## 📁 폴더 구조

```
GameEvents/                     # 게임 내 발생하는 이벤트들
├── SpawnEnemyByIndex.cs        # 에너미를 스폰시키는 트리거

Manager/                        # 스폰 관련된 매니저들 모음집
├── Core/                       
├──── CoreManager.cs            # 서브 매니저들의 초기화 담당
├── Subs/
├── ResourceManager.cs          # 리소스 로딩 및 보관
├── ObjectPoolManager.cs        # 풀 생성 및 보관 
├── SpawnManager.cs             # 인덱스 기반 Npc 소환

SpawnData.cs                    # 스폰 위치 저장
CustomDebugWindow.cs            # 씬 내의 오브젝트 기준 스폰데이터 생성    
```

<br>

---

<br>

## 💻 코드 샘플 및 주석

### 1. 스폰 데이터 생성 - 디버깅 윈도우
```csharp
if (GUILayout.Button("Find Spawn Points & Create S.O."))
{
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

    foreach (var ((index, type), list) in enemyDict)
    {
        Debug.Log($"{index}: {type}: {list}");
        data.SetSpawnPoints(index, type, list.ToArray());
    }

    AssetDatabase.CreateAsset(data, "Assets/8.ScriptableObjects/SpawnPoint/SpawnPoints.asset");
    AssetDatabase.SaveAssets();
}
```
- 스폰데이터 좌표를 하나하나 지정하면 시간소요 증가
- 디버그윈도우로 이름과 태그를 지정한 게임오브젝트들의 위치를 자동으로 SpawnData로 매핑
- 