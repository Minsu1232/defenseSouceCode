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

    public int waypointIndex; // �ν����Ϳ��� ���� ������ ��������Ʈ �ε���
    [SerializeField] private int moneyPerBuildingPerHour = 1000; // ������ �ð��� �����Ǵ� ��
    public GameObject collectMoneyUI; // �� ���� UI (��������Ʈ���� ����)
    public TextMeshProUGUI taxTimeText; // ���� �ð��� ǥ���� �ؽ�Ʈ UI

    private int buildingCount; // ��������Ʈ���� ���� �� ����
    private float timeSinceLastCheck; // ��� �ð� üũ
    public bool moneyReadyForWaypoint; // �� ���� ���� ����

    private void Start()
    {
        StartCoroutine(WaitForFirebaseAndInitialize());
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
            Debug.Log("Firebase Initialized in BuildingTaxManager");
            yield return LoadWaypointData(); // ��������Ʈ �� ���� ���� �ε�
        }
        else
        {
            Debug.LogError("Failed to get Firebase Database Reference in BuildingTaxManager.");
        }
    }

    private IEnumerator LoadWaypointData()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // �� ��������Ʈ���� ���� ���� ������
        var task = databaseReference.Child("users").Child(userId).Child("waypoints").Child(waypointIndex.ToString()).Child("buildings").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted); // �񵿱� �۾��� ���� ������ ���

        if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                //moneyReadyForWaypoint = true;
                buildingCount = int.Parse(snapshot.Value.ToString());
                Debug.Log($"Building count for waypoint {waypointIndex}: {buildingCount}");

                timeSinceLastCheck = 0f; // ��������Ʈ�� ���� ��� �ð� �ʱ�ȭ

                // ��������Ʈ�� �ð��� ������ ���� ������ �� �ֵ��� �غ�
                yield return LoadLastSaveTime();
            }
            else
            {
                Debug.LogError("No building data found for waypoint " + waypointIndex);
                buildingCount = 0; // ���� ���� ������ 0���� ����
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
        yield return new WaitUntil(() => task.IsCompleted); // �񵿱� �۾��� ���� ������ ���

        if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                lastSaveTime = DateTime.Parse(snapshot.Value.ToString());
                TimeSpan timePassed = DateTime.Now - lastSaveTime;
                float secondsPassed = (float)timePassed.TotalSeconds;

                // �ǹ� ������ ���� ���� �ð��� ��� (1���� 1�ð�, 2���� 2�ð� ��)
                float requiredTime = buildingCount * 3600f; // 1�ð� = 3600��

                // ����� �ð��� ���� ������ ���� �䱸 �ð� �̻��� ��쿡�� �� ���� ���� ���·� ����
                if (secondsPassed >= requiredTime)
                {
                    moneyReadyForWaypoint = true;
                    collectMoneyUI.SetActive(true); // ���� UI Ȱ��ȭ
                    taxTimeText.text = "���� ����"; // ���� ���� ���� ǥ��
                    Debug.Log($"Money ready for waypoint {waypointIndex}");
                }
                else
                {
                    // ���� �ð��� ���
                    moneyReadyForWaypoint = false; // ���� ���� �������� ����
                    timeSinceLastCheck = secondsPassed;
                    UpdateTaxTimeText(requiredTime - secondsPassed); // ���� �ð��� ������Ʈ
                    collectMoneyUI.SetActive(false); // UI ��Ȱ��ȭ
                }
            }
            else
            {
                Debug.Log("No lastSaveTime data found.");
                lastSaveTime = DateTime.Now; // �����Ͱ� ���� ��� ���� �ð��� ����
                SaveLastSaveTime();
            }
        }
        else
        {
            Debug.LogError("Failed to load lastSaveTime for waypoint " + waypointIndex);
        }
    }

    // �� �����Ӹ��� ���� �ð��� ������Ʈ�ϴ� �Լ�
    private void Update()
    {
        if (!moneyReadyForWaypoint && buildingCount > 0)
        {
            // �ǹ� ������ ���� �䱸 �ð�
            float requiredTime = buildingCount * 3600f; // 1�ð� = 3600��

            // ���� �ð��� ����ϰ� �ؽ�Ʈ�� ǥ��
            float remainingTime = requiredTime - timeSinceLastCheck;
            UpdateTaxTimeText(remainingTime);

            // �ð��� �� �Ǹ� �� ���� ���� ���·� ��ȯ
            if (remainingTime <= 0f)
            {
                moneyReadyForWaypoint = true;
                collectMoneyUI.SetActive(true);
                taxTimeText.text = "���� ����"; // ���� ���� ���� ǥ��
            }
            else
            {
                timeSinceLastCheck += Time.deltaTime;
            }
        }
    }

    // ���� �ð��� �ؽ�Ʈ�� ǥ���ϴ� �Լ�
    private void UpdateTaxTimeText(float remainingTimeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTimeInSeconds);
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        taxTimeText.text = timeText;
    }

    // ���� �����ϴ� �Լ�
    public void CollectMoneyForWaypoint()
    {
        StartCoroutine(CollectMoneyRoutine());
    }

    private IEnumerator CollectMoneyRoutine()
    {
        // ��������Ʈ �� ���� ������ �ٽ� �ε��ϰ� ��ٸ�
        yield return LoadWaypointData();

        if (moneyReadyForWaypoint)
        {
            int accumulatedMoney = Mathf.FloorToInt(buildingCount * moneyPerBuildingPerHour);
            Debug.Log($"������ {buildingCount}");
            Debug.Log($"���� {accumulatedMoney}");

            if (accumulatedMoney > 0)
            {
                AddMoneyToPlayer(accumulatedMoney);
                SaveLastSaveTime(); // ������ ���� �ð� ����
            }
            else
            {
                Debug.LogError("No money to collect.");
            }

            // ���� �Ϸ� �� ���� �ʱ�ȭ
            moneyReadyForWaypoint = false;
            collectMoneyUI.SetActive(false); // UI ��Ȱ��ȭ
            timeSinceLastCheck = 0f; // ��� �ð� �ʱ�ȭ
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

    // �÷��̾�� ���� �߰��ϴ� �Լ�
    private void AddMoneyToPlayer(int money)
    {
        PlayerMoneyManager.Instance.AddMoney(money);
        Debug.Log($"Collected money: {money} from waypoint {waypointIndex}");
    }
}
