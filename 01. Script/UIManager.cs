using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject achievementPanel;
    public Button achievementButton;
    public TextMeshProUGUI achivementName;
    public TextMeshProUGUI achievementTier;
    public TextMeshProUGUI achievementCurrentMonster;

    public TextMeshProUGUI achivementName2;
    public TextMeshProUGUI achievementTier2;
    public TextMeshProUGUI achievementClearTime;

    public GameObject playerGragePanel;
    public Button playerGradeButton;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(InitializeAndLoadData());
        achievementButton.onClick.AddListener(ToggleAchievementPanel);
        playerGradeButton.onClick.AddListener(TogglePlayerGragePanel);

        UpdateAllAchievementsUI();
    }

    private void ToggleAchievementPanel()
    {
        achievementPanel.SetActive(!achievementPanel.activeSelf);
    }
    private void TogglePlayerGragePanel()
    {
        playerGragePanel.SetActive(!playerGragePanel.activeSelf);
    }

    private IEnumerator InitializeAndLoadData()
    {
        // SaveLoadManager.Instance가 null이 아니게 될 때까지 대기
        while (SaveLoadManager.Instance == null)
        {
            yield return null; // 프레임 대기
        }

        if (SaveLoadManager.Instance != null)
        {
            // SaveLoadManager가 초기화된 후에 필요한 작업 수행
            Debug.Log("UIManager initialized and ready.");
        }
        else
        {
            Debug.LogError("SaveLoadManager.Instance is still null after waiting. Cannot load player data.");
        }
    }
    public void InitializeAchievementUI(int index, Achievements achievement)
    {
        var (rewardExperience, rewardMoney) = AchievementsManager.Instance.CalculateReward(achievement);

        switch (index)
        {
            case 0: // 첫 번째 업적 (예: 몬스터 처치)
                if (achivementName != null)
                    achivementName.text = achievement.name;

                if (achievementTier != null)
                    achievementTier.text = $"티어: {achievement.currentTier}/{achievement.maxTier}";

                if (achievementCurrentMonster != null)
                    achievementCurrentMonster.text = $"마릿 수: {AchievementsManager.Instance.totalMonstersDefeated}/{achievement.targetCount}";

                // 보상 표기
                if (achievementCurrentMonster != null)
                    achievementCurrentMonster.text += $"\n보상: {rewardExperience} XP, {rewardMoney} Gold";
                break;

            case 1: // 두 번째 업적 (예: 던전 클리어)
                if (achivementName2 != null)
                    achivementName2.text = achievement.name;

                if (achievementTier2 != null)
                    achievementTier2.text = $"티어: {achievement.currentTier}/{achievement.maxTier}";

                if (achievementClearTime != null)
                    achievementClearTime.text = $"클리어 횟수: {AchievementsManager.Instance.totalClearTime}/{achievement.targetCount}";

                // 보상 표기
                if (achievementClearTime != null)
                    achievementClearTime.text += $"\n보상: {rewardExperience} XP, {rewardMoney} Gold";
                break;

            default:
                Debug.LogWarning("알 수 없는 인덱스: " + index);
                break;
        }
    }
    public void UpdateAllAchievementsUI()
    {
        for (int i = 0; i < AchievementsManager.Instance.achievementList.Count; i++)
        {
            InitializeAchievementUI(i, AchievementsManager.Instance.achievementList[i]);
        }
    }


    public void UpdateAchievementUI(int index, string name, int tier, int currentProgress, int targetProgress)
    {
        Achievements achievement = AchievementsManager.Instance.achievementList[index];
        var (rewardExperience, rewardMoney) = AchievementsManager.Instance.CalculateReward(achievement);

        switch (index)
        {
            case 0: // 첫 번째 업적 
                if (achivementName != null)
                    achivementName.text = name;

                if (achievementTier != null)
                    achievementTier.text = $"티어: {tier}";

                if (achievementCurrentMonster != null)
                    achievementCurrentMonster.text = $"마릿 수: {currentProgress}/{targetProgress} \n 보상: {rewardExperience} XP, {rewardMoney} Gold";
                break;

            case 1: // 두 번째 업적
                if (achivementName2 != null)
                    achivementName2.text = name;

                if (achievementTier2 != null)
                    achievementTier2.text = $"티어: {tier}";

                if (achievementClearTime != null)
                    achievementClearTime.text = $"클리어 횟수: {currentProgress}/{targetProgress} \n 보상: {rewardExperience} XP, {rewardMoney} Gold";
                break;

            default:
                Debug.LogWarning("알 수 없는 인덱스: " + index);
                break;
        }
    }

}
