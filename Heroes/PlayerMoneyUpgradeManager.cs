using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMoneyUpgradeManager : MonoBehaviour
{   
    
    public List<CharacterData> characterDataList; // ���ӿ� �����ϴ� ��� ĳ���� ������
    public TextMeshProUGUI[] upgradeCostTexts; // ��ȭ ����� ǥ���� �ؽ�Ʈ UI �迭
    public TextMeshProUGUI[] upgradeCountTexts; // ��ȭ�ܰ�ǥ���� �ؽ�Ʈ

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        StartCoroutine(WaitForFirebaseAndLoadUpgrades());
    }

    private IEnumerator WaitForFirebaseAndLoadUpgrades()
    {
        // FirebaseManager�� �ʱ�ȭ�� ������ ���
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // �� ������ ���
        }

        // ���� ���� �� ��� ĳ������ ��ȭ ���¸� �ε��ϰ�, UI ������Ʈ
        for (int i = 0; i < characterDataList.Count; i++)
        {
            var characterData = characterDataList[i];
            if (characterData != null)
            {
                SaveLoadManager.Instance.LoadCharacterUpgrade(characterData);
                UpdateUpgradeCostUI(characterData, i); // ��ȭ ��� UI ������Ʈ
            }
            else
            {
                Debug.LogError("CharacterData is null in characterDataList.");
            }
        }
    }

    public void UpgradeUnit(CharacterData characterData)
    {
        int index = characterDataList.IndexOf(characterData);

        if (index == -1)
        {
            Debug.LogError("CharacterData not found in characterDataList.");
            return;
        }

        // ��޿� ���� �⺻ ��ȭ ��� ����
        int baseCost = 0;
        switch (characterData.heroGrade.gradeName)
        {
            case "�⺻":
                baseCost = 300;
                break;
            case "���":
                baseCost = 400;
                break;
            case "����":
                baseCost = 500;
                break;
            case "����":
                baseCost = 800;
                break;
            case "��ȭ":
                baseCost = 1200;
                break;
            case "����":
                baseCost = 2000;
                break;
            default:
                Debug.LogError("�� �� ���� ����Դϴ�.");
                return;
        }

        // ��ȭ ��� ��� (��ȭ Ƚ���� ���� ����)
        int upgradeCost = Mathf.RoundToInt(baseCost * Mathf.Pow(1.1f, characterData.upgradeCount));

        // ���� ������� Ȯ���ϰ� ��ȭ ����
        if (PlayerMoneyManager.Instance.SpendMoney(upgradeCost))
        {
            // ��ȭ�� ���� ���
            characterData.attackPower *= 1.03f; // ���ݷ� 3% ����
            characterData.attackSpeed *= 1.05f; // ���� �ӵ� 5% ����
            characterData.criticalChance *= 1.02f; // ġ��Ÿ Ȯ�� 2% ����

            // ��ȭ Ƚ�� ����
            characterData.upgradeCount++;

            // ��ȭ�� ���¸� Firebase�� ����
            SaveLoadManager.Instance.SaveCharacterUpgrade(characterData);

            // ��ȭ ��� UI ������Ʈ
            UpdateUpgradeCostUI(characterData, index);

            Debug.Log($"{characterData.heroName}�� {characterData.upgradeCount}��° ��ȭ�� �Ϸ��߽��ϴ�.");
        }
        else
        {
            Debug.Log("��ȭ�� ���� ���� �����մϴ�.");
        }
    }

    private void UpdateUpgradeCostUI(CharacterData characterData, int index)
    {
        if (index >= 0 && index < upgradeCostTexts.Length)
        {
            // ��޿� ���� �⺻ ��ȭ ��� ����
            int baseCost = 0;
            switch (characterData.heroGrade.gradeName)
            {
                case "�⺻":
                    baseCost = 300;
                    break;
                case "���":
                    baseCost = 400;
                    break;
                case "����":
                    baseCost = 500;
                    break;
                case "����":
                    baseCost = 800;
                    break;
                case "��ȭ":
                    baseCost = 1200;
                    break;
                case "����":
                    baseCost = 2000;
                    break;
                default:
                    Debug.LogError("�� �� ���� ����Դϴ�.");
                    return;
            }

            // ��ȭ ��� ��� (��ȭ Ƚ���� ���� ����)
            int upgradeCost = Mathf.RoundToInt(baseCost * Mathf.Pow(1.1f, characterData.upgradeCount));

            // �ؽ�Ʈ UI ������Ʈ
            upgradeCostTexts[index].text = $"{upgradeCost}";
            upgradeCountTexts[index].text = $"+ {characterDataList[index].upgradeCount}";
        }
        else
        {
            Debug.LogError($"Invalid index {index} for upgradeCostTexts array.");
        }
    }
}
