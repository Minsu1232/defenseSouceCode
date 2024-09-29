using Firebase.Database;
using Firebase.Auth;
using System.Collections;
using UnityEngine;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager Instance { get; private set; }

    public List<Achievements> achievementList = new List<Achievements>(); // Achievements ����Ʈ
    public int totalMonstersDefeated; // �� óġ�� ���� ��
    public int totalClearTime; // �� Ŭ������ ���潺
    private DatabaseReference databaseReference;
    private FirebaseUser user;

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

        StartCoroutine(WaitForFirebaseAndLoadAchievements());
    }

    private void InitializeAchievements()
    {
        // Firebase�� �����Ͱ� ���� ���� ȣ��˴ϴ�.
        achievementList = new List<Achievements>
    {
        new Achievements { name = "���� óġ", targetCount = 20, maxTier = 10, rewardMultiplier = 1.1f, currentTier = 1, isCompleted = false },
        new Achievements { name = "Ŭ���� Ƚ��", targetCount = 1, maxTier = 10, rewardMultiplier = 1.1f, currentTier = 1, isCompleted = false }
    };
    }

    private IEnumerator WaitForFirebaseAndLoadAchievements()
    {
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // Firebase �ʱ�ȭ �Ϸ� ���
        }

        user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("Firebase user is null.");
            yield break;
        }

        databaseReference = FireBaseManager.Instance.GetDatabaseReference();
        if (databaseReference == null)
        {
            Debug.LogError("Failed to get Firebase Database Reference.");
            yield break;
        }

        databaseReference = databaseReference.Child("users").Child(user.UserId).Child("achievements");

        databaseReference.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load achievements from Firebase");
                return;
            }

            if (!task.Result.Exists || task.Result.ChildrenCount == 0)
            {
                Debug.LogWarning("Achievements path does not exist. Initializing default values.");
                InitializeAchievements(); // �⺻ ���� �����ͷ� �ʱ�ȭ
                SaveAchievementsToFirebase(); // �ʱ�ȭ �� Firebase�� ����
            }
            else
            {
                LoadAchievementsFromFirebase(task.Result); // Firebase���� �����͸� �ε�
            }

            // UI �ʱ�ȭ
            UIManager.Instance.UpdateAllAchievementsUI();
        });
    }
    private void InitializeUI()
    {
        for (int i = 0; i < achievementList.Count; i++)
        {
            UIManager.Instance.InitializeAchievementUI(i, achievementList[i]);
        }
    }
    public void CheckAchievement(string achievementName, int progress)
    {
        // ���� �̸����� ���� ã��
        int index = achievementList.FindIndex(a => a.name == achievementName);

        // �ش� �̸��� ������ ���ų� �̹� �Ϸ�� ���, �޼��带 ����
        if (index == -1 || achievementList[index].isCompleted) return;

        Achievements achievement = achievementList[index];

        // ���� ���� ��Ȳ�� ������ ��ǥ�� �޼��ߴ��� Ȯ��
        if (progress >= achievement.targetCount)
        {
            // ���� ����
            GiveReward(achievement);

            // ���� ��ǥ ���� �� ���� ���� ����
            if (achievement.name == "���� óġ")
            {
                achievement.targetCount *= 2; // ���� ��ǥ�� 2��� ����
            }
            else if (achievement.name == "Ŭ���� Ƚ��")
            {
                achievement.targetCount += 2; // ���� ��ǥ�� 2 ����
            }

            // Ƽ�� ����
            achievement.currentTier++;

            // �ִ� Ƽ� �������� �ʾҴٸ� ���� ���� ����
            if (achievement.currentTier <= achievement.maxTier)
            {
                achievement.rewardMultiplier *= 1.1f;
            }

            // ������ �ִ� Ƽ� ���������� ���� �Ϸ�� ����
            if (achievement.currentTier >= achievement.maxTier)
            {
                achievement.isCompleted = true;
            }

            // Firebase�� ���� ���� ����
           
        }
        SaveAchievementsToFirebase();
        // UI ������Ʈ (���� �ε���, ���� �̸�, Ƽ��, ���� ��Ȳ, ��ǥ)
        UIManager.Instance.UpdateAchievementUI(
            index,               // ���� �ε���
            achievement.name,    // ���� �̸�
            achievement.currentTier, // ���� Ƽ��
            progress,            // ���� ���� ��Ȳ
            achievement.targetCount  // ��ǥ
        );
    }

    private void GiveReward(Achievements achievement)
    {
        // ������ ���
        var (rewardExperience, rewardMoney) = CalculateReward(achievement);

        // ������ ����
        Debug.Log($"Achievement {achievement.name} completed! Reward: {rewardExperience} XP, {rewardMoney} Gold");
        Player.Instance.AddExperience(rewardExperience);
        PlayerMoneyManager.Instance.AddMoney(rewardMoney);
    }
    public (int experience, int money) CalculateReward(Achievements achievement)
    {
        // ���� ������ ������ ������ ���
        int rewardExperience = Mathf.RoundToInt(achievement.baseExperience * achievement.rewardMultiplier * achievement.currentTier);
        int rewardMoney = Mathf.RoundToInt(achievement.baseMoney * achievement.rewardMultiplier * achievement.currentTier);

        return (rewardExperience, rewardMoney);

     
    }
    private void SaveAchievementsToFirebase()
    {
        // ���� ����
        foreach (var achievement in achievementList)
        {
            string json = JsonUtility.ToJson(achievement);
            databaseReference.Child("achievements").Child(achievement.name).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"Achievement '{achievement.name}' saved successfully.");
                }
                else
                {
                    Debug.LogError($"Failed to save achievement '{achievement.name}': " + task.Exception);
                }
            });
        }

        // totalMonstersDefeated ����
        databaseReference.Child("achievements").Child("���� óġ").Child("totalMonstersDefeated").SetValueAsync(totalMonstersDefeated).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("totalMonstersDefeated saved successfully.");
            }
            else
            {
                Debug.LogError("Failed to save totalMonstersDefeated: " + task.Exception);
            }
        });

        databaseReference.Child("achievements").Child("Ŭ���� Ƚ��").Child("totalClearTime").SetValueAsync(totalClearTime).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("totalMonstersDefeated saved successfully.");
            }
            else
            {
                Debug.LogError("Failed to save totalMonstersDefeated: " + task.Exception);
            }
        });
    }

    private void LoadAchievementsFromFirebase(DataSnapshot snapshot)
    {
        // ���� ����Ʈ�� �ʱ�ȭ (���� �����͸� ��� ����ϴ�)
        achievementList.Clear();

        // Firebase���� ���� �����͸� �ε��Ͽ� ����Ʈ�� �߰�
        List<Achievements> loadedAchievements = new List<Achievements>();

        foreach (DataSnapshot childSnapshot in snapshot.Child("achievements").Children)
        {
            string json = childSnapshot.GetRawJsonValue();
            Achievements loadedAchievement = JsonUtility.FromJson<Achievements>(json);

            // 'Enemies Defeated'�� �ƴ� ������ �߰��ϵ��� ���͸�
            if (loadedAchievement.name != "Enemies Defeated")
            {
                loadedAchievements.Add(loadedAchievement);
            }
            else
            {
                Debug.LogWarning("Enemies Defeated ������ ���õ˴ϴ�.");
            }

        }

        // �̸� �������� ����
        loadedAchievements.Sort((a, b) => string.Compare(a.name, b.name));

        // ���ĵ� �����͸� achievementList�� �߰�
        achievementList.AddRange(loadedAchievements);

        // totalMonstersDefeated �ҷ�����
        var monsterKillAchievement = snapshot.Child("achievements").Child("���� óġ");
        if (monsterKillAchievement.Exists && monsterKillAchievement.Child("totalMonstersDefeated").Exists)
        {
            totalMonstersDefeated = int.Parse(monsterKillAchievement.Child("totalMonstersDefeated").Value.ToString());
            Debug.Log("totalMonstersDefeated loaded: " + totalMonstersDefeated);
        }
        else
        {
            Debug.LogWarning("'���� óġ' path or totalMonstersDefeated does not exist in Firebase.");
        }
        // totalClearTime �ҷ�����
        var totalClearTime_ = snapshot.Child("achievements").Child("Ŭ���� Ƚ��");
        if (totalClearTime_.Exists && totalClearTime_.Child("totalClearTime").Exists)
        {
            totalClearTime = int.Parse(totalClearTime_.Child("totalClearTime").Value.ToString());
            Debug.Log("totalMonstersDefeated loaded: " + totalClearTime);
        }
        else
        {
            Debug.LogWarning("'���� óġ' path or totalMonstersDefeated does not exist in Firebase.");
        }
        // UI �ʱ�ȭ (Firebase���� �����͸� �ҷ��� ��)
        UIManager.Instance.UpdateAllAchievementsUI();
        Debug.Log("Achievements loaded from Firebase and UI updated.");
    }
}

