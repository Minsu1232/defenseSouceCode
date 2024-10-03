using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseDatabaseManager : MonoBehaviour
{
    private DatabaseReference reference;

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        ReadAchievements(); // 앱 시작 시 업적 데이터 로드
    }

    public void WriteAchievement(string achievementId, string achievementName)
    {
        Achievement achievement = new Achievement(achievementId, achievementName);
        string json = JsonUtility.ToJson(achievement);

        reference.Child("achievements").Child(achievementId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Achievement data written successfully.");
            }
            else
            {
                Debug.LogError("Failed to write achievement data: " + task.Exception);
            }
        });
    }

    public void ReadAchievements()
    {
        reference.Child("achievements").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    Debug.Log("Achievement data: " + childSnapshot.GetRawJsonValue());
                }
            }
            else
            {
                Debug.LogError("Failed to read achievement data: " + task.Exception);
            }
        });
    }
}

[System.Serializable]
public class Achievement
{
    public string achievementId;
    public string achievementName;

    public Achievement(string achievementId, string achievementName)
    {
        this.achievementId = achievementId;
        this.achievementName = achievementName;
    }
}


