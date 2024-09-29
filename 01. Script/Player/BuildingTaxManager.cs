using System;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class BuildingTaxManager : MonoBehaviour
{      


    private DatabaseReference databaseReference;
    private DateTime lastSaveTime;

    public int waypointIndex; // 인스펙터에서 직접 설정할 웨이포인트 인덱스
    [SerializeField] private int moneyPerBuildingPerHour = 1000; // 빌딩당 시간당 축적되는 돈
    public GameObject collectMoneyUI; // 돈 수거 UI (웨이포인트마다 존재)
    public TextMeshProUGUI taxTimeText; // 남은 시간을 표시할 텍스트 UI

    private int buildingCount; // 웨이포인트별로 빌딩 수 저장
    private float timeSinceLastCheck; // 경과 시간 체크
    public bool moneyReadyForWaypoint; // 돈 수거 가능 여부

    private void Start()
    {
        StartCoroutine(WaitForFirebaseAndInitialize());
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
            Debug.Log("Firebase Initialized in BuildingTaxManager");
            yield return LoadWaypointData(); // 웨이포인트 및 빌딩 정보 로드
        }
        else
        {
            Debug.LogError("Failed to get Firebase Database Reference in BuildingTaxManager.");
        }
    }

    private IEnumerator LoadWaypointData()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // 각 웨이포인트에서 빌딩 수를 가져옴
        var task = databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("buildings").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted); // 비동기 작업이 끝날 때까지 대기

        if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                //moneyReadyForWaypoint = true;
                buildingCount = int.Parse(snapshot.Value.ToString());
                Debug.Log($"Building count for waypoint {waypointIndex}: {buildingCount}");

                timeSinceLastCheck = 0f; // 웨이포인트에 대한 경과 시간 초기화

                // 웨이포인트의 시간이 지나면 돈을 수거할 수 있도록 준비
                yield return LoadLastSaveTime();
            }
            else
            {
                Debug.LogError("No building data found for waypoint " + waypointIndex);
                buildingCount = 0; // 빌딩 수가 없으면 0으로 설정
            }
        }
        else
        {
            Debug.LogError("Failed to load building data for waypoint " + waypointIndex);
        }
    }

    private IEnumerator LoadLastSaveTime()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        var task = databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("lastSaveTime").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted); // 비동기 작업이 끝날 때까지 대기

        if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                lastSaveTime = DateTime.Parse(snapshot.Value.ToString());
                TimeSpan timePassed = DateTime.Now - lastSaveTime;
                float secondsPassed = (float)timePassed.TotalSeconds;

                // 건물 개수에 따라 축적 시간을 계산 (1개당 1시간, 2개당 2시간 등)
                float requiredTime = buildingCount * 3600f; // 1시간 = 3600초

                // 경과한 시간이 빌딩 개수에 따른 요구 시간 이상일 경우에만 돈 수거 가능 상태로 변경
                if (secondsPassed >= requiredTime)
                {
                    moneyReadyForWaypoint = true;
                    collectMoneyUI.SetActive(true); // 수거 UI 활성화
                    taxTimeText.text = "수거 가능"; // 수거 가능 상태 표시
                    Debug.Log($"Money ready for waypoint {waypointIndex}");
                }
                else
                {
                    // 남은 시간을 계산
                    moneyReadyForWaypoint = false; // 아직 수거 가능하지 않음
                    timeSinceLastCheck = secondsPassed;
                    UpdateTaxTimeText(requiredTime - secondsPassed); // 남은 시간을 업데이트
                    collectMoneyUI.SetActive(false); // UI 비활성화
                }
            }
            else
            {
                Debug.Log("No lastSaveTime data found.");
                lastSaveTime = DateTime.Now; // 데이터가 없을 경우 현재 시간을 저장
                SaveLastSaveTime();
            }
        }
        else
        {
            Debug.LogError("Failed to load lastSaveTime for waypoint " + waypointIndex);
        }
    }

    // 매 프레임마다 남은 시간을 업데이트하는 함수
    private void Update()
    {
        if (!moneyReadyForWaypoint && buildingCount > 0)
        {
            // 건물 개수에 따른 요구 시간
            float requiredTime = buildingCount * 3600f; // 1시간 = 3600초

            // 남은 시간을 계산하고 텍스트로 표시
            float remainingTime = requiredTime - timeSinceLastCheck;
            UpdateTaxTimeText(remainingTime);

            // 시간이 다 되면 돈 수거 가능 상태로 전환
            if (remainingTime <= 0f)
            {
                moneyReadyForWaypoint = true;
                collectMoneyUI.SetActive(true);
                taxTimeText.text = "수거 가능"; // 수거 가능 상태 표시
            }
            else
            {
                timeSinceLastCheck += Time.deltaTime;
            }
        }
    }

    // 남은 시간을 텍스트로 표시하는 함수
    private void UpdateTaxTimeText(float remainingTimeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTimeInSeconds);
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        taxTimeText.text = timeText;
    }

    // 돈을 수거하는 함수
    public void CollectMoneyForWaypoint()
    {
        StartCoroutine(CollectMoneyRoutine());
    }

    private IEnumerator CollectMoneyRoutine()
    {
        // 웨이포인트 및 빌딩 정보를 다시 로드하고 기다림
        yield return LoadWaypointData();

        if (moneyReadyForWaypoint)
        {
            int accumulatedMoney = Mathf.FloorToInt(buildingCount * moneyPerBuildingPerHour);
            Debug.Log($"빌딩수 {buildingCount}");
            Debug.Log($"세금 {accumulatedMoney}");

            if (accumulatedMoney > 0)
            {
                AddMoneyToPlayer(accumulatedMoney);
                SaveLastSaveTime(); // 마지막 수거 시간 저장
            }
            else
            {
                Debug.LogError("No money to collect.");
            }

            // 수거 완료 후 상태 초기화
            moneyReadyForWaypoint = false;
            collectMoneyUI.SetActive(false); // UI 비활성화
            timeSinceLastCheck = 0f; // 경과 시간 초기화
        }
        else
        {
            Debug.LogError("Money not ready to be collected.");
        }
    }

    private void SaveLastSaveTime()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("lastSaveTime").SetValueAsync(DateTime.Now.ToString());
    }

    // 플레이어에게 돈을 추가하는 함수
    private void AddMoneyToPlayer(int money)
    {
        PlayerMoneyManager.Instance.AddMoney(money);
        Debug.Log($"Collected money: {money} from waypoint {waypointIndex}");
    }
}
