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

- Behavior Tree
