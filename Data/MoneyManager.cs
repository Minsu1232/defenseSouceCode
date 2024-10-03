using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    public event Action<int> OnMoneyChanged; // ���� ����� �� �߻��ϴ� �̺�Ʈ
    public event Action<int> OnBossCoinsChanged; // ���� ������ ����� �� �߻��ϴ� �̺�Ʈ
    public event Action<int> OnPlayeMoneyChanged; // ���� ������ ����� �� �߻��ϴ� �̺�Ʈ

    private int currentMoney;
    public int bossCoins; // ���� ������ �����ϴ� ����
    private int currentPlayerMoney;

    [SerializeField] private TextMeshProUGUI moneyText; // ���� ǥ���� UI Text (����Ƽ �����Ϳ��� �Ҵ�)
    [SerializeField] private TextMeshProUGUI bossCoinsText; // ���� ������ ǥ���� UI Text (����Ƽ �����Ϳ��� �Ҵ�)
    [SerializeField] private Image moneyImage;
    [SerializeField] private Image bossCoinsImage;

    public int CurrentMoney
    {
        get => currentMoney;
        private set
        {
            currentMoney = value;
            OnMoneyChanged?.Invoke(currentMoney); // ���� ����� �� �̺�Ʈ ȣ��
        }
    }

    public int BossCoins
    {
        get => bossCoins;
        private set
        {
            bossCoins = value;
            OnBossCoinsChanged?.Invoke(bossCoins); // ���� ������ ����� �� �̺�Ʈ ȣ��
        }
    }

    //public int CurrentPlayerMoney
    //{
    //    get => CurrentPlayerMoney;
    //    private set
    //    {
    //        CurrentPlayerMoney = value;
    //        OnPlayeMoneyChanged?.Invoke(currentPlayerMoney); // ���� ������ ����� �� �̺�Ʈ ȣ��
    //    }
    //}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            

            // ���� ����� ������ UI�� ������Ʈ�ϴ� �̺�Ʈ ���
            OnMoneyChanged += UpdateMoneyText;
            OnBossCoinsChanged += UpdateBossCoinsText;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeMoney(250); // ���� �� �� �ʱ�ȭ
        InitializeBossCoins(0); // ���� �� ���� ���� �ʱ�ȭ
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
        return false; // ���� ������� ������ false ��ȯ
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
        return false; // ���� ������ ������� ������ false ��ȯ
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
