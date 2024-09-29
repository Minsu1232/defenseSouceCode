using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SkillDetailPanel : MonoBehaviour
{
    public GameObject detailPanel; // 스킬 세부 정보 패널
    public TextMeshProUGUI skillInfoText; // 일반 스킬 정보를 담을 텍스트
    public TextMeshProUGUI manaSkillInfoText; // 마나 스킬 정보를 담을 텍스트 (마나 스킬이 있을 경우만)

    // 스킬 정보를 업데이트하고 패널을 활성화
    public void ShowSkillInfo(List<Skill> skills)
    {
        skillInfoText.text = ""; // 기존 스킬 정보 초기화
        manaSkillInfoText.text = ""; // 마나 스킬 정보 초기화
        bool hasManaSkill = false; // 마나 스킬이 존재하는지 확인

        foreach (Skill skill in skills)
        {
            // 마나 스킬일 경우, 일반 스킬 목록에 추가하지 않고 별도로 처리
            if (skill is ManaSkill manaSkill)
            {
                hasManaSkill = true; // 마나 스킬이 존재함을 표시
                manaSkillInfoText.text = $"마나 스킬: {manaSkill.skillName}\n" +
                                         $"설명: {manaSkill.skillDescription}\n" +
                                         $"슬로우 효과 여부: {(manaSkill.hasSlowEffect ? "O" : "X")}\n" +
                                         $"방어력 감소 여부: {(manaSkill.hasDefenseReduction ? "O" : "X")}\n";
            }
            else
            {
                // 일반 스킬 정보 추가
                skillInfoText.text += $"스킬: {skill.skillName}\n" +
                                      $"설명: {skill.skillDescription}\n" +
                                      $"발동 확률: {Mathf.RoundToInt(skill.skillProbability * 100)}%\n" +
                                      $"슬로우 효과 여부: {(skill.hasSlowEffect ? "O" : "X")}\n" +
                                      $"방어력 감소 여부: {(skill.hasDefenseReduction ? "O" : "X")}\n\n";
            }
        }
        if (hasManaSkill)
        {
            skillInfoText.enableWordWrapping = true; // 줄바꿈 활성화
        }
        else
        {
            skillInfoText.enableWordWrapping = false; // 줄바꿈 비활성화 (오버플로우 유지)
            manaSkillInfoText.text = "";
        }
    

        detailPanel.SetActive(true); // 패널을 활성화하여 표시
    }

    public void HideSkillDetail()
    {
        detailPanel.SetActive(false); // 패널 숨기기
    }
}