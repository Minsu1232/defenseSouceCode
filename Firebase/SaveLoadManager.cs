using Firebase.Database;
using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Auth;
/// <summary>
/// ���̾�̽� �߾Ӱ���
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }
    private DatabaseReference databaseReference;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(WaitForFirebaseAndInitialize());
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator WaitForFirebaseAndInitialize()
    {
        // Firebase �ʱ�ȭ�� �Ϸ�� ������ ���
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // �� ������ ���
        }

        // Firebase �ʱ�ȭ �Ϸ� �� �����ͺ��̽� ���� ���
        databaseReference = FireBaseManager.Instance.GetDatabaseReference();

        if (databaseReference != null)
        {
            Debug.Log("Firebase Initialized in SaveLoadManager");
            LoadBuildings(); // Firebase �ʱ�ȭ �Ϸ� �� �ǹ� �ε�
        }
        else
        {
            Debug.LogError("Failed to get Firebase Database Reference in SaveLoadManager.");
        }
    }

    // ��ȭ ���� �޼���
    public void SaveMoney(int money)
    {
        // Firebase�� �� ����
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("money").SetValueAsync(money);
        Debug.Log("Money saved to Firebase: " + money);
    }

    // ��ȭ �ε� �޼���
    public void LoadMoney(Action<int> onMoneyLoaded)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("money").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load money from Firebase");
                onMoneyLoaded?.Invoke(0);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int money = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;
                onMoneyLoaded?.Invoke(money);
                Debug.Log("Money loaded from Firebase: " + money);
            }
        });
    }

    // ����ġ ���� �޼���
    public void SaveExperience(int experience)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("experience").SetValueAsync(experience);
        Debug.Log("Experience saved to Firebase: " + experience);
    }

    // ����ġ �ε� �޼���
    public void LoadExperience(Action onComplete = null) // �ݹ� �߰�
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("experience").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load experience from Firebase");
                Player.Instance.Experience = 0; // �ε� ���� �� �⺻�� ����
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int experience = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;
                Player.Instance.Experience = experience; // ������Ƽ�� �� ����
                Debug.Log("Experience loaded from Firebase: " + experience);
            }

            onComplete?.Invoke(); // �ε� �Ϸ� �� �ݹ� ����
        });
    }

    // ���� ���� �޼���
    public void SaveLevel(int level)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("level").SetValueAsync(level);
        Debug.Log("Level saved to Firebase: " + level);
    }

    // ���� �ε� �޼���
    public void LoadLevel(Action onComplete = null) // �ݹ� �߰�
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("level").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load level from Firebase");
                Player.Instance.Level = 1; // �⺻ ������ 1
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int level = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 1;
                Player.Instance.Level = level; // ������Ƽ�� �� ����
                Debug.Log("Level loaded from Firebase: " + level);
            }

            onComplete?.Invoke(); // �ε� �Ϸ� �� �ݹ� ����
        });
    }

    // ���� ���൵ ���� �޼���
    public void SaveGameProgress(int progress)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("gameProgress").SetValueAsync(progress);
        Debug.Log("Game progress saved to Firebase: " + progress);
    }

    // ���� ���൵ �ε� �޼���
    public void LoadGameProgress(Action<int> onProgressLoaded)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("gameProgress").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load game progress from Firebase");
                onProgressLoaded?.Invoke(0);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int progress = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;
                onProgressLoaded?.Invoke(progress);
                Debug.Log("Game progress loaded from Firebase: " + progress);
            }
        });
    }

    // �÷��̾� ��ġ ���� �޼���
    public void SavePlayerPosition(Vector3 position)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        string positionStr = $"{position.x},{position.y},{position.z}";
        databaseReference.Child("users").Child(userId).Child("playerPosition").SetValueAsync(positionStr);
        Debug.Log("Player position saved to Firebase: " + positionStr);
    }

    // �÷��̾� ��ġ �ε� �޼���
    public void LoadPlayerPosition(Action<Vector3> onPositionLoaded)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("playerPosition").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load player position from Firebase");
                onPositionLoaded?.Invoke(Vector3.zero);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string[] positionValues = snapshot.Value.ToString().Split(',');
                    Vector3 position = new Vector3(
                        float.Parse(positionValues[0]),
                        float.Parse(positionValues[1]),
                        float.Parse(positionValues[2]));
                    onPositionLoaded?.Invoke(position);
                    Debug.Log("Player position loaded from Firebase: " + position);
                }
                else
                {
                    onPositionLoaded?.Invoke(Vector3.zero);
                }
            }
        });
    }

    // Ư�� ��������Ʈ�� Ŭ���� Ƚ���� �������� �޼���
    public void GetClearedTimesForWaypoint(int waypointIndex, Action<int> onLoaded)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("clearedTimes").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load cleared times from Firebase");
                onLoaded?.Invoke(0);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int clearedTimes = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;
                onLoaded?.Invoke(clearedTimes);

                // �ش� ��������Ʈ�� �ǹ��� ��ġ�մϴ�.
                if (clearedTimes > 0)
                {
                    WaypointManager.Instance.PlaceBuildingAtWaypoint(waypointIndex - 1, clearedTimes);
                }
            }
        });
    }

    // Ư�� ��������Ʈ�� Ŭ������ ���·� ����ϴ� �޼���
    public void MarkWaypointAsCleared(int waypointIndex)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("cleared").SetValueAsync(true);
        Debug.Log("Waypoint marked as cleared: " + waypointIndex);
    }

    // Ư�� ��������Ʈ�� Ŭ����Ǿ����� Ȯ���ϴ� �޼���
    public void IsWaypointCleared(int waypointIndex, Action<bool> onLoaded)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("cleared").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to check if waypoint is cleared from Firebase");
                onLoaded?.Invoke(false);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                bool isCleared = snapshot.Exists && bool.Parse(snapshot.Value.ToString());
                onLoaded?.Invoke(isCleared);
            }
        });
    }

    // Ư�� ��������Ʈ�� ���̵��� �����ϴ� �޼���
    public void SetDifficultyForWaypoint(int waypointIndex, int difficulty)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        int cappedDifficulty = Mathf.Min(difficulty, 3);
        databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("difficulty").SetValueAsync(cappedDifficulty);
        Debug.Log("Waypoint difficulty set: " + cappedDifficulty);
    }

    // Ư�� ��������Ʈ�� ���̵��� �������� �޼���
    public void GetDifficultyForWaypoint(int waypointIndex, Action<int> onLoaded)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("difficulty").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load difficulty from Firebase");
                onLoaded?.Invoke(1); // �⺻ ���̵��� 1
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int difficulty = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 1;
                onLoaded?.Invoke(difficulty);
            }
        });
    }

    // Ư�� ��������Ʈ�� Ŭ������ Ƚ���� ������Ű�� �޼���
    public void IncrementClearedTimesForWaypoint(int waypointIndex)
    {
        GetClearedTimesForWaypoint(waypointIndex, currentTimes =>
        {
            int newClearedTimes = currentTimes + 1;
            string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            // Ŭ���� Ƚ���� Firebase�� ����
            databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("clearedTimes").SetValueAsync(newClearedTimes);

            // ��������Ʈ�� �ش��ϴ� ��ġ�� �ǹ��� �����մϴ�.
            WaypointManager.Instance.PlaceBuildingAtWaypoint(waypointIndex - 1, newClearedTimes);

            // ������ �ǹ� ���¸� ����
            databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("buildings").SetValueAsync(newClearedTimes);

            // ���̵� ���� ó��
            GetDifficultyForWaypoint(waypointIndex, currentDifficulty =>
            {
                int newDifficulty = Mathf.Min(currentDifficulty + 1, 3);
                databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("difficulty").SetValueAsync(newDifficulty);
            });
        });
    }
    // Ŭ���� Ƚ���� ������� ���� �ε�
    public void LoadBuildings()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // �� ��������Ʈ�� �ǹ� ���¸� �ҷ��ɴϴ�.
        for (int i = 0; i < 13; i++)
        {
            int waypointIndex = i;

            GetClearedTimesForWaypoint(waypointIndex, clearedTimes =>
            {
                if (clearedTimes > 0)
                {
                    // clearedTimes ��ŭ �ݺ��Ͽ� �� ��ġ�� ���� ����
                    for (int j = 1; j <= clearedTimes; j++)
                    {
                        WaypointManager.Instance.PlaceBuildingAtWaypoint(waypointIndex - 1, j);
                    }
                }
            });
        }
    }
    public void SaveCharacterUpgrade(CharacterData characterData)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        string characterKey = characterData.heroName;

        databaseReference.Child("users").Child(userId).Child("characters").Child(characterKey).Child("upgradeCount").SetValueAsync(characterData.upgradeCount);
        //databaseReference.Child("users").Child(userId).Child("characters").Child(characterKey).Child("attackPower").SetValueAsync(characterData.attackPower);
        //databaseReference.Child("users").Child(userId).Child("characters").Child(characterKey).Child("attackSpeed").SetValueAsync(characterData.attackSpeed);
        //databaseReference.Child("users").Child(userId).Child("characters").Child(characterKey).Child("criticalChance").SetValueAsync(characterData.criticalChance);

        Debug.Log($"Character {characterData.heroName} upgrade saved to Firebase.");
    }

    public void LoadCharacterUpgrade(CharacterData characterData)
    {
        if (characterData == null)
        {
            Debug.LogError("LoadCharacterUpgrade received null CharacterData.");
            return;
        }

        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("characters").Child(characterData.heroName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"Failed to load upgrade data for {characterData.heroName} from Firebase");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    characterData.upgradeCount = int.Parse(snapshot.Child("upgradeCount").Value.ToString());
                    //characterData.attackPower = float.Parse(snapshot.Child("attackPower").Value.ToString());
                    //characterData.attackSpeed = float.Parse(snapshot.Child("attackSpeed").Value.ToString());
                    //characterData.criticalChance = float.Parse(snapshot.Child("criticalChance").Value.ToString());
                }
            }
        });
    }
}
