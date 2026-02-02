# KeyStorm Dungeon
<img width="832" height="470" alt="image" src="https://github.com/user-attachments/assets/a5ac79b0-d8ff-4b9e-a5bc-33276214f646" />
**KeyStorm Dungeon**은
독특한 전투 시스템을 가진 로그라이크 게임입니다.
플레이어는 던전에 진입해 끊임없이 몰려오는 적들과 맞서 싸우며,
키 조합을 활용한 공격과 아이템 빌드를 통해 매 판마다 다른 전략을 만들어갑니다.
도전할수록 새롭게 열리는 플레이 경험이
짧지만 강렬한 몰입을 선사합니다.

## 목차
1. [게임 소개](#게임-소개)
2. [주요기능](#주요기능)
3. [개발기간](#개발기간)
4. [기술스택](#기술스택)
5. [클라이언트 구조](#클라이언트-구조)
6. [팀원 구성](#팀원-구성)

## [게임 소개]
KeyStorm Dungeon
<img width="1200" height="900" alt="image" src="https://github.com/user-attachments/assets/5f335a91-e62f-475d-9116-969ee00d1c56" />
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/cb15f11e-c5fd-486b-9f32-025a94e98526" />
<img width="1500" height="1200" alt="image" src="https://github.com/user-attachments/assets/028ca932-b4cf-4bbe-937a-5f9c7c1a0313" />
<img width="621" height="288" alt="image" src="https://github.com/user-attachments/assets/b0a25927-6344-463c-8ec8-f5f61dc15527" />

- **장르** : 로그라이트, 액션, 어드벤처, 탑다운 슈팅
- **플랫폼** : PC

### 조작
- **UI 조작**: 마우스 좌클릭
- **공격**: Q, W, E, R, A, S, D, F
- **이동**: 방향키
- **폭탄**: Shift 입력 후 원하는 방향의 공격키 입력
- **액티브 아이템 발동** : Spacebar

### 게임 목표
- 던전을 탐험하며 몬스터를 처치하고 다음 구역으로 나아간다.
- 아이템과 능력을 획득해 자신만의 전투 방식을 완성한다.
- 반복되는 도전 속에서 더 깊은 던전에 도달해 최종 보스를 잡는 것을 목표로 한다.

#### 튜토리얼
<img width="828" height="462" alt="image" src="https://github.com/user-attachments/assets/d5536c30-88ad-4076-b870-e5f859ce8737" />
#### 아이템
<img width="897" height="460" alt="image" src="https://github.com/user-attachments/assets/0a401696-3f3a-490f-a1bf-344062e9bdef" />
#### 보스
<img width="832" height="465" alt="image" src="https://github.com/user-attachments/assets/2e95fe5d-0e4f-4195-8c0f-335d27531fe5" />
<img width="830" height="460" alt="image" src="https://github.com/user-attachments/assets/6b450762-c924-4203-93ca-3cb81bbe5ead" />
<img width="828" height="459" alt="image" src="https://github.com/user-attachments/assets/a04fa7a7-2c3a-4396-ac35-79171409b0a0" />

## 주요기능

- 기능 1
    - ScriptableObject 기반 튜토리얼 시스템
- 기능 2
    - 이벤트 기반 퀘스트 시스템
- 기능 3
    - 이중 딕셔너리 구조를 사용한 아이템 풀 관리
- 기능 4
    - FSM 사용 - 플레이어 및 몬스터의 상태 관리
- 기능 5
    - a* 알고리즘 사용 - 몬스터의 장애물 회피 기동
- 기능 6
    - 트리 기반 랜덤 무방향 그래프 사용 - 스테이지 랜덤 생성

## 개발기간
- 총 60 일   { 2025.12.05(금) ~ 2026.02.02(월) }

## [기술스택]

|Language|Engine|Version Control|IDE|Collaboration|
|--|--|--|--|--|
|[![My Skills](https://skillicons.dev/icons?i=cs&perline=1)](https://skillicons.dev)|[![My Skills](https://skillicons.dev/icons?i=unity&perline=1)](https://skillicons.dev)|[![My Skills](https://skillicons.dev/icons?i=git,github&perline=2)](https://skillicons.dev)|[![My Skills](https://skillicons.dev/icons?i=visualstudio&perline=1)](https://skillicons.dev)|[![My Skills](https://skillicons.dev/icons?i=notion,figma&perline=1)](https://skillicons.dev)|

## [클라이언트 구조]
<img width="854" height="927" alt="image" src="https://github.com/user-attachments/assets/a7670dd0-070d-4d0b-b135-42d032db6635" />


## [팀원 구성]
|기획, 맵|맵 생성, 플레이어|아이템|몬스터|UI/UX|
|:---:|:---:|:---:|:---:|:---:|
|<img src="https://avatars.githubusercontent.com/u/208426399?v=4" width="100"/>|<img src="https://avatars.githubusercontent.com/u/151013695?v=4" width="100"/>|<img src="https://avatars.githubusercontent.com/u/233680526?v=4" width="100"/>|<img src="https://avatars.githubusercontent.com/u/101345563?v=4" width="100">|<img src="https://avatars.githubusercontent.com/u/127909764?v=4" width="100"/>|
|[이승민](https://github.com/Seungmin0514)|[정유찬](https://github.com/youchan97)|[김문경](https://github.com/moon7441-dev)|[김하늘](https://github.com/Hagill)|[황준영](https://github.com/PeacefulKim)|
