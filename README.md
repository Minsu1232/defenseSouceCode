# 운이없다운이 게임 스크립트

이 저장소는 "운이없다운이" 게임의 **스크립트 파일들**만 포함되어 있습니다. Unity로 개발된 이 프로젝트의 일부로, 해당 스크립트들은 게임의 주요 로직을 처리하며, 코드가 어떤 구조로 짜여졌는지와 그 의도를 확인할 수 있습니다.

## 프로젝트 소개

- 이 프로젝트는 마블과 디펜스 요소를 결합한 **2D 픽셀 디펜스 게임**입니다. 운에 따라 게임이 진행되며, 다양한 전략을 통해 게임을 클리어할 수 있습니다.
- 스크립트들은 게임의 핵심 로직을 담고 있으며, 주석을 통해 의도를 쉽게 이해할 수 있도록 작성되었습니다.

## 실행 방법

이 저장소에는 **스크립트 파일들**만 포함되어 있으며, Unity 프로젝트의 전체 소스는 포함되지 않았습니다.



## 주요 스크립트 설명

- **Data폴더**: CharacterData는 스크립터블 오브젝트 스크립트, CharacterInfo는유닛들의 최상위클래스 입니다. CSV는 CharacterDataLoad로 읽어 옵니다.


- **Firebase폴더**: Firebase관련 스크립트들이 있습니다.


- **Heroes 폴더**: 모든 유닛 스크립트, 유닛 소환 및 강화 스크립트가 있습니다. CharacterInfo를 상속합니다.


- **Skill 폴더**: 스킬 관련 스크립트 폴더입니다. 각 고유 스킬들이 있으며, 기본적으로 스킬들은 skill을 상속받습니다. SkillBehavior는 스킬 프리팹에 할당 되어있고 실질적인 효과를 발휘하는 역할입니다.
	        스킬 정보는 CSV를 통해 관리하며 CSV는 SkillLoader로 읽어 옵니다.


- **CSV 폴더**:  사용된 CSV 복사본 입니다.


-** Item 폴더 **: 아이템 제작 스크립트 입니다.  ( 게임에서 아이템을 생각하면 이 부분도 CSV을 사용 했어야 했을거 같습니다.)

 
**코드를 설계한 이유는 기재된 기술문서에 있습니다.**