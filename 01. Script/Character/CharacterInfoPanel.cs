using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoPanel : MonoBehaviour
{
    public GameObject infoPanel; // ĳ���� ������ ǥ���� �г�
    public TextMeshProUGUI nameText; // ĳ���� �̸�
    public TextMeshProUGUI levelText; // ĳ���� ����
    public TextMeshProUGUI heroGradeText; // ĳ���� ���
    public TextMeshProUGUI heroType; // ĳ���� Ÿ��
    public TextMeshProUGUI attackPowerText; // ���ݷ�
    public TextMeshProUGUI attackSpeedText; // ���ݼӵ�
    public TextMeshProUGUI attackRangeText; // ���ݹ���
    public TextMeshProUGUI criticalChanceText; // ġ��Ÿ Ȯ��
    public Button skillPanelButton; // ��ų �г��� ���� ��ư
    public SkillDetailPanel skillDetailPanel; // ��ų ���� ������ ǥ���� �г�
    public void UpdateCharacterInfo(CharacterData characterData, List<Skill> skills)
    {
        // ĳ���� ���� ������Ʈ (�ѱ���� ǥ��)
        nameText.text = $"�̸�: {characterData.heroName}";
        levelText.text = $"����: {Mathf.RoundToInt(characterData.level)}"; // ������ ������ ǥ��
        heroGradeText.text = $"���: {characterData.heroGrade.gradeName}";
        heroType.text = $"Ÿ��: {characterData.selectedType}";
        attackPowerText.text = $"���ݷ�: {Mathf.RoundToInt(characterData.attackPower)}"; // ���ݷ� ������ ǥ��
        attackSpeedText.text = $"���ݼӵ�: {Mathf.RoundToInt(characterData.attackSpeed)}"; // ���ݼӵ� ������ ǥ��
        attackRangeText.text = $"���ݹ���: {Mathf.RoundToInt(characterData.attackRange)}"; // ���ݹ��� ������ ǥ��
        criticalChanceText.text = $"ġ��Ÿ Ȯ��: {Mathf.RoundToInt(characterData.criticalChance * 100)}%"; // ġ��Ÿ Ȯ�� �ۼ�Ʈ�� ��ȯ�Ͽ� ������ ǥ��
        switch (characterData.selectedType)
        {
            case CharacterData.AttackType.Magic:
                heroType.text = "Ÿ��: ������";
                heroType.color = Color.blue;
                break;
            case CharacterData.AttackType.Archer:
                heroType.text = "Ÿ��: �ü�";
                heroType.color = Color.green;
                break;
            case CharacterData.AttackType.Warrior:
                heroType.text = "Ÿ��: ����";
                heroType.color = Color.red;
                break;
            default:
                heroType.text = "Ÿ��: �� �� ����";
                break;
        }
        switch (characterData.heroGrade.gradeName)
        {
            case "�⺻":
                heroGradeText.color = Color.white;
                break;
            case "���":
                heroGradeText.color = Color.blue;
                break;
            case "����":
                heroGradeText.color = Color.magenta;
                break;
            case "����":
                heroGradeText.color = Color.yellow;
                break;
            case "��ȭ":
                heroGradeText.color = new Color(0.984f, 0.396f, 0.267f); // ��Ȳ��
                break;
            case "����":
                heroGradeText.color = new Color(0.529f, 0.808f, 0.922f);
                break;

            default:
                heroGradeText.color = new Color(255, 255, 255);
                break;

        }
        // ��ų �г� ���� ��ư�� ������ �߰�
        skillPanelButton.onClick.RemoveAllListeners(); // ���� ������ ����
        skillPanelButton.onClick.AddListener(() => skillDetailPanel.ShowSkillInfo(skills));
        if (skillDetailPanel.detailPanel.activeSelf)
        {
            skillDetailPanel.detailPanel.gameObject.SetActive(false);
        }
        infoPanel.SetActive(true); // ĳ���� ���� �г� ǥ��

       
        
    }

    public void HideCharacterInfo()
    {
        // �г��� ����
        infoPanel.SetActive(false);
        skillDetailPanel.detailPanel.gameObject.SetActive(false);
    }
}
