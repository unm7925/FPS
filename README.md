# FPS — Steam 멀티플레이 라운드 기반 1인칭 슈터

Unity 6 + Mirror + Steamworks로 구현한 **1인 개발 멀티플레이 FPS**입니다.
히트스캔 사격, 부위별 피격 판정, 서버 권한 데미지 처리, Steam 로비 매칭/리더보드/클라우드 세이브까지
FPS 한 판이 돌아가는 데 필요한 전체 흐름을 직접 구현했습니다.

| 항목 | 내용 |
|---|---|
| 개발 기간 | 2026.03.04 ~ 2026.04.15 (약 6주) |
| 개발 인원 | 1인 |
| 엔진 / 언어 | Unity 6000.3.10f1 (URP) / C# |
| 장르 | 팀 기반 라운드 FPS (1v1 / 2v2) |
| 플랫폼 | PC (Windows, Steam) |
| 규모 | 자체 스크립트 61개, 커밋 40개 |

<!--
시연 GIF 자리. docs/ 에 아래 3개를 넣고 주석을 풀 것.
![사격과 반동](docs/gunplay.gif)
![봇 AI FSM](docs/bot_ai.gif)
![Steam 로비 매칭](docs/steam_lobby.gif)
-->

---

## 목차

- [주요 기능](#주요-기능)
- [기술 스택](#기술-스택)
- [프로젝트 구조](#프로젝트-구조)
- [핵심 구현](#핵심-구현)
- [트러블슈팅](#트러블슈팅)
- [실행 방법](#실행-방법)
- [현재 상태](#현재-상태)

---

## 주요 기능

**전투**
- 히트스캔 사격, 상태별 탄퍼짐 + 연사 반동 누적/회복
- 부위별 피격 판정 (머리 ×4.0 / 몸 ×1.0 / 팔다리 ×0.75)
- 무기 슬롯 교체, 드랍 / 습득, 교체·장전 애니메이션 연동
- 1인칭 손 카메라와 월드 카메라 분리 렌더링

**멀티플레이 (Mirror + FizzySteamworks)**
- Steam 로비 기반 방 만들기 / 코드 참가 / 랜덤 매칭 / Ready 동기화
- 로비 방장이 호스트로 승격되어 인게임 씬으로 전환
- 데미지·체력·사망 처리 전부 서버 권한, 클라이언트는 `SyncVar`로 수신
- 자동 팀 배정(A/B), 팀별 스폰

**싱글 (봇 매치)**
- NavMesh 기반 봇 AI, 5-state FSM (Patrol / Search / Attack / Straf / Idle)
- 난이도 3단계(Easy / Normal / Hard) — 조준 오차 범위로 차등
- 봇은 ObjectPool로 라운드마다 재사용

**게임 진행**
- 라운드제 승리(선취점), 라운드 시작 카운트다운 및 입력 잠금
- 라운드 종료 시 전원 리스폰 + 무기/탄약 초기화
- 킬/데스 집계, 라운드 스코어 HUD, 결과 UI

**Steam 연동**
- 리더보드 업로드/다운로드 (랭크 점수 기반 글로벌 랭킹)
- 친구 목록, 프로필, 아바타 이미지 로드
- Steam Cloud(`SteamRemoteStorage`)에 전적 JSON 저장/로드

---

## 기술 스택

| 분류 | 사용 기술 |
|---|---|
| 엔진 | Unity 6000.3.10f1, URP, Post Processing |
| 네트워크 | Mirror, FizzySteamworks (Steam P2P 트랜스포트) |
| 플랫폼 SDK | Steamworks.NET (Matchmaking, UserStats, Friends, RemoteStorage) |
| 입력 | Unity Input System |
| AI | NavMeshAgent, Unity AI Navigation |
| 데이터 | ScriptableObject, Newtonsoft.Json |
| 연출 | DOTween, LineRenderer |

**적용 패턴** — FSM, Object Pool, 인터페이스 기반 설계(`IState` / `IDamageable` / `IPoolable`),
이벤트 기반 UI 갱신, ScriptableObject 데이터 분리, 컴포지션 우선 구조

---

## 프로젝트 구조

```
Assets/01.Scripts/
├── Player/           # 이동·점프·중력, 입력 래핑, 플레이어 FSM
│   └── FSM/          # PlayerIdleState, PlayerMoveState
├── AI/               # 봇 컨트롤러, 시야 판정, 적 FSM
│   └── FSM/          # Patrol / Search / Attack / Straf / Idle
├── Weapon/           # WeaponBase(추상) → GunWeapon / BotGunWeapon
│                     # 무기 교체, 드랍, 탄퍼짐·반동, 탄 궤적 이펙트
├── Combat/           # HP(서버 권한 + SyncVar), Hitbox(부위 배율)
├── Manager/          # Game / Round / Spawn / Camera / Stats
│                     # CustomNetworkManager, Steam 계열 매니저
├── Steam/            # SteamHelper (아바타 텍스처 변환)
├── ObjectPool/       # 제네릭 ObjectPool<T>, IPoolable
├── Data/             # WeaponData·MatchData(SO), SaveData·MatchRecord(JSON)
├── Common/           # IState, IDamageable
└── UI/               # HUD, 로비/탭 패널, 리더보드, 전적, 결과 화면
```

---

## 핵심 구현

### 1. 서버 권한 데미지 처리

클라이언트가 체력을 직접 깎으면 조작이 가능하므로, **판정은 클라이언트 / 적용은 서버**로 분리했습니다.

```
클라이언트: GunWeapon.Fire()
  → Raycast로 Hitbox 검출, 배율 적용해 최종 데미지 계산
  → OnDamageDealt(targetNetId, damage) 이벤트 발행
       │
PlayerController.CmdApplyDamage()  [Command]  ← 서버로 전송
       │
서버:  HP.TakeDamage()  [Server]  → currentHP 감소
       │
       ├─ [SyncVar(hook)] → 전 클라이언트 HP UI 갱신
       └─ RpcDie()        → 킬 집계, 사망 처리
```

무기 스크립트는 네트워크를 전혀 모르고 `event Action<uint, int>`만 발행합니다.
Mirror 의존성을 `PlayerController` 한 곳에 모아, 무기 로직을 싱글/멀티 양쪽에서 그대로 재사용합니다.

> `Weapon/GunWeapon.cs`, `Player/PlayerController.cs`, `Combat/HP.cs`

### 2. 봇 AI — 5-state FSM

`IState` 인터페이스 기반 FSM으로, 상태 전이 조건과 실제 행동(코루틴)을 분리했습니다.

```
Patrol ──시야 내 적 감지──> Attack ──사격 후──> Straf (횡이동 회피)
  ▲                                                │
  │                                       ┌────────┴────────┐
  │                                  적 계속 보임        적 놓침
  │                                       │                │
  └──── 일정 시간 경과 ──── Search <───────┘         Attack 유지
                          (제자리 회전 탐색)
```

- 시야 판정: 각도(90°) 1차 필터 → Raycast 가림 판정. `0.2초` 간격 코루틴으로 폴링해 매 프레임 연산 회피
- 난이도는 조준점의 수직 오차 범위로 표현 — Easy `0~6`, Normal `2~4`, Hard `3.5~4`
  (Hard일수록 실제 상체 높이에 수렴)

> `AI/AIController.cs`, `AI/EnemyStateMachine.cs`, `AI/EnemySight.cs`, `Weapon/BotGunWeapon.cs`

### 3. 탄퍼짐 & 반동 모델

정확도를 **상태 기반 기본 탄퍼짐 + 이동 보정 + 연사 누적 반동**의 합으로 계산합니다.

```
finalSpread = min( baseSpread(Idle | Crouch | Jump)
                 + 이동속도비율 × spreadMove
                 + recoilSpread,
                 maxSpread )

발사할 때마다  recoilSpread += recoilIncrement
recoverDelay 이후  recoilSpread를 recoverRate로 0까지 회복
```

발사 방향은 카메라 정면 Ray에 `Random.insideUnitCircle * finalSpread`를 카메라 로컬 축으로 더해 결정합니다.
수치는 전부 `WeaponData`(ScriptableObject)에 있어 코드 수정 없이 총기별 밸런싱이 가능합니다.

> `Weapon/WeaponBase.cs`, `Data/WeaponData.cs`

### 4. 1인칭 카메라 / 렌더 레이어 분리

- 카메라는 씬에 하나만 두고 `CameraManager`가 로컬 플레이어의 `cameraMount`를 추적
- 좌우 회전은 플레이어 트랜스폼, 상하 회전은 카메라(-90°~90° Clamp)로 분리
- 로컬 플레이어의 몸 렌더러는 `LocalBody` 레이어로 옮기고 메인 카메라 컬링 마스크에서 제외
  → 1인칭 시점에서 내 몸이 시야를 가리지 않으면서, 다른 클라이언트에게는 정상적으로 보임

> `Manager/CameraManager.cs`, `Player/PlayerController.OnStartLocalPlayer()`

### 5. 라운드 진행 흐름

```
RoundManager.StartGame()
  → OnRoundStart   : 스폰/위치 초기화, HP·탄약 리필, 입력 잠금
  → 5초 카운트다운 : OnCountdown(i) → CountdownUI
  → OnRoundReady   : 플레이어 + 봇 입력 해제
  → 전투
  → GameManager.OnTeamEliminated (한 팀 전멸)
  → OnRoundEnd     : 스코어 갱신, 디스폰
  → 목표 라운드 도달 시 MatchWin → 결과 UI + 전적 저장
```

매니저 간 직접 호출 대신 `event`로 연결해, `SpawnManager` / `HUDController` / `CountdownUI`가
각자 필요한 시점만 구독하도록 했습니다.

> `Manager/RoundManager.cs`, `Manager/SpawnManager.cs`, `Manager/GameManager.cs`

### 6. 제네릭 ObjectPool

`where T : Component, IPoolable` 제약을 건 제네릭 풀입니다.
`IPoolable.OnSpawn()` / `OnReturn()`에서 상태 초기화를 강제해, 재사용 오브젝트가
이전 라운드의 타겟·경로·탄약을 들고 살아나는 문제를 막았습니다.

> `ObjectPool/ObjectPool.cs`, `ObjectPool/IPoolable.cs`

### 7. Steam 연동

| 기능 | 사용 API |
|---|---|
| 방 생성 / 참가 / 랜덤 매칭 | `SteamMatchmaking.CreateLobby`, `JoinLobby`, `RequestLobbyList` |
| Ready 동기화 | `LobbyDataUpdate_t` 콜백 → 전원 Ready 시 방장이 `StartHost()` |
| 글로벌 랭킹 | `FindOrCreateLeaderboard` → `UploadLeaderboardScore` → `DownloadLeaderboardEntries` |
| 친구 / 프로필 / 아바타 | `SteamFriends`, `GetImageRGBA` → `Texture2D` 변환 |
| 전적 저장 | `SteamRemoteStorage.FileWrite/FileRead` + Newtonsoft.Json |

전적은 Steam Cloud에 JSON으로 저장되어 PC를 바꿔도 유지되고,
승/패에 따른 랭크 점수(+5 / -3)가 그대로 리더보드 점수로 올라갑니다.

> `Manager/SteamLobbyManager.cs`, `Manager/SteamLeaderboardManager.cs`, `Manager/MatchRecordManager.cs`

---

## 트러블슈팅

### 1인칭에서 내 캐릭터 몸이 화면을 가림

**문제** — 1인칭 카메라를 머리에 붙이자 자신의 몸/팔 메시가 화면 전체를 덮었습니다.
카메라를 몸 밖으로 빼면 벽 관통과 피격 위치 불일치가 생겨 근본 해결이 아니었습니다.

**해결** — 렌더링과 판정을 레이어로 분리했습니다.
`OnStartLocalPlayer()`에서 **로컬 플레이어의 렌더러만** `LocalBody` 레이어로 옮기고,
`CameraManager`에서 `cam.cullingMask &= ~LayerMask.GetMask("LocalBody")`로 제외했습니다.
콜라이더는 그대로 두었기 때문에 다른 클라이언트에서는 정상적으로 보이고 피격도 됩니다.
1인칭 손/무기는 별도 `FirstPersonHand` 트랜스폼에 런타임 주입해 몸과 독립적으로 렌더링합니다.

### 자기 자신을 쏴서 데미지를 입음

**문제** — 카메라가 캐릭터 내부에 있어, 발사한 Raycast가 **자기 히트박스에 먼저 맞았습니다.**

**해결** — 로컬 플레이어의 히트박스를 `SelfHitbox` 레이어로 옮기고,
사격 Raycast에서 `~hitboxLayer` 마스크로 제외했습니다.
`Raycast` 결과를 코드로 걸러내는 대신 물리 단계에서 제외해, 벽 뒤 적을 관통 판정하는 부작용도 없앴습니다.

### 리스폰 시 카메라가 이전 위치에서 날아옴

**문제** — 카메라 추적을 `Vector3.Lerp`로 부드럽게 처리했더니,
라운드 시작으로 플레이어가 반대편 스폰으로 순간이동할 때 카메라가 맵을 가로질러 날아갔습니다.

**해결** — `CameraManager.TeleportToTarget()`을 추가해,
스폰처럼 **의도된 위치 점프**에서는 보간을 건너뛰고 목표 위치로 즉시 스냅하도록 분리했습니다.
평상시 이동은 그대로 Lerp를 유지합니다.

### 카운트다운 중에 선점 사격이 가능함

**문제** — 라운드 시작 카운트다운을 넣었지만 카운트다운 동안 이동·사격이 그대로 되어,
스폰 위치를 아는 쪽이 먼저 쏘면 라운드가 끝났습니다. 봇도 마찬가지로 즉시 교전을 시작했습니다.

**해결** — 플레이어는 `SetLocked()`로 `PlayerInputHandler`와 `PlayerController`를 비활성화,
봇은 `agent.isStopped = true` + FSM 비활성화로 잠갔습니다.
`RoundManager.OnRoundReady` 이벤트 시점에 양쪽을 동시에 해제해, 잠금/해제 타이밍을 한 곳에서 관리합니다.

### 라운드마다 봇을 생성/파괴해 순간 프레임 드랍

**문제** — 라운드 시작마다 `Instantiate`, 종료마다 `Destroy`를 호출해
NavMeshAgent·Animator 초기화 비용과 GC로 라운드 전환 때마다 끊김이 발생했습니다.

**해결** — 제네릭 `ObjectPool<T>`로 교체하고 `IPoolable`로 재사용 규약을 강제했습니다.
`OnSpawn()`에서 타겟·HP·탄약·이동 상태를 초기화하고,
`OnReturn()`에서 `StopAllCoroutines()`와 `agent.ResetPath()`로 이전 라운드 상태를 확실히 끊습니다.
Mirror 환경이라 `NetworkServer.Spawn` / `UnSpawn`을 풀 대여/반납과 함께 호출하도록 맞췄습니다.

---

## 실행 방법

**요구 사항** — Unity 6000.3.10f1 이상, Steam 클라이언트 실행 상태

```bash
git clone https://github.com/unm7925/FPS.git
```

1. Unity Hub에서 프로젝트를 엽니다 (패키지는 `Packages/manifest.json` 기준 자동 복원)
2. `Assets/00.Scenes/SampleScene.unity` (로비) 에서 실행
3. 싱글: 봇 매치 선택 / 멀티: 방 만들기 · 코드 참가 · 랜덤 매칭

> ⚠️ 저장소에는 **스크립트와 씬/프리팹 구성만** 포함되어 있습니다.
> 외부 구매 아트 에셋(`Assets/AssetsGame/`)은 재배포가 불가해 제외되어 있으므로,
> 클론 후 바로 실행하면 모델·애니메이션이 비어 있습니다.
> 실제 플레이는 [Releases](https://github.com/unm7925/FPS/releases)의 빌드를 이용해 주세요.

멀티플레이는 Steam P2P(FizzySteamworks)를 사용하므로 **양쪽 모두 Steam 로그인 상태**여야 합니다.

---

## 현재 상태

구현이 끝난 범위와 남은 범위를 명확히 적어 둡니다.

**동작하는 것**
- 1v1 / 2v2 라운드 매치 (싱글 · 멀티 모두)
- 사격 · 부위 피격 · 무기 교체/드랍 · 장전 동기화
- Steam 로비 매칭, 리더보드, 전적 저장/조회

**개선 예정**
- `null` 안전성 보강 — Mirror 환경에서 파괴된 `NetworkIdentity` 참조 방어
- `WeaponData.range` 미적용 등 하드코딩 상수 정리
- 이동/중력 로직 분리 (`PlayerController.Move`)
- 사망 애니메이션 및 킬로그 UI

---

<sub>1인 개발 · 2026.03 ~ 2026.04</sub>
