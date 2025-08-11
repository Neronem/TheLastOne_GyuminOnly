# 🧐 AI 구현

<br>

## 📁 폴더 구조

```
BehaviorDesigner/      # BT 노드 모음집
├── Action             # 행동을 실행하는 최하위 노드 모음
├── Condition          # 조건 분기를 따지는 최하위 노드 모음
├── SharedVariable     # BlackBoard 공유 변수 커스텀 생성

Data/                  # AI 로직에 필요한 데이터 모음집
├── AnimationHashData  # 애니메이션 해시값 정적 캐싱 (성능 및 오타 방지)
├── ForRuntime         # 스탯 SO를 읽어와 자신에게 복제. 런타임에 수정하기 위한 용도
├── StatDataSO         # 각 Npc의 특수 능력치 SO

StatControllers/       # 각 Npc 스탯 보관 및 변경 로직 모음집
├── Base               # 여러 Npc가 공통으로 소유한 로직 구현한 추상클래스
├── Dog                # AI 객체 중 하나인 Dog의 스탯 변경 로직
├── Drone              # 드론의 스탯 변경 로직
├── Shebots            # 인간형 AI의 스탯 변경 로직
```

<br>

---

<br>

## 💻 코드 샘플 및 주석

### 1. 액션 노드 - 주변에 알림 전달

- [BT에 관한 기술 스택 설명](https://github.com/Neronem/TheLastOne_Public/blob/main/Tech%20Stack/Tech%20Stack%204_Tech%20of%20Npc.md)

```csharp
[TaskCategory("Every")]
[TaskDescription("AlertNearBy")]
public class AlertNearBy : global::BehaviorDesigner.Runtime.Tasks.Action
{
    public SharedTransform selfTransform;
    public SharedTransform targetTransform;
    public SharedVector3 targetPos;
    
    public SharedCollider myCollider;
    public SharedBaseNpcStatController statController;
    public SharedLight enemylight;
    public SharedLight allyLight;
    public SharedBool isAlerted;
    public SharedBool shouldAlertNearBy;

    public bool isDog;
    
    public override TaskStatus OnUpdate()
    {
        shouldAlertNearBy.Value = false;
        if (isAlerted.Value) return TaskStatus.Success;
        if (targetTransform.Value == null) return TaskStatus.Success;
        
        if (!statController.Value.TryGetRuntimeStatInterface<IAlertable>(out var alertable)) // 있을 시 변환
        {
            return TaskStatus.Failure;
        }
        
        (statController.Value.RuntimeStatData.IsAlly ? allyLight.Value : enemylight.Value).enabled = true; // 경고등 On
        if (isDog) CoreManager.Instance.soundManager.PlaySFX(SfxType.Dog, myCollider.Value.bounds.center, index:0); // 사운드 출력
        else CoreManager.Instance.soundManager.PlaySFX(SfxType.Drone, myCollider.Value.bounds.center, index:1); 
        isAlerted.Value = true;

        int droneAlertKey = 13;
        CoreManager.Instance.dialogueManager.TriggerDialogue(droneAlertKey);
        
        bool isAlly = statController.Value.RuntimeStatData.IsAlly; 
        Vector3 selfPos = selfTransform.Value.position;
        float range = alertable.AlertRadius;

        int layerMask = isAlly ? 1 << LayerConstants.Ally : 1 << LayerConstants.Enemy;
        Collider[] colliders = Physics.OverlapSphere(selfPos, range, layerMask); // 주변 콜라이더 모음

        foreach (var col in colliders)
        {
            if (col == myCollider.Value)
            {
                continue;
            }
            
            var BT = col.GetComponent<global::BehaviorDesigner.Runtime.BehaviorTree>();
            if (BT != null)
            {
                BT.SetVariableValue(BehaviorNames.TargetTransform, targetTransform.Value);
                BT.SetVariableValue(BehaviorNames.TargetPos, targetPos.Value);
                BT.SetVariableValue(BehaviorNames.IsAlerted, true);
            }
        }
        
        return TaskStatus.Success;
    }
}
```
- "적 발견 시 주변에 알리기" 행동 패턴 구현 클래스
- IAlertable 인터페이스를 가져와 사거리 확인 후 주변 AI에 알람 상태 전파
- [코드로 이동](https://github.com/Neronem/TheLastOne_GyuminOnly/blob/main/1.%20AI%20%EA%B5%AC%ED%98%84/AIBehaviors/BehaviorDesigner/Action/AlertNearBy.cs)

<br>

---

<br>

### 2. 조건 노드 - 현재 경로가 있는지 체크

- [BT에 관한 기술 스택 설명](https://github.com/Neronem/TheLastOne_Public/blob/main/Tech%20Stack/Tech%20Stack%204_Tech%20of%20Npc.md)

```csharp
[TaskCategory("Every")]
[TaskDescription("IsNavMeshAgentHasPath")]
public class IsNavMeshAgentHasPath : Conditional
{
    public SharedNavMeshAgent agent;
    public SharedVector3 targetPosition;
    public SharedBaseNpcStatController statController;
    
    public override TaskStatus OnUpdate()
    {
        if (statController.Value.RuntimeStatData.IsAlly && targetPosition.Value == Vector3.zero)
        {
            return TaskStatus.Success;
        }
        
        if (!agent.Value.hasPath && targetPosition.Value == Vector3.zero)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
}
```
- "지속적으로 주변 반경을 정찰" 행동 패턴 조건 클래스
- 지정한 경로가 존재할 때 다른 경로를 지정하는 것을 방지하는 용도
- [코드로 이동](https://github.com/Neronem/TheLastOne_GyuminOnly/blob/main/1.%20AI%20%EA%B5%AC%ED%98%84/AIBehaviors/BehaviorDesigner/Condition/IsNavMeshAgentHasPath.cs)

<br>

---

<br>

### 3. StatData - 자폭 드론 스탯

```csharp
[CreateAssetMenu(fileName = "Stats", menuName = "ScriptableObjects/Entity/Enemy/SuicideDroneData")]
public class SuicideDroneStatData : EntityStatData, IAttackable, IAlertable, IBoomable, IDetectable
{
    [Header("Range")] 
    [SerializeField] private float attackRange;
    [SerializeField] private float boomRange;
    [SerializeField] private float detectRange;
    
    [Header("Alert")]
    [SerializeField] private float alertDuration;
    [SerializeField] private float alertRadius;
    
    public float AttackRange => attackRange;
    public float AlertDuration => alertDuration;
    public float AlertRadius => alertRadius;
    public float BoomRange => boomRange;
    public float DetectRange => detectRange;
}
```
- EntityStatData에선 전체 살아있는 객체가 가져야할 스탯 정의 (ex. 이동속도)
- SuicideDroneStatData같이 세부 스탯은 인터페이스를 상속하여 스탯을 정의
- 의존성 역전에 의해 사용하는 곳에선 SuicideDroneStatData을 알 필요 없이 인터페이스만 참조 가능
- [코드로 이동](https://github.com/Neronem/TheLastOne_GyuminOnly/blob/main/1.%20AI%20%EA%B5%AC%ED%98%84/Data/StatDataSO/SuicideDroneStatData.cs)

<br>

---

<br>

### 4. RuntimeStatData

```csharp
public class RuntimeEntityStatData
{
    private readonly EntityStatData originalSO;
    
    public int SpawnIndex { get; set; }
    public string EntityName { get; set; }
    public bool IsPlayer { get; set; }
    public bool IsAlly { get; set; }
    public float MaxHealth { get; set; }
    public int BaseDamage { get; set; }
    public float BaseAttackRate { get; set; }
    public float Armor { get; set; }
    public float MaxArmor { get; set; }
    public float MoveSpeed { get; set; }
    public float RunMultiplier { get; set; }
    public float WalkMultiplier { get; set; } 

    public AudioClip[] footStepSounds;
    public AudioClip[] hitSounds;
    public AudioClip[] deathSounds;
    
    protected RuntimeEntityStatData(EntityStatData so)
    {
        originalSO = so;
        
        SpawnIndex = so.spawnIndex;
        
        EntityName = so.entityName;
        IsPlayer = so.isPlayer;
        IsAlly = so.isAlly;

        MaxHealth = so.maxHealth;
        BaseDamage = so.baseDamage;
        BaseAttackRate = so.baseAttackRate;
        Armor = so.armor;
        MaxArmor = so.maxArmor;
        
        MoveSpeed = so.moveSpeed;
        RunMultiplier = so.runMultiplier;
        WalkMultiplier = so.walkMultiplier;

        footStepSounds = so.footStepSounds;
        hitSounds = so.hitSounds;
        deathSounds = so.deathSounds;
    }
    
    /// <summary>
    /// 공통 스탯 리셋
    /// </summary>
    public virtual void ResetStats()
    {
        EntityName = originalSO.entityName;
        IsPlayer = originalSO.isPlayer;
        IsAlly = originalSO.isAlly;
        
        MaxHealth = originalSO.maxHealth;
        BaseDamage = originalSO.baseDamage;
        BaseAttackRate = originalSO.baseAttackRate;
        Armor = originalSO.armor;
        MaxArmor = originalSO.maxArmor;
        
        MoveSpeed = originalSO.moveSpeed;
        RunMultiplier = originalSO.runMultiplier;
        WalkMultiplier = originalSO.walkMultiplier;
        
        footStepSounds = originalSO.footStepSounds;
        hitSounds = originalSO.hitSounds;
        deathSounds = originalSO.deathSounds;
    }
}
```

- SO를 조작하면 전체가 바뀌기에, 복사본을 생성하는 클래스들 모음
- [코드로 이동](https://github.com/Neronem/TheLastOne_GyuminOnly/blob/main/1.%20AI%20%EA%B5%AC%ED%98%84/Data/ForRuntime/StatsForRuntime.cs)

<br>

---

<br>

### 5. BaseNpcStatController

```csharp
/// <summary>
/// Npc 스텟 공통로직 정의
/// </summary>
public abstract class BaseNpcStatController : MonoBehaviour, IStunnable, IHackable, IBleedable
{
    // 자식마다 들고있는 런타임 스탯을 부모가 가지고 있도록 함
    [Header("StatData")]
    protected RuntimeEntityStatData runtimeStatData;
    public RuntimeEntityStatData RuntimeStatData => runtimeStatData;
    public bool IsDead { get; private set; }
    
    // 조절할 컴포넌트
    [Header("Components")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected BehaviorTree behaviorTree;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private Collider[] colliders;
    [SerializeField] private Light[] lights;
    [SerializeField] private DamageConvertForNpc[] damageConvertForNpc;
    [SerializeField] protected ParticleSystem onStunParticle;
    
    // 스턴 관련 플래그
    [Header("Stunned")] 
    private bool isStunned;
    public bool IsStunned => isStunned;
    
    // 해킹 관련 정보
    [Header("Hacking_Process")]
    public bool isHacking;
    [SerializeField] private bool canBeHacked = true;
    [SerializeField] private bool useSelfDefinedChance = true;
    [SerializeField] protected float hackingDuration = 3f;
    [SerializeField] protected float successChance = 0.7f;
    protected virtual bool CanBeHacked => canBeHacked; // 오버라이드해서 false로 바꾸거나, 인스펙터에서 설정
    private HackingProgressUI hackingProgressUI;
    private Dictionary<Transform, int> originalLayers = new();
    
    // 해킹 성공 시 올려야할 퀘스트 진행도들
    [Header("Hacking_Quest")]  
    [SerializeField] private bool shouldCountHackingQuest;
    [SerializeField] private int[] hackingQuestIndex;
    
    // 사망 시 올려야할 퀘스트 진행도들
    [Header("Kill_Quest")] 
    [SerializeField] private bool shouldCountKillQuest;
    [SerializeField] private int[] killQuestIndex;
    private static bool hasFirstkilled = false;

    private const int FirstKillDialogueKey = 1;
    
    // 이 스크립트에서 사용하는 토큰들
    private CancellationTokenSource hackCts;
    private CancellationTokenSource stunCts;
    private CancellationTokenSource bleedCts;

    // 사망 시 이벤트 (외부에서 등록)
    public event Action OnDeath;
    
    protected virtual void Awake()
    {
        animator ??= GetComponent<Animator>();
        behaviorTree ??= GetComponent<BehaviorTree>();
        agent ??= GetComponent<NavMeshAgent>();
        onStunParticle ??= this.TryGetChildComponent<ParticleSystem>("PlasmaExplosionEffect");
        if (damageConvertForNpc == null || damageConvertForNpc.Length == 0) damageConvertForNpc = GetComponentsInChildren<DamageConvertForNpc>();
        if (lights == null || lights.Length == 0) lights = GetComponentsInChildren<Light>();
        if (colliders == null || colliders.Length == 0) colliders = GetComponentsInChildren<Collider>();
        IsDead = false;
        
        foreach (DamageConvertForNpc convert in damageConvertForNpc) { convert.Initialize(this); }
        CacheOriginalLayers(this.transform);
    }
    
    /// <summary>
    /// Components 초기화 용도
    /// </summary>
    protected virtual void Reset()
    {
        animator = GetComponent<Animator>();
        behaviorTree = GetComponent<BehaviorTree>();
        agent = GetComponent<NavMeshAgent>();
        lights = GetComponentsInChildren<Light>();
        colliders = GetComponentsInChildren<Collider>();
        damageConvertForNpc = GetComponentsInChildren<DamageConvertForNpc>();
        onStunParticle = this.TryGetChildComponent<ParticleSystem>("PlasmaExplosionEffect");
    }
    
    /// <summary>
    /// 풀링 사용하므로 반드시 반환될때마다 초기화 해야함
    /// </summary>
    protected virtual void OnDisable()
    {
        // 사망 초기화
        IsDead = false;
        behaviorTree.SetVariableValue(BehaviorNames.IsDead, false);
        
        // 스탯 초기화
        isHacking = false;
        isStunned = false;
        if (agent != null) agent.enabled = false;
        
        if (onStunParticle != null && onStunParticle.IsAlive())
        {
            onStunParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        } 
        
        ResetLayersToOriginal();
        DisposeAllUniTasks();
    }

    protected virtual void OnEnable()
    { 
        if (CoreManager.Instance.spawnManager.IsVisible) 
            NpcUtil.SetLayerRecursively(this.gameObject, RuntimeStatData.IsAlly ? LayerConstants.StencilAlly : LayerConstants.StencilEnemy, LayerConstants.IgnoreLayerMask_ForStencil, false);
        foreach (Collider coll in colliders) { coll.enabled = true; }

        animator.speed = 1f;
    }

    protected abstract void PlayHitAnimation();
    protected virtual void PlayDeathAnimation() { animator.speed = 1f; }
    protected abstract void HackingFailurePenalty();

    public virtual void OnTakeDamage(int damage)
    {
        if (IsDead) return;
       
        float armorRatio = RuntimeStatData.Armor / RuntimeStatData.MaxArmor;
        float reducePercent = Mathf.Clamp01(armorRatio);
        damage = (int)(damage * (1f - reducePercent));

        RuntimeStatData.MaxHealth -= damage;

        if (RuntimeStatData.MaxHealth <= 0) // 사망
        {
            if (!hasFirstkilled)
            {
                hasFirstkilled = true;
                CoreManager.Instance.dialogueManager.TriggerDialogue(FirstKillDialogueKey);
            }

            if (CoreManager.Instance.sceneLoadManager.CurrentScene == SceneType.Stage2)
            {
                GameEventSystem.Instance.RaiseEvent(runtimeStatData.SpawnIndex);
            }
            else
            {
                if (shouldCountKillQuest && !RuntimeStatData.IsAlly)
                {
                    foreach (int index in killQuestIndex) GameEventSystem.Instance.RaiseEvent(index);
                }   
            }

            foreach (Light objlight in lights) { objlight.enabled = false; }
            foreach (Collider coll in colliders) { coll.enabled = false; }
            
            behaviorTree.SetVariableValue(BehaviorNames.IsDead, true);
            PlayDeathAnimation();
            if (!runtimeStatData.IsAlly)
                CoreManager.Instance.gameManager.Player.PlayerCondition.OnRecoverFocusGauge(FocusGainType.Kill);
            OnDeath?.Invoke();
            IsDead = true;
        }
        else
        {
            PlayHitAnimation();
        }
    }

    public void OnBleed(int totalTick, float tickInterval, int damagePerTick)
    {
        bleedCts?.Cancel();
        bleedCts?.Dispose();
        bleedCts = NpcUtil.CreateLinkedNpcToken();
        
        _= BleedAsync(totalTick, tickInterval, damagePerTick, bleedCts.Token);
    }

    private async UniTaskVoid BleedAsync(int totalTick, float tickInterval, int damagePerTick, CancellationToken token)
    {
        for (int i = 0; i < totalTick; i++)
        {
            OnTakeDamage(damagePerTick);
            await UniTask.WaitForSeconds(tickInterval, cancellationToken:token);
        }
    }

    #region 상호작용
    public void Hacking(float chance)
    {
        if (IsDead || !CanBeHacked || isHacking || RuntimeStatData.IsAlly) return;

        var obj = CoreManager.Instance.objectPoolManager.Get("HackingProgressUI");
        hackingProgressUI = obj.GetComponent<HackingProgressUI>();
        hackingProgressUI.SetTarget(transform);
        hackingProgressUI.gameObject.SetActive(true);
        hackingProgressUI.SetProgress(0f);
        
        if (isStunned)
        {
            hackingProgressUI.SetProgress(1f);
            HackingSuccess();
            return;
        }

        float finalChance = useSelfDefinedChance ? successChance : chance;
        
        hackCts?.Cancel();
        hackCts?.Dispose();
        hackCts = NpcUtil.CreateLinkedNpcToken();
        _ = HackingProcessAsync(finalChance, hackCts.Token);
    }

    private async UniTaskVoid HackingProcessAsync(float chance, CancellationToken token)
    {
        try
        {
            isHacking = true;

            // 1. 드론 멈추기
            float stunDurationOnHacking = hackingDuration + 1f; // 스턴 중 해킹결과가 영향 끼치지 않게 더 길게 설정
            OnStunned(stunDurationOnHacking);

            // 2. 해킹 시도 시간 기다림
            float time = 0f;
            while (time < hackingDuration)
            {
                time += Time.deltaTime;
                hackingProgressUI.SetProgress(time / hackingDuration);
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
            }

            // 3. 확률 판정
            bool success = UnityEngine.Random.value < chance;

            if (success)
            {
                HackingSuccess();
            }
            else
            {
                hackingProgressUI.OnFail();
                HackingFailurePenalty();
            }
        }
        catch (Exception ex)
        {
            hackingProgressUI.OnCanceled();
        }
        finally
        {
            isHacking = false;
        }
    }
    
    protected virtual void HackingSuccess()
    {
        RuntimeStatData.IsAlly = true;
        NpcUtil.SetLayerRecursively_Hacking(this.gameObject);
        
        if (CoreManager.Instance.sceneLoadManager.CurrentScene == SceneType.Stage2)
        {
            GameEventSystem.Instance.RaiseEvent(runtimeStatData.SpawnIndex);
        }
        else
        {
            if (shouldCountHackingQuest)
            {
                foreach (int index in hackingQuestIndex) {GameEventSystem.Instance.RaiseEvent(index);}
            }
        }
        
        CoreManager.Instance.gameManager.Player.PlayerCondition.OnRecoverFocusGauge(FocusGainType.Hack);
        hackingProgressUI.OnSuccess();
    }
    
    public void OnStunned(float duration = 3f)
    {
        if (IsDead) return;
        
        stunCts?.Cancel();
        stunCts?.Dispose();
        stunCts = NpcUtil.CreateLinkedNpcToken();
        _ = OnStunnedAsync(duration, stunCts.Token);
    }
    
    private async UniTaskVoid OnStunnedAsync(float duration, CancellationToken token)
    {
        isStunned = true;
        behaviorTree.SetVariableValue(BehaviorNames.CanRun, false);
        ResetAIState();
        animator.speed = 0f;
        
        onStunParticle.Play();
        await UniTask.WaitForSeconds(duration, cancellationToken: token); // 원하는 시간만큼 유지
        
        if (onStunParticle != null && onStunParticle.IsAlive())
        {
            onStunParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        isStunned = false;
        animator.speed = 1f;
        behaviorTree.SetVariableValue(BehaviorNames.CanRun, true);
    }
    
    /// <summary>
    /// 자식마다 들고있는 런타임 스탯에 특정 인터페이스가 있는지 검사 후, 그 인터페이스를 반환
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool TryGetRuntimeStatInterface<T>(out T result) where T : class
    {
        result = null;

        result = RuntimeStatData as T;
        if (result == null)
        {
            Debug.LogWarning($"{GetType().Name}의 RuntimeStatData에 {typeof(T).Name} 인터페이스가 없음");
            return false;
        }

        return true;
    }
}
```
- 모든 Npc의 스탯 변경 공통 로직을 정의 (데미지 처리, 스턴, 해킹, 출혈)
- 자식 StatController에선 runtimeStatData 필드에 자신을 넘김 
- 외부에서 BaseNpcStatController의 TryGetRuntimeStatInterface 실행 시 해당 Npc가 특정 동작 수행이 가능한지, 수치는 얼마인지 알 수 있음
- [코드로 이동](https://github.com/Neronem/TheLastOne_GyuminOnly/blob/main/1.%20AI%20%EA%B5%AC%ED%98%84/StatControllers/Base/BaseNpcStatController.cs#L383)

---
