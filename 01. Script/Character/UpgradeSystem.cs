using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public float upgradeMultiplier = 1.05f; // ��ȭ �� ����� ���� ������ (5% ����)

    private Dictionary<CharacterData.AttackType, int> upgradeLevels = new Dictionary<CharacterData.AttackType, int>()
    {
        { CharacterData.AttackType.Warrior, 1 },
        { CharacterData.AttackType.Archer, 1 },
        { CharacterData.AttackType.Magic, 1 }
    };

    public TextMeshProUGUI warriorCostText; // ���� ��ȭ ��� �ؽ�Ʈ
    public TextMeshProUGUI archerCostText; // �ü� ��ȭ ��� �ؽ�Ʈ
    public TextMeshProUGUI magicCostText; // ������ ��ȭ ��� �ؽ�Ʈ

    private void Start()
    {
        UpdateUpgradeCostUI(); // �ʱ�ȭ �� UI ������Ʈ
    }

    public void Upgrade(CharacterData.AttackType attackType)
    {
        int cost = upgradeLevels[attackType] * 10;

        if (MoneyManager.Instance.SpendMoney(cost)) // ��ȭ ����� ������ ���� ����
        {
            upgradeLevels[attackType]++; // ���� ����

            foreach (var character in FindObjectsOfType<CharacterInfo>())
            {
                if (character.AttackType == attackType)
                {
                    character.LevelUp(); // ���� ���� ���� ����
                }
            }

            Debug.Log($"{attackType} units have been leveled up to level {upgradeLevels[attackType]}!");

            // ��ȭ �� UI ������Ʈ
            UpdateUpgradeCostUI();
        }
        else
        {
            Debug.Log("Not enough gold for upgrade.");
        }
    }

    public int GetUpgradeLevel(CharacterData.AttackType attackType)
    {
        return upgradeLevels[attackType];
    }

    private void UpdateUpgradeCostUI()
    {
        warriorCostText.text = $"{upgradeLevels[CharacterData.AttackType.Warrior] * 10}";
        archerCostText.text = $"{upgradeLevels[CharacterData.AttackType.Archer] * 10}";
        magicCostText.text = $"{upgradeLevels[CharacterData.AttackType.Magic] * 10}";
    }
}
