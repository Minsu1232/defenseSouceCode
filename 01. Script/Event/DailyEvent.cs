using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DailyEvent : MonoBehaviour
{
    DatabaseReference databaseReference;

    // �⼮ UI ���� ����
    public GameObject eventUIPanel;   // �̺�Ʈ UI �г�
    public GameObject dailyEventPanel; // �⼮ �̺�Ʈ �г�
    public Button eventButton;        // �̺�Ʈ ��ư
    public Image[] checkImages;       // �� �г��� Check �̹��� �迭

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
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
            string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            Debug.Log("Firebase Initialized in SaveLoadManager");
            CheckAttendance(userId);
        }
        else
        {
            Debug.LogError("Failed to get Firebase Database Reference in SaveLoadManager.");
        }
    }

    // �⼮�� üũ�ϴ� �޼���
    public void CheckAttendance(string userId)
    {
        databaseReference.Child("users").Child(userId).Child("attendance").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                DateTime lastLogin;
                if (snapshot.HasChild("lastLogin"))
                {
                    lastLogin = DateTime.Parse(snapshot.Child("lastLogin").Value.ToString());
                }
                else
                {
                    lastLogin = DateTime.MinValue; // ù �⼮���� ����
                }

                DateTime currentDate = DateTime.Now;

                if ((currentDate - lastLogin).Days >= 1)
                {
                    ShowEventUI();
                    int attendedDays = CalculateAttendanceDays(snapshot);
                    UpdateAttendance(userId, attendedDays, currentDate);
                }

                // �� ���� üũ �̹��� ǥ��
                UpdateCheckImages(snapshot);
            }
        });
    }

    // �⼮�� ���� üũ �̹����� ǥ���ϴ� �޼���
    void UpdateCheckImages(DataSnapshot snapshot)
    {
        for (int i = 0; i < checkImages.Length; i++)
        {
            // �ش� ���� �⼮ �Ϸ�Ǿ����� üũ �̹��� Ȱ��ȭ
            if (snapshot.HasChild($"day{i + 1}") && bool.Parse(snapshot.Child($"day{i + 1}").Value.ToString()))
            {
                checkImages[i].gameObject.SetActive(true);
            }
            else
            {
                checkImages[i].gameObject.SetActive(false);  // �⼮���� ���� ���� ��Ȱ��ȭ
            }
        }
    }

    int CalculateAttendanceDays(DataSnapshot snapshot)
    {
        int daysAttended = 0;
        for (int i = 1; i <= 7; i++)
        {
            if (snapshot.HasChild($"day{i}") && bool.Parse(snapshot.Child($"day{i}").Value.ToString()))
            {
                daysAttended++;
            }
        }
        return daysAttended;
    }

    void UpdateAttendance(string userId, int daysAttended, DateTime currentDate)
    {
        if (daysAttended < 7)
        {
            string nextDay = $"day{daysAttended + 1}";

            // Firebase�� �⼮ ���� ������Ʈ
            databaseReference.Child("users").Child(userId).Child("attendance").Child(nextDay).SetValueAsync(true);
            databaseReference.Child("users").Child(userId).Child("attendance").Child("lastLogin").SetValueAsync(currentDate.ToString("yyyy-MM-dd"));

            GiveReward(daysAttended + 1);

            Debug.Log($"{daysAttended + 1}���� �⼮ �Ϸ�!");

            // ��� üũ �̹��� ������Ʈ
            databaseReference.Child("users").Child(userId).Child("attendance").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    UpdateCheckImages(snapshot); // �⼮ ������Ʈ �� ��� üũ �̹��� ������Ʈ
                }
            });
        }
    }

    void GiveReward(int day)
    {
        int rewardAmount = day * 1000;
        PlayerMoneyManager.Instance.AddMoney(rewardAmount);
        Debug.Log($"{rewardAmount}���� ���޵Ǿ����ϴ�.");
    }

    void ShowEventUI()
    {
        eventUIPanel.SetActive(true);  // �⼮ UI ǥ��
    }
    public void OnDailyEventPanel()
    {
        dailyEventPanel.SetActive(true);
    }
    public void OnEventButtonClick()
    {
        eventUIPanel.SetActive(true);  // �⼮ UI ǥ��
    }
}
