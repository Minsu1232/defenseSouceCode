using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; } // 싱글턴 인스턴스

    public event Action<int> OnMoneyChanged; // 돈이 변경될 때 발생하는 이벤트
    public event Action<int> OnBossCoinsChanged; // 보스 코인이 변경될 때 발생하는 이벤트
    public event Action<int> OnPlayeMoneyChanged; // 보스 코인이 변경될 때 발생하는 이벤트

    private int currentMoney;
    public int bossCoins; // 보스 코인을 관리하는 변수
    private int currentPlayerMoney;

    [SerializeField] private TextMeshProUGUI moneyText; // 돈을 표시할 UI Text (유니티 에디터에서 할당)
    [SerializeField] private TextMeshProUGUI bossCoinsText; // 보스 코인을 표시할 UI Text (유니티 에디터에서 할당)
    [SerializeField] private Image moneyImage;
    [SerializeField] private Image bossCoinsImage;

    public int CurrentMoney
    {
        get => currentMoney;
        private set
        {
            currentMoney = value;
            OnMoneyChanged?.Invoke(currentMoney); // 돈이 변경될 때 이벤트 호출
        }
    }

    public int BossCoins
    {
        get => bossCoins;
        private set
        {
            bossCoins = value;
            OnBossCoinsChanged?.Invoke(bossCoins); // 보스 코인이 변경될 때 이벤트 호출
        }
    }

    //public int CurrentPlayerMoney
    //{
    //    get => CurrentPlayerMoney;
    //    private set
    //    {
    //        CurrentPlayerMoney = value;
    //        OnPlayeMoneyChanged?.Invoke(currentPlayerMoney); // 보스 코인이 변경될 때 이벤트 호출
    //    }
    //}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            

            // 돈이 변경될 때마다 UI를 업데이트하는 이벤트 등록
            OnMoneyChanged += UpdateMoneyText;
            OnBossCoinsChanged += UpdateBossCoinsText;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeMoney(250); // 시작 시 돈 초기화
        InitializeBossCoins(0); // 시작 시 보스 코인 초기화
    }

    public void InitializeMoney(int initialAmount)
    {
        CurrentMoney = initialAmount;
    }

    private void InitializeBossCoins(int initialAmount)
    {
        BossCoins = initialAmount;
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

    public void AddBossCoins(int amount)
    {
        BossCoins += amount;
    }

    public bool SpendBossCoins(int amount)
    {
        if (bossCoins >= amount)
        {
            BossCoins -= amount;
            return true;
        }
        return false; // 보스 코인이 충분하지 않으면 false 반환
    }

    private void UpdateMoneyText(int currentMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString();
        }
    }

    private void UpdateBossCoinsText(int currentBossCoins)
    {
        if (bossCoinsText != null)
        {
            bossCoinsText.text = currentBossCoins.ToString();
        }
    }
}
