# [Unity 2D] WeaponMaster
무기를 조합하고 강화하며 몬스터 웨이브와 보스를 상대하는 2D 탑다운 뱀서라이크 게임입니다.

* **개발 기간**: 2026.06.17 ~ 2026.07.03
* **개발 인원**: 3명
* **플랫폼**: Windows
* **플레이 인원**: 싱글 플레이

## 목차
- [팀원 및 담당 분야](#1-팀원-및-담당-분야)
- [게임 진행](#2-게임-진행)
- [주요 구현](#3-주요-구현)
  - [무기별 공격과 성장](#무기별-공격과-성장)
  - [몬스터 공격 시퀀스와 보스 상태](#몬스터-공격-시퀀스와-보스-상태)
  - [다수 몬스터를 위한 탐색·재사용 구조](#다수-몬스터를-위한-탐색-및-재사용-구조)
  - [결과 화면](#결과-화면)
- [실행 안내](#4-실행-및-플레이-안내)
- [개발 환경](#5-개발-환경)

<br/>

## 1. 팀원 및 담당 분야
|프로필|이름|역할|GitHub|
|:---:|:---:|:---:|:---:|
|<img src="https://avatars.githubusercontent.com/u/172166404?v=4" width="100">|구한빈|팀장, 기획, 몬스터 및 보스 로직, 최적화, HUD UI |[@gyunabry](https://github.com/gyunabry)|
|<img src="https://avatars.githubusercontent.com/u/279920847?v=4" width="100">|최겸|플레이어 로직, 무기 및 성장 시스템, 무기 UI|[@Ritz55555](https://github.com/Ritz55555)|
|<img src="https://avatars.githubusercontent.com/u/285576312?v=4" width="100">|김기환|초기 몬스터 스폰 시스템, 맵 선택 카드 UI|[@Ritz55555](https://github.com/Las1118)|    

<br/>

## 2. 게임 진행
<table align="center">
  <tr>
    <td><img src="https://github.com/user-attachments/assets/60afff3a-98ce-4a55-9638-d597c596beec" width="100%"></td>
    <td><img src="https://github.com/user-attachments/assets/f0115033-0830-4350-aa5f-71f0f6442125" width="100%"></td>
  </tr>
  <tr>
    <td><img src="https://github.com/user-attachments/assets/1dc08c4a-3d72-4fad-bb82-4fdc44b00e0a" width="100%"></td>
    <td><img src="https://github.com/user-attachments/assets/43e879ac-2e26-4ab6-95a1-fa900ba895ff" width="100%"></td>
  </tr>
</table>


```text
스테이지·난이도 선택
  → 몬스터 웨이브 전투
  → 경험치 획득 및 레벨업
  → 무기 획득·강화
  → 보스 전투
  → 플레이 기록·무기 통계 확인
```

무기 조합과 강화로 전투를 이어가고, 적의 공격을 피하며 생존하는 게임입니다.  
보스 처치 후 결과 화면에서 플레이 시간, 레벨, 처치 수와 무기별 기록을 확인합니다.  

<br/>
  
## 3. 주요 구현

### [무기별 공격과 성장]
<img width="450" height="254" alt="0706_3" src="https://github.com/user-attachments/assets/eb742bcf-8c6f-4b98-842d-9b6995b53fe5" />

* 무기 종류에 따라 달라지는 공격 모션을 보여줍니다.

    
### [몬스터 공격 시퀀스와 보스 상태]
<img width="450" height="253" alt="Adobe Express - 원공격" src="https://github.com/user-attachments/assets/311fcb07-f50d-4e5a-8740-abb02e4d4426" />

* 개별 공격 설정과 공격 순서를 데이터로 분리했습니다.
* 일반 몬스터는 거리·쿨타임 조건을 확인한 뒤, 등록된 시퀀스에 따라 돌진과 원거리 공격을 실행합니다.
* 보스는 추적·공격·페이즈 전환·사망 상태를 구분해 제어합니다.

  
### [다수 몬스터를 위한 탐색 및 재사용 구조]
<img width="450" height="250" alt="image" src="https://github.com/user-attachments/assets/e8b747e7-d5f1-4841-bad6-e910a16b39b1" />

* **공간 분할:** 적을 셀별로 등록하고, 주변 셀의 적을 조회해 분리 이동과 반경 탐색에 사용합니다.
* **오브젝트 풀링:** 타입별 큐에 오브젝트를 보관하고 대여·반환하여 재사용합니다.

  
### [결과 화면]
<img width="450" height="254" alt="image" src="https://github.com/user-attachments/assets/b8ab0a28-9f99-453c-8d8d-db9bdb838229" />

* 스테이지, 난이도, 플레이 시간, 레벨, 처치 수를 표시합니다.
* 플레이어가 보유한 무기의 아이콘, 강화 단계, 누적 피해량을 표시합니다.

<br/>

## 4. 실행 및 플레이 안내
### 실행 방법
[**Download Latest Build**](https://github.com/gyunabry/GM7_TEAM01_WeaponMaster/releases/download/v1.0.0/WeaponMaster_v1.0.0_Windows.zip)
1. 위 링크를 클릭해 빌드 ZIP 파일을 다운로드 합니다.
2. 압축을 해제합니다.
3. 압축 해제한 폴더 내의 'WeaponMaster.exe'를 실행합니다.

### 조작법
| 동작 | 키 / 입력 |
| --- | --- |
| 캐릭터 이동 | `WASD` |
| 공격 | `자동 공격` |
| 일시정지 / 메뉴 | `ESC` |

### 플레이 방법

1. 타이틀 화면에서 게임을 시작하고 스테이지와 난이도를 선택합니다.
2. 몬스터를 처치하고 경험치를 모아 레벨을 올립니다.
3. 레벨업 시 무기를 획득하거나 강화합니다.
4. 적의 공격을 피하며 생존하고, 보스를 처치하면 클리어합니다.  

<br/>

## 5. 개발 환경
구분 | 사용 기술 |
| --- | --- |
| 엔진·언어 | Unity 6000.3.7f1, C# |
| 데이터 | ScriptableObject |
| 형상 관리 | Git, GitHub |
| 협업 | Notion, Discord |
