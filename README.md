<br><br>

<p align="center">
  <img src="https://github.com/Neronem/TheLastOne_Public/blob/main/Images/img_TheLastOne.png" alt="img_TheLastOne.png" />
</p>

<br><br>

<a name="목차"></a>
## 목차

|                   [🧐 개요 🧐](#project)                   |
|:--------------------------------------------------------:|
|              [💻 주요 기술 코드 샘플 🛠💻](#sample)              |
|         [📁 Scripts 폴더 구조 및 담당 영역 📁](#scripts)          |

<br><br>

---

<br><br>

<a name="project"></a>
# 🧐 개요
TheLastOne 게임 제작 중, 송규민이 작업한 스크립트를 모아놨습니다. <br>
송규민은 **적 오브젝트를 담당**하였습니다.  
전체 스크립트는 [여기](https://github.com/Neronem/TheLastOne_Public)를 참고해주세요.

<br><br>

---

<br><br>

<a name="sample"></a>
## 💻 주요 기술 코드 샘플
#### 클릭하면 자세한 내용을 확인하실 수 있습니다!

<br>

[<img width="400" src="">](https://github.com/Neronem/TheLastOne_GyuminOnly/tree/main/1.%20AI%20%EA%B5%AC%ED%98%84) <br>
[<img width="400" src="https://github.com/Neronem/TheLastOne_Public/blob/main/Images/%EC%BD%94%EB%93%9C%EA%B5%AC%ED%98%84%20%EC%83%98%ED%94%8C_%EC%8A%A4%ED%8F%B0%EC%A0%84%EB%9E%B5.png">](https://github.com/Neronem/TheLastOne_GyuminOnly/tree/main/2.%20%ED%9A%A8%EC%9C%A8%EC%A0%81%EC%9D%B8%20%EC%8A%A4%ED%8F%B0%20%EC%A0%84%EB%9E%B5)


<br><br>

---

<br><br>

<a name="scripts"></a>
## 📁 Scripts 폴더 구조 및 담당 영역

### 1. `Entity/NPC`
- 적 NPC의 **AI 로직**, **스탯 관리**, **데이터 관리** 전반 담당
- 비헤이비어 트리 기반 AI 로직

### 2. `Interface`
- 적 스탯 및 AI 관련 기능에 사용되는 **공통 인터페이스** 정의

### 3. `Manager`
- **ResourceManager**: 적 프리팹을 주소 기반으로 로딩
- **PoolManager**: 오브젝트 풀링 시스템
- **SpawnManager**: 스테이지 기반 적 스폰 제어

### 4. `Map`
- **클리어 존**, **스폰 트리거** 등 맵에 존재하는 **NPC 관련 트리거 요소들 제어

### 5. `Static`
- 하드코딩 방지를 위해 자주 사용하는 **string**, **int** 값을 미리 정의
- 예: 애니메이션 이름, 비헤이비어 트리 변수명 등

### 6. `Util`
- NPC 관련 자주 사용하는 **유틸리티 함수**와 **확장 메서드** 정리

### 7. `VisualEffects`
- 적 오브젝트에 적용되는 **시각 효과(VFX)** 모음

<br><br>

---