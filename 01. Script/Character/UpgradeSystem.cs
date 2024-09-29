using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public float upgradeMultiplier = 1.05f; // 강화 시 적용될 스탯 증가율 (5% 증가)

    private Dictionary<CharacterData.AttackType, int> upgradeLevels = new Dictionary<CharacterData.AttackType, int>()
    {
        { CharacterData.AttackType.Warrior, 1 },
        { CharacterData.AttackType.Archer, 1 },
        { CharacterData.AttackType.Magic, 1 }
    };

    public TextMeshProUGUI warriorCostText; // 전사 강화 비용 텍스트
    public TextMeshProUGUI archerCostText; // 궁수 강화 비용 텍스트
    public TextMeshProUGUI magicCostText; // 마법사 강화 비용 텍스트

    private void Start()
    {
        UpdateUpgradeCostUI(); // 초기화 시 UI 업데이트
    }

    public void Upgrade(CharacterData.AttackType attackType)
    {
        int cost = upgradeLevels[attackType] * 10;

        if (MoneyManager.Instance.SpendMoney(cost)) // 강화 비용을 레벨에 따라 결정
        {
            upgradeLevels[attackType]++; // 레벨 증가

            foreach (var character in FindObjectsOfType<CharacterInfo>())
            {
                if (character.AttackType == attackType)
                {
                    character.LevelUp(); // 레벨 업과 스탯 증가
                }
            }

            Debug.Log($"{attackType} units have been leveled up to level {upgradeLevels[attackType]}!");

            // 강화 후 UI 업데이트
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
