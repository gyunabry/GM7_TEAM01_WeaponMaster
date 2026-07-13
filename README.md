# [Unity 2D] WeaponMaster
다양한 무기를 사용하는 웨펀마스터의 일당백 전투

## 목차
- [게임 소개](#게임-소개)
- [핵심 기능 및 특징](#핵심-기능-및-특징)
- [개발 환경 및 기술 스택](#개발-환경-및-기술-스택)
- [팀원 소개](#팀원-소개)

## 1. 게임 소개
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

**WeaponMaster**는 2D 탑다운 뱀서라이크 게임입니다.
플레이어는 스테이지와 난이도를 선택한 뒤, 몰려오는 몬스터 웨이브를 버티며 경험치는 획득하고, 레벨업을 통해 무기를 획득하거나 강화합니다.
15분(900초)를 버티면 보스가 등장하며, 보스 처치 후 결과 팝업에서 플레이 기록과 무기 통계를 확인할 수 있습니다.

* **개발 기간**: 2026.06.17 ~ 2026.07.03
* **개발 인원**: 2명
* **플랫폼**: Windows
* **플레이 인원**: 1인
  
## 2. 핵심 기능 및 특징
### [무기 별 공격 모션]
<img width="450" height="254" alt="0706_3" src="https://github.com/user-attachments/assets/eb742bcf-8c6f-4b98-842d-9b6995b53fe5" />

* DOTween을 사용해 무기의 특색을 살린 공격 모션 구현
* 다양한 무기 조합을 통한 전략적인 전투

### [적의 다양한 공격 패턴]
<img width="450" height="253" alt="Adobe Express - 원공격" src="https://github.com/user-attachments/assets/311fcb07-f50d-4e5a-8740-abb02e4d4426" />

* 적의 다양한 공격들이 플레이어를 위협
* 플레이어에게는 회피 능력 요구

### [오브젝트 최적화]
* Object Pooling, Spatial Hash Grid를 사용한 최적화
* 다수의 적이 스폰되는 환경에서 프레임 저하 방지


## 3. 개발 환경 및 기술 스택
* **게임 엔진**: Unity6000.3.7f1
* **언어**: C#
* **형상 관리**: Git, GitHub
* **협업**: Notion, Discord

## 4. 팀원 소개
|프로필|이름|역할|GitHub|
|:---:|:---:|:---:|:---:|
|<img src="https://avatars.githubusercontent.com/u/172166404?v=4" width="100">|구한빈|팀장, 기획, 클라이언트 (몬스터 로직, 최적화, UI) |[@gyunabry](https://github.com/gyunabry)|
|<img src="https://avatars.githubusercontent.com/u/279920847?v=4" width="100">|최겸|클라이언트 (플레이어 로직, UI)|[@Ritz55555](https://github.com/Ritz55555)|
