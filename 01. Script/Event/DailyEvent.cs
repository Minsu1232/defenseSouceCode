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

    // 출석 UI 관련 변수
    public GameObject eventUIPanel;   // 이벤트 UI 패널
    public GameObject dailyEventPanel; // 출석 이벤트 패널
    public Button eventButton;        // 이벤트 버튼
    public Image[] checkImages;       // 각 패널의 Check 이미지 배열

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
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
            string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
            Debug.Log("Firebase Initialized in SaveLoadManager");
            CheckAttendance(userId);
        }
        else
        {
            Debug.LogError("Failed to get Firebase Database Reference in SaveLoadManager.");
        }
    }

    // 출석을 체크하는 메서드
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
                    lastLogin = DateTime.MinValue; // 첫 출석으로 간주
                }

                DateTime currentDate = DateTime.Now;

                if ((currentDate - lastLogin).Days >= 1)
                {
                    ShowEventUI();
                    int attendedDays = CalculateAttendanceDays(snapshot);
                    UpdateAttendance(userId, attendedDays, currentDate);
                }

                // 각 날에 체크 이미지 표시
                UpdateCheckImages(snapshot);
            }
        });
    }

    // 출석한 날에 체크 이미지를 표시하는 메서드
    void UpdateCheckImages(DataSnapshot snapshot)
    {
        for (int i = 0; i < checkImages.Length; i++)
        {
            // 해당 날이 출석 완료되었으면 체크 이미지 활성화
            if (snapshot.HasChild($"day{i + 1}") && bool.Parse(snapshot.Child($"day{i + 1}").Value.ToString()))
            {
                checkImages[i].gameObject.SetActive(true);
            }
            else
            {
                checkImages[i].gameObject.SetActive(false);  // 출석되지 않은 날은 비활성화
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

            // Firebase에 출석 정보 업데이트
            databaseReference.Child("users").Child(userId).Child("attendance").Child(nextDay).SetValueAsync(true);
            databaseReference.Child("users").Child(userId).Child("attendance").Child("lastLogin").SetValueAsync(currentDate.ToString("yyyy-MM-dd"));

            GiveReward(daysAttended + 1);

            Debug.Log($"{daysAttended + 1}일차 출석 완료!");

            // 즉시 체크 이미지 업데이트
            databaseReference.Child("users").Child(userId).Child("attendance").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    UpdateCheckImages(snapshot); // 출석 업데이트 후 즉시 체크 이미지 업데이트
                }
            });
        }
    }

    void GiveReward(int day)
    {
        int rewardAmount = day * 1000;
        PlayerMoneyManager.Instance.AddMoney(rewardAmount);
        Debug.Log($"{rewardAmount}원이 지급되었습니다.");
    }

    void ShowEventUI()
    {
        eventUIPanel.SetActive(true);  // 출석 UI 표시
    }
    public void OnDailyEventPanel()
    {
        dailyEventPanel.SetActive(true);
    }
    public void OnEventButtonClick()
    {
        eventUIPanel.SetActive(true);  // 출석 UI 표시
    }
}
