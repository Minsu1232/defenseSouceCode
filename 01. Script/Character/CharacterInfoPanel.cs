using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoPanel : MonoBehaviour
{
    public GameObject infoPanel; // 캐릭터 정보를 표시할 패널
    public TextMeshProUGUI nameText; // 캐릭터 이름
    public TextMeshProUGUI levelText; // 캐릭터 레벨
    public TextMeshProUGUI heroGradeText; // 캐릭터 등급
    public TextMeshProUGUI heroType; // 캐릭터 타입
    public TextMeshProUGUI attackPowerText; // 공격력
    public TextMeshProUGUI attackSpeedText; // 공격속도
    public TextMeshProUGUI attackRangeText; // 공격범위
    public TextMeshProUGUI criticalChanceText; // 치명타 확률
    public Button skillPanelButton; // 스킬 패널을 여는 버튼
    public SkillDetailPanel skillDetailPanel; // 스킬 세부 정보를 표시할 패널
    public void UpdateCharacterInfo(CharacterData characterData, List<Skill> skills)
    {
        // 캐릭터 정보 업데이트 (한국어로 표기)
        nameText.text = $"이름: {characterData.heroName}";
        levelText.text = $"레벨: {Mathf.RoundToInt(characterData.level)}"; // 레벨을 정수로 표시
        heroGradeText.text = $"등급: {characterData.heroGrade.gradeName}";
        heroType.text = $"타입: {characterData.selectedType}";
        attackPowerText.text = $"공격력: {Mathf.RoundToInt(characterData.attackPower)}"; // 공격력 정수로 표시
        attackSpeedText.text = $"공격속도: {Mathf.RoundToInt(characterData.attackSpeed)}"; // 공격속도 정수로 표시
        attackRangeText.text = $"공격범위: {Mathf.RoundToInt(characterData.attackRange)}"; // 공격범위 정수로 표시
        criticalChanceText.text = $"치명타 확률: {Mathf.RoundToInt(characterData.criticalChance * 100)}%"; // 치명타 확률 퍼센트로 변환하여 정수로 표시
        switch (characterData.selectedType)
        {
            case CharacterData.AttackType.Magic:
                heroType.text = "타입: 마법사";
                heroType.color = Color.blue;
                break;
            case CharacterData.AttackType.Archer:
                heroType.text = "타입: 궁수";
                heroType.color = Color.green;
                break;
            case CharacterData.AttackType.Warrior:
                heroType.text = "타입: 전사";
                heroType.color = Color.red;
                break;
            default:
                heroType.text = "타입: 알 수 없음";
                break;
        }
        switch (characterData.heroGrade.gradeName)
        {
            case "기본":
                heroGradeText.color = Color.white;
                break;
            case "희귀":
                heroGradeText.color = Color.blue;
                break;
            case "영웅":
                heroGradeText.color = Color.magenta;
                break;
            case "전설":
                heroGradeText.color = Color.yellow;
                break;
            case "신화":
                heroGradeText.color = new Color(0.984f, 0.396f, 0.267f); // 주황색
                break;
            case "태초":
                heroGradeText.color = new Color(0.529f, 0.808f, 0.922f);
                break;

            default:
                heroGradeText.color = new Color(255, 255, 255);
                break;

        }
        // 스킬 패널 열기 버튼에 리스너 추가
        skillPanelButton.onClick.RemoveAllListeners(); // 기존 리스너 제거
        skillPanelButton.onClick.AddListener(() => skillDetailPanel.ShowSkillInfo(skills));
        if (skillDetailPanel.detailPanel.activeSelf)
        {
            skillDetailPanel.detailPanel.gameObject.SetActive(false);
        }
        infoPanel.SetActive(true); // 캐릭터 정보 패널 표시

       
        
    }

    public void HideCharacterInfo()
    {
        // 패널을 숨김
        infoPanel.SetActive(false);
        skillDetailPanel.detailPanel.gameObject.SetActive(false);
    }
}
