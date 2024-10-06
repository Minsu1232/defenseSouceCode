using Firebase.Database;
using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Auth;
/// <summary>
/// 파이어베이스 중앙관리
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
        // Firebase 초기화가 완료될 때까지 대기
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // 한 프레임 대기
        }

        // Firebase 초기화 완료 후 데이터베이스 참조 얻기
        databaseReference = FireBaseManager.Instance.GetDatabaseReference();

        if (databaseReference != null)
        {
            Debug.Log("Firebase Initialized in SaveLoadManager");
            LoadBuildings(); // Firebase 초기화 완료 후 건물 로드
        }
        else
        {
            Debug.LogError("Failed to get Firebase Database Reference in SaveLoadManager.");
        }
    }

    // 재화 저장 메서드
    public void SaveMoney(int money)
    {
        // Firebase에 돈 저장
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("money").SetValueAsync(money);
        Debug.Log("Money saved to Firebase: " + money);
    }

    // 재화 로드 메서드
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

    // 경험치 저장 메서드
    public void SaveExperience(int experience)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("experience").SetValueAsync(experience);
        Debug.Log("Experience saved to Firebase: " + experience);
    }

    // 경험치 로드 메서드
    public void LoadExperience(Action onComplete = null) // 콜백 추가
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("experience").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load experience from Firebase");
                Player.Instance.Experience = 0; // 로드 실패 시 기본값 설정
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int experience = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;
                Player.Instance.Experience = experience; // 프로퍼티에 값 설정
                Debug.Log("Experience loaded from Firebase: " + experience);
            }

            onComplete?.Invoke(); // 로드 완료 후 콜백 실행
        });
    }

    // 레벨 저장 메서드
    public void SaveLevel(int level)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("level").SetValueAsync(level);
        Debug.Log("Level saved to Firebase: " + level);
    }

    // 레벨 로드 메서드
    public void LoadLevel(Action onComplete = null) // 콜백 추가
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("level").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load level from Firebase");
                Player.Instance.Level = 1; // 기본 레벨은 1
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int level = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 1;
                Player.Instance.Level = level; // 프로퍼티에 값 설정
                Debug.Log("Level loaded from Firebase: " + level);
            }

            onComplete?.Invoke(); // 로드 완료 후 콜백 실행
        });
    }

    // 게임 진행도 저장 메서드
    public void SaveGameProgress(int progress)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("gameProgress").SetValueAsync(progress);
        Debug.Log("Game progress saved to Firebase: " + progress);
    }

    // 게임 진행도 로드 메서드
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

    // 플레이어 위치 저장 메서드
    public void SavePlayerPosition(Vector3 position)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        string positionStr = $"{position.x},{position.y},{position.z}";
        databaseReference.Child("users").Child(userId).Child("playerPosition").SetValueAsync(positionStr);
        Debug.Log("Player position saved to Firebase: " + positionStr);
    }

    // 플레이어 위치 로드 메서드
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

    // 특정 웨이포인트의 클리어 횟수를 가져오는 메서드
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

                // 해당 웨이포인트에 건물을 배치합니다.
                if (clearedTimes > 0)
                {
                    WaypointManager.Instance.PlaceBuildingAtWaypoint(waypointIndex - 1, clearedTimes);
                }
            }
        });
    }

    // 특정 웨이포인트를 클리어한 상태로 기록하는 메서드
    public void MarkWaypointAsCleared(int waypointIndex)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("cleared").SetValueAsync(true);
        Debug.Log("Waypoint marked as cleared: " + waypointIndex);
    }

    // 특정 웨이포인트가 클리어되었는지 확인하는 메서드
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

    // 특정 웨이포인트의 난이도를 설정하는 메서드
    public void SetDifficultyForWaypoint(int waypointIndex, int difficulty)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        int cappedDifficulty = Mathf.Min(difficulty, 3);
        databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("difficulty").SetValueAsync(cappedDifficulty);
        Debug.Log("Waypoint difficulty set: " + cappedDifficulty);
    }

    // 특정 웨이포인트의 난이도를 가져오는 메서드
    public void GetDifficultyForWaypoint(int waypointIndex, Action<int> onLoaded)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("difficulty").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to load difficulty from Firebase");
                onLoaded?.Invoke(1); // 기본 난이도는 1
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int difficulty = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 1;
                onLoaded?.Invoke(difficulty);
            }
        });
    }

    // 특정 웨이포인트를 클리어한 횟수를 증가시키는 메서드
    public void IncrementClearedTimesForWaypoint(int waypointIndex)
    {
        GetClearedTimesForWaypoint(waypointIndex, currentTimes =>
        {
            int newClearedTimes = currentTimes + 1;
            string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

            // 클리어 횟수를 Firebase에 저장
            databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("clearedTimes").SetValueAsync(newClearedTimes);

            // 웨이포인트에 해당하는 위치에 건물을 생성합니다.
            WaypointManager.Instance.PlaceBuildingAtWaypoint(waypointIndex - 1, newClearedTimes);

            // 생성된 건물 상태를 저장
            databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("buildings").SetValueAsync(newClearedTimes);

            // 난이도 증가 처리
            GetDifficultyForWaypoint(waypointIndex, currentDifficulty =>
            {
                int newDifficulty = Mathf.Min(currentDifficulty + 1, 3);
                databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("difficulty").SetValueAsync(newDifficulty);
            });
        });
    }
    // 클리어 횟수를 가지고와 빌딩 로드
    public void LoadBuildings()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // 각 웨이포인트의 건물 상태를 불러옵니다.
        for (int i = 0; i < 13; i++)
        {
            int waypointIndex = i;

            GetClearedTimesForWaypoint(waypointIndex, clearedTimes =>
            {
                if (clearedTimes > 0)
                {
                    // clearedTimes 만큼 반복하여 각 위치에 빌딩 생성
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
