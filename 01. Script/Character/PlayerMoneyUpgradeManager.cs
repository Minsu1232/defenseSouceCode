using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMoneyUpgradeManager : MonoBehaviour
{   
    
    public List<CharacterData> characterDataList; // 게임에 존재하는 모든 캐릭터 데이터
    public TextMeshProUGUI[] upgradeCostTexts; // 강화 비용을 표시할 텍스트 UI 배열
    public TextMeshProUGUI[] upgradeCountTexts; // 강화단계표시할 텍스트

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
        // FirebaseManager가 초기화될 때까지 대기
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // 한 프레임 대기
        }

        // 게임 시작 시 모든 캐릭터의 강화 상태를 로드하고, UI 업데이트
        for (int i = 0; i < characterDataList.Count; i++)
        {
            var characterData = characterDataList[i];
            if (characterData != null)
            {
                SaveLoadManager.Instance.LoadCharacterUpgrade(characterData);
                UpdateUpgradeCostUI(characterData, i); // 강화 비용 UI 업데이트
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

        // 등급에 따른 기본 강화 비용 결정
        int baseCost = 0;
        switch (characterData.heroGrade.gradeName)
        {
            case "기본":
                baseCost = 300;
                break;
            case "희귀":
                baseCost = 400;
                break;
            case "영웅":
                baseCost = 500;
                break;
            case "전설":
                baseCost = 800;
                break;
            case "신화":
                baseCost = 1200;
                break;
            case "태초":
                baseCost = 2000;
                break;
            default:
                Debug.LogError("알 수 없는 등급입니다.");
                return;
        }

        // 강화 비용 계산 (강화 횟수에 따라 증가)
        int upgradeCost = Mathf.RoundToInt(baseCost * Mathf.Pow(1.1f, characterData.upgradeCount));

        // 돈이 충분한지 확인하고 강화 진행
        if (PlayerMoneyManager.Instance.SpendMoney(upgradeCost))
        {
            // 강화된 스탯 계산
            characterData.attackPower *= 1.03f; // 공격력 3% 증가
            characterData.attackSpeed *= 1.05f; // 공격 속도 5% 증가
            characterData.criticalChance *= 1.02f; // 치명타 확률 2% 증가

            // 강화 횟수 증가
            characterData.upgradeCount++;

            // 강화된 상태를 Firebase에 저장
            SaveLoadManager.Instance.SaveCharacterUpgrade(characterData);

            // 강화 비용 UI 업데이트
            UpdateUpgradeCostUI(characterData, index);

            Debug.Log($"{characterData.heroName}가 {characterData.upgradeCount}번째 강화를 완료했습니다.");
        }
        else
        {
            Debug.Log("강화를 위한 돈이 부족합니다.");
        }
    }

    private void UpdateUpgradeCostUI(CharacterData characterData, int index)
    {
        if (index >= 0 && index < upgradeCostTexts.Length)
        {
            // 등급에 따른 기본 강화 비용 결정
            int baseCost = 0;
            switch (characterData.heroGrade.gradeName)
            {
                case "기본":
                    baseCost = 300;
                    break;
                case "희귀":
                    baseCost = 400;
                    break;
                case "영웅":
                    baseCost = 500;
                    break;
                case "전설":
                    baseCost = 800;
                    break;
                case "신화":
                    baseCost = 1200;
                    break;
                case "태초":
                    baseCost = 2000;
                    break;
                default:
                    Debug.LogError("알 수 없는 등급입니다.");
                    return;
            }

            // 강화 비용 계산 (강화 횟수에 따라 증가)
            int upgradeCost = Mathf.RoundToInt(baseCost * Mathf.Pow(1.1f, characterData.upgradeCount));

            // 텍스트 UI 업데이트
            upgradeCostTexts[index].text = $"{upgradeCost}";
            upgradeCountTexts[index].text = $"+ {characterDataList[index].upgradeCount}";
        }
        else
        {
            Debug.LogError($"Invalid index {index} for upgradeCostTexts array.");
        }
    }
}
