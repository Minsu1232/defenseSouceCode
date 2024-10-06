using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase.Extensions;
/// <summary>
/// 플레이어의 재화 중앙 관리 스크립트
/// </summary>
public class PlayerMoneyManager : MonoBehaviour
{
    public static PlayerMoneyManager Instance { get; private set; } // 싱글턴 인스턴스

    public event Action<int> OnMoneyChanged; // 돈이 변경될 때 발생하는 이벤트

    private int currentMoney;

    [SerializeField] private TextMeshProUGUI moneyText; // 돈을 표시할 UI Text (유니티 에디터에서 할당)
    private DatabaseReference databaseReference;
    public int CurrentMoney
    {
        get => currentMoney;
        private set
        {
            currentMoney = value;
            OnMoneyChanged?.Invoke(currentMoney); // 돈이 변경될 때 이벤트 호출            
            SaveMoneyToFirebase(); // 재화 변경 시 Firebase에 저장

        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            StartCoroutine(WaitForFirebaseAndLoadMoney());
            UpdateMoneyText(currentMoney);


            // 돈이 변경될 때마다 UI를 업데이트하는 이벤트 등록
            OnMoneyChanged += UpdateMoneyText;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator WaitForFirebaseAndLoadMoney()
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
            // Firebase에서 재화 정보 불러오기
            yield return StartCoroutine(LoadMoneyFromFirebase()); // 비동기 로드 완료될 때까지 대기


        }
        else
        {
            Debug.LogError("Failed to get Firebase Database Reference in PlayerMoneyManager.");
        }
    }
    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            CurrentMoney -= amount;
            return true;
        }
        return false; // 돈이 충분하지 않으면 false 반환
    }
    //private string FormatMoney(float money)
    //{
    //    if (money >= 1000000)
    //    {
    //        return $"{money / 1000000f:0.#}M"; // 백만 단위로 변환
    //    }
    //    else if (money >= 1000)
    //    {
    //        return $"{money / 1000f:0.#}K"; // 천 단위로 변환
    //    }
    //    else
    //    {
    //        return money.ToString(); // 천 미만은 그대로 출력
    //    }
    //}
    private void UpdateMoneyText(int currentMoney)
    {
        if (moneyText != null)
        {
            //string text = FormatMoney(currentMoney);
            moneyText.text = currentMoney.ToString();
        }
    }

    // Firebase에 재화 정보를 저장하는 메서드
    private void SaveMoneyToFirebase()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("money").SetValueAsync(currentMoney);
        Debug.Log("Firebase에 재화 저장: " + currentMoney);
    }

    // Firebase에서 재화 정보를 로드하는 메서드
    private IEnumerator LoadMoneyFromFirebase()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var task = databaseReference.Child("users").Child(userId).Child("money").GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted); // Firebase 데이터가 로드될 때까지 대기

        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.LogError("Failed to load money from Firebase");
            CurrentMoney = 0; // 실패 시 기본값 설정
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            // 불러온 값으로 CurrentMoney 설정
            CurrentMoney = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;

            // 돈이 불러와진 후 출석 이벤트 로직 실행
            DailyEvent dailyEvent = FindObjectOfType<DailyEvent>();
            if (dailyEvent != null)
            {
                dailyEvent.CheckAttendance(userId); // 출석 이벤트 실행
            }

            // UI 업데이트
            UpdateMoneyText(CurrentMoney);
            Debug.Log("Firebase에서 재화 로드: " + CurrentMoney);
        }
    }
}
