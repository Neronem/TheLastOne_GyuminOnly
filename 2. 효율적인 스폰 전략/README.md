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
- [디버깅 윈도우에 관한 기술 스택 설명](https://github.com/Neronem/TheLastOne_Public/blob/main/Tech%20Stack/Tech%20Stack%205_Debug%20Window.md)
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
- [코드로 이동](https://github.com/Neronem/TheLastOne_GyuminOnly/blob/main/2.%20%ED%9A%A8%EC%9C%A8%EC%A0%81%EC%9D%B8%20%EC%8A%A4%ED%8F%B0%20%EC%A0%84%EB%9E%B5/CustomDebugWindow.cs)

<br>

---

<br>

### 2. Npc 풀 생성 - ObjectPoolManager

- [오브젝트 풀링에 관한 기술 스택 설명](https://github.com/Neronem/TheLastOne_Public/blob/main/Tech%20Stack/Tech%20Stack%202_Optimization.md#object-pooling)

```csharp
/// <summary>
/// 오브젝트풀 생성
/// 최소/최대 용량지정은 필수 X
/// </summary>
/// <param name="prefab"></param>
/// <param name="capacity"></param>
/// <param name="maxSize"></param>
public void CreatePool(GameObject prefab, int capacity = -1, int maxSize = -1)
{
    if (pools.ContainsKey(prefab.name))
    {
        return;
    }

    int poolCapacity = capacity > 0 ? capacity : defaultCapacity;
    int poolMaxSize = maxSize > 0 ? maxSize : maxCapacity;

    Transform parent = new GameObject($"{prefab.name}_Parent").transform;
    parent.SetParent(poolRoot);
    
    var pool = new ObjectPool<GameObject>(
        createFunc: () =>
        {
            GameObject obj = UnityEngine.Object.Instantiate(prefab, parent);

            if (obj.TryGetComponent(out NavMeshAgent agent))
            {
                agent.enabled = false;
            }

            return obj;
        },
        actionOnGet: item => item.gameObject.SetActive(true),
        actionOnRelease: item =>
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(parent);
        },
        actionOnDestroy: item => UnityEngine.Object.Destroy(item.gameObject),
        collectionCheck: false,
        defaultCapacity: poolCapacity,
        maxSize: poolMaxSize);

    List<GameObject> preloadedObjects = new List<GameObject>(poolCapacity);
    for (int i = 0; i < poolCapacity; ++i)
    {
        GameObject obj = pool.Get();
        preloadedObjects.Add(obj);
    }

    foreach (var obj in preloadedObjects)
    {
        pool.Release(obj);
    }

    pools[prefab.name] = pool;
    activeObjects[prefab.name] = new HashSet<GameObject>();
}
```

- 메모리 절약을 위해 풀링 기법 사용
- [코드로 이동](https://github.com/Neronem/TheLastOne_GyuminOnly/blob/main/2.%20%ED%9A%A8%EC%9C%A8%EC%A0%81%EC%9D%B8%20%EC%8A%A4%ED%8F%B0%20%EC%A0%84%EB%9E%B5/Manager/Subs/ObjectPoolManager.cs)

<br>

---

<br>

### 3. 인덱스 기반 적 스폰 - SpawnManager
```csharp
public void SpawnEnemyBySpawnData(int index)
{
    if (CurrentSpawnData == null) return;
    if (!CurrentSpawnData.EnemySpawnPoints.TryGetValue(index, out var spawnPoints)) return;
    
    foreach (var pair in spawnPoints)
    {
        foreach (var val in pair.Value)
        {
            GameObject enemy = coreManager.objectPoolManager.Get(pair.Key.ToString());
            
            var behaviorTrees = enemy.GetComponentsInChildren<BehaviorTree>();
            var statControllers = enemy.GetComponentsInChildren<BaseNpcStatController>();
            var agents = enemy.GetComponentsInChildren<NavMeshAgent>();
            
            foreach (var behaviorTree in behaviorTrees) { behaviorTree.SetVariableValue(BehaviorNames.CanRun, false); }
            foreach (var statController in statControllers) { statController.RuntimeStatData.SpawnIndex = index + BaseEventIndex.BaseSpawnEnemyIndex; }
            
            enemy.transform.position = val.position;
            enemy.transform.rotation = val.rotation;
            
            foreach (var agent in agents) { agent.enabled = true; } // 적 객체마다 OnEnable에서 키면 위에서 Get()할때 켜져서 디폴트 위치로 가는 버그 재발함. 위치 지정 후 켜야함
            foreach (var behaviorTree in behaviorTrees) { behaviorTree.SetVariableValue(BehaviorNames.CanRun, true); }
            
            spawnedEnemies.Add(enemy);
        }
    }
}
```

- 매개변수로 받는 인덱스에 따라서 알맞는 적 스폰
- 위치 지정 전 BT 및 agent 비활성화 필수 (위치 연산 오류 발생)
- [코드로 이동](https://github.com/Neronem/TheLastOne_GyuminOnly/blob/main/2.%20%ED%9A%A8%EC%9C%A8%EC%A0%81%EC%9D%B8%20%EC%8A%A4%ED%8F%B0%20%EC%A0%84%EB%9E%B5/Manager/Subs/SpawnManager.cs)

<br>

---

<br>

### 4. 스폰 트리거
```csharp
private void OnTriggerEnter(Collider other)
{
    if (isSpawned || !other.CompareTag("Player")) return;
    
    if (!coreManager.spawnManager.CurrentSpawnData.EnemySpawnPoints.TryGetValue(spawnIndex,
            out var spawnPoints))
    {
        Debug.LogError("Couldn't find spawn point, Target Count is currently zero!");
        return;
    }
    
    Debug.Log("Spawned!");
    
    isSpawned = true;
    foreach (var point in spawnPoints)
        targetCount += point.Key is EnemyType.ShebotRifleDuo or EnemyType.ShebotSwordDogDuo ? point.Value.Count * 2 : point.Value.Count;

    if (invisibleWall) invisibleWall.SetActive(true);
    
    coreManager.spawnManager.SpawnEnemyBySpawnData(spawnIndex);
    GameEventSystem.Instance.RegisterListener(this);
    
    if (timeline) PlayCutScene(timeline);
}
```
- 씬에 스폰트리거를 미리 배치 후 인덱스 지정
- 플레이어가 진입 시 스폰매니저 호출, 투명벽 활성화
- [코드로 이동](https://github.com/Neronem/TheLastOne_GyuminOnly/blob/main/2.%20%ED%9A%A8%EC%9C%A8%EC%A0%81%EC%9D%B8%20%EC%8A%A4%ED%8F%B0%20%EC%A0%84%EB%9E%B5/GameEvents/SpawnEnemyByIndex.cs)

<br>

---
