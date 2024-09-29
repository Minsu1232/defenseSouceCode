using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    public GameObject achievementPanel; // ���� �г�
    public Button achievementButton; // ���� ��ư
    public Transform achievementListContent; // ���� ����Ʈ�� ������ �θ� ��ü
    public GameObject achievementListItemPrefab; // ���� ����Ʈ ������ ������

    private void Start()
    {
        achievementButton.onClick.AddListener(ToggleAchievementPanel);
        PopulateAchievementList();
    }

    private void ToggleAchievementPanel()
    {
        achievementPanel.SetActive(!achievementPanel.activeSelf);
    }

    private void PopulateAchievementList()
    {
        foreach (Achievements achievement in AchievementsManager.Instance.achievementList)
        {
            GameObject item = Instantiate(achievementListItemPrefab, achievementListContent);
            item.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = achievement.name;
            item.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = $"Tier: {achievement.currentTier}/{achievement.maxTier} - Next Target: {achievement.targetCount}";
            item.transform.Find("RewardText").GetComponent<TextMeshProUGUI>().text = $"Reward Multiplier: {achievement.rewardMultiplier}";
        }
    }
}
