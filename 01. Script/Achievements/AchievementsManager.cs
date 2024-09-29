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

    public List<Achievements> achievementList = new List<Achievements>(); // Achievements 리스트
    public int totalMonstersDefeated; // 총 처치된 몬스터 수
    public int totalClearTime; // 총 클리어한 디펜스
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
        // Firebase에 데이터가 없을 때만 호출됩니다.
        achievementList = new List<Achievements>
    {
        new Achievements { name = "몬스터 처치", targetCount = 20, maxTier = 10, rewardMultiplier = 1.1f, currentTier = 1, isCompleted = false },
        new Achievements { name = "클리어 횟수", targetCount = 1, maxTier = 10, rewardMultiplier = 1.1f, currentTier = 1, isCompleted = false }
    };
    }

    private IEnumerator WaitForFirebaseAndLoadAchievements()
    {
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // Firebase 초기화 완료 대기
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
                InitializeAchievements(); // 기본 업적 데이터로 초기화
                SaveAchievementsToFirebase(); // 초기화 후 Firebase에 저장
            }
            else
            {
                LoadAchievementsFromFirebase(task.Result); // Firebase에서 데이터를 로드
            }

            // UI 초기화
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
        // 업적 이름으로 업적 찾기
        int index = achievementList.FindIndex(a => a.name == achievementName);

        // 해당 이름의 업적이 없거나 이미 완료된 경우, 메서드를 종료
        if (index == -1 || achievementList[index].isCompleted) return;

        Achievements achievement = achievementList[index];

        // 현재 진행 상황이 업적의 목표를 달성했는지 확인
        if (progress >= achievement.targetCount)
        {
            // 보상 지급
            GiveReward(achievement);

            // 다음 목표 설정 및 보상 배율 증가
            if (achievement.name == "몬스터 처치")
            {
                achievement.targetCount *= 2; // 다음 목표는 2배로 증가
            }
            else if (achievement.name == "클리어 횟수")
            {
                achievement.targetCount += 2; // 다음 목표는 2 증가
            }

            // 티어 증가
            achievement.currentTier++;

            // 최대 티어에 도달하지 않았다면 보상 배율 증가
            if (achievement.currentTier <= achievement.maxTier)
            {
                achievement.rewardMultiplier *= 1.1f;
            }

            // 업적이 최대 티어에 도달했으면 업적 완료로 설정
            if (achievement.currentTier >= achievement.maxTier)
            {
                achievement.isCompleted = true;
            }

            // Firebase에 업적 상태 저장
           
        }
        SaveAchievementsToFirebase();
        // UI 업데이트 (업적 인덱스, 업적 이름, 티어, 진행 상황, 목표)
        UIManager.Instance.UpdateAchievementUI(
            index,               // 업적 인덱스
            achievement.name,    // 업적 이름
            achievement.currentTier, // 현재 티어
            progress,            // 현재 진행 상황
            achievement.targetCount  // 목표
        );
    }

    private void GiveReward(Achievements achievement)
    {
        // 리워드 계산
        var (rewardExperience, rewardMoney) = CalculateReward(achievement);

        // 리워드 지급
        Debug.Log($"Achievement {achievement.name} completed! Reward: {rewardExperience} XP, {rewardMoney} Gold");
        Player.Instance.AddExperience(rewardExperience);
        PlayerMoneyManager.Instance.AddMoney(rewardMoney);
    }
    public (int experience, int money) CalculateReward(Achievements achievement)
    {
        // 보상 배율을 적용한 리워드 계산
        int rewardExperience = Mathf.RoundToInt(achievement.baseExperience * achievement.rewardMultiplier * achievement.currentTier);
        int rewardMoney = Mathf.RoundToInt(achievement.baseMoney * achievement.rewardMultiplier * achievement.currentTier);

        return (rewardExperience, rewardMoney);

     
    }
    private void SaveAchievementsToFirebase()
    {
        // 업적 저장
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

        // totalMonstersDefeated 저장
        databaseReference.Child("achievements").Child("몬스터 처치").Child("totalMonstersDefeated").SetValueAsync(totalMonstersDefeated).ContinueWithOnMainThread(task =>
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

        databaseReference.Child("achievements").Child("클리어 횟수").Child("totalClearTime").SetValueAsync(totalClearTime).ContinueWithOnMainThread(task =>
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
        // 업적 리스트를 초기화 (이전 데이터를 모두 지웁니다)
        achievementList.Clear();

        // Firebase에서 업적 데이터를 로드하여 리스트에 추가
        List<Achievements> loadedAchievements = new List<Achievements>();

        foreach (DataSnapshot childSnapshot in snapshot.Child("achievements").Children)
        {
            string json = childSnapshot.GetRawJsonValue();
            Achievements loadedAchievement = JsonUtility.FromJson<Achievements>(json);

            // 'Enemies Defeated'가 아닌 업적만 추가하도록 필터링
            if (loadedAchievement.name != "Enemies Defeated")
            {
                loadedAchievements.Add(loadedAchievement);
            }
            else
            {
                Debug.LogWarning("Enemies Defeated 업적은 무시됩니다.");
            }

        }

        // 이름 기준으로 정렬
        loadedAchievements.Sort((a, b) => string.Compare(a.name, b.name));

        // 정렬된 데이터를 achievementList에 추가
        achievementList.AddRange(loadedAchievements);

        // totalMonstersDefeated 불러오기
        var monsterKillAchievement = snapshot.Child("achievements").Child("몬스터 처치");
        if (monsterKillAchievement.Exists && monsterKillAchievement.Child("totalMonstersDefeated").Exists)
        {
            totalMonstersDefeated = int.Parse(monsterKillAchievement.Child("totalMonstersDefeated").Value.ToString());
            Debug.Log("totalMonstersDefeated loaded: " + totalMonstersDefeated);
        }
        else
        {
            Debug.LogWarning("'몬스터 처치' path or totalMonstersDefeated does not exist in Firebase.");
        }
        // totalClearTime 불러오기
        var totalClearTime_ = snapshot.Child("achievements").Child("클리어 횟수");
        if (totalClearTime_.Exists && totalClearTime_.Child("totalClearTime").Exists)
        {
            totalClearTime = int.Parse(totalClearTime_.Child("totalClearTime").Value.ToString());
            Debug.Log("totalMonstersDefeated loaded: " + totalClearTime);
        }
        else
        {
            Debug.LogWarning("'몬스터 처치' path or totalMonstersDefeated does not exist in Firebase.");
        }
        // UI 초기화 (Firebase에서 데이터를 불러온 후)
        UIManager.Instance.UpdateAllAchievementsUI();
        Debug.Log("Achievements loaded from Firebase and UI updated.");
    }
}

