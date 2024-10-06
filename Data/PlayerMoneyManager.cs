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
/// �÷��̾��� ��ȭ �߾� ���� ��ũ��Ʈ
/// </summary>
public class PlayerMoneyManager : MonoBehaviour
{
    public static PlayerMoneyManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    public event Action<int> OnMoneyChanged; // ���� ����� �� �߻��ϴ� �̺�Ʈ

    private int currentMoney;

    [SerializeField] private TextMeshProUGUI moneyText; // ���� ǥ���� UI Text (����Ƽ �����Ϳ��� �Ҵ�)
    private DatabaseReference databaseReference;
    public int CurrentMoney
    {
        get => currentMoney;
        private set
        {
            currentMoney = value;
            OnMoneyChanged?.Invoke(currentMoney); // ���� ����� �� �̺�Ʈ ȣ��            
            SaveMoneyToFirebase(); // ��ȭ ���� �� Firebase�� ����

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


            // ���� ����� ������ UI�� ������Ʈ�ϴ� �̺�Ʈ ���
            OnMoneyChanged += UpdateMoneyText;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator WaitForFirebaseAndLoadMoney()
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
            // Firebase���� ��ȭ ���� �ҷ�����
            yield return StartCoroutine(LoadMoneyFromFirebase()); // �񵿱� �ε� �Ϸ�� ������ ���


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
        return false; // ���� ������� ������ false ��ȯ
    }
    //private string FormatMoney(float money)
    //{
    //    if (money >= 1000000)
    //    {
    //        return $"{money / 1000000f:0.#}M"; // �鸸 ������ ��ȯ
    //    }
    //    else if (money >= 1000)
    //    {
    //        return $"{money / 1000f:0.#}K"; // õ ������ ��ȯ
    //    }
    //    else
    //    {
    //        return money.ToString(); // õ �̸��� �״�� ���
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

    // Firebase�� ��ȭ ������ �����ϴ� �޼���
    private void SaveMoneyToFirebase()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("money").SetValueAsync(currentMoney);
        Debug.Log("Firebase�� ��ȭ ����: " + currentMoney);
    }

    // Firebase���� ��ȭ ������ �ε��ϴ� �޼���
    private IEnumerator LoadMoneyFromFirebase()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        var task = databaseReference.Child("users").Child(userId).Child("money").GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted); // Firebase �����Ͱ� �ε�� ������ ���

        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.LogError("Failed to load money from Firebase");
            CurrentMoney = 0; // ���� �� �⺻�� ����
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            // �ҷ��� ������ CurrentMoney ����
            CurrentMoney = snapshot.Exists ? int.Parse(snapshot.Value.ToString()) : 0;

            // ���� �ҷ����� �� �⼮ �̺�Ʈ ���� ����
            DailyEvent dailyEvent = FindObjectOfType<DailyEvent>();
            if (dailyEvent != null)
            {
                dailyEvent.CheckAttendance(userId); // �⼮ �̺�Ʈ ����
            }

            // UI ������Ʈ
            UpdateMoneyText(CurrentMoney);
            Debug.Log("Firebase���� ��ȭ �ε�: " + CurrentMoney);
        }
    }
}
