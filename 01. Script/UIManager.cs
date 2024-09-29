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
        // SaveLoadManager.Instance�� null�� �ƴϰ� �� ������ ���
        while (SaveLoadManager.Instance == null)
        {
            yield return null; // ������ ���
        }

        if (SaveLoadManager.Instance != null)
        {
            // SaveLoadManager�� �ʱ�ȭ�� �Ŀ� �ʿ��� �۾� ����
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
            case 0: // ù ��° ���� (��: ���� óġ)
                if (achivementName != null)
                    achivementName.text = achievement.name;

                if (achievementTier != null)
                    achievementTier.text = $"Ƽ��: {achievement.currentTier}/{achievement.maxTier}";

                if (achievementCurrentMonster != null)
                    achievementCurrentMonster.text = $"���� ��: {AchievementsManager.Instance.totalMonstersDefeated}/{achievement.targetCount}";

                // ���� ǥ��
                if (achievementCurrentMonster != null)
                    achievementCurrentMonster.text += $"\n����: {rewardExperience} XP, {rewardMoney} Gold";
                break;

            case 1: // �� ��° ���� (��: ���� Ŭ����)
                if (achivementName2 != null)
                    achivementName2.text = achievement.name;

                if (achievementTier2 != null)
                    achievementTier2.text = $"Ƽ��: {achievement.currentTier}/{achievement.maxTier}";

                if (achievementClearTime != null)
                    achievementClearTime.text = $"Ŭ���� Ƚ��: {AchievementsManager.Instance.totalClearTime}/{achievement.targetCount}";

                // ���� ǥ��
                if (achievementClearTime != null)
                    achievementClearTime.text += $"\n����: {rewardExperience} XP, {rewardMoney} Gold";
                break;

            default:
                Debug.LogWarning("�� �� ���� �ε���: " + index);
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
            case 0: // ù ��° ���� 
                if (achivementName != null)
                    achivementName.text = name;

                if (achievementTier != null)
                    achievementTier.text = $"Ƽ��: {tier}";

                if (achievementCurrentMonster != null)
                    achievementCurrentMonster.text = $"���� ��: {currentProgress}/{targetProgress} \n ����: {rewardExperience} XP, {rewardMoney} Gold";
                break;

            case 1: // �� ��° ����
                if (achivementName2 != null)
                    achivementName2.text = name;

                if (achievementTier2 != null)
                    achievementTier2.text = $"Ƽ��: {tier}";

                if (achievementClearTime != null)
                    achievementClearTime.text = $"Ŭ���� Ƚ��: {currentProgress}/{targetProgress} \n ����: {rewardExperience} XP, {rewardMoney} Gold";
                break;

            default:
                Debug.LogWarning("�� �� ���� �ε���: " + index);
                break;
        }
    }

}
