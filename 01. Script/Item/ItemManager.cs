using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public List<Item> Items = new List<Item>(); // ���� �� �ִ� ������ ����Ʈ
    public List<Item> activeItems = new List<Item>(); // ���� Ȱ��ȭ�� ��� ������ ����Ʈ
    List<int> occuInt = new List<int>(); // ������ �ߺ� ������ ���� ���� ����Ʈ
    public Image[] itemImage;
    public Transform textTransform;

    public GameObject itemInfoPanel; // ������ ������ ǥ���� �г�
    public TextMeshProUGUI itemNameText; // ������ �̸��� ǥ���� �ؽ�Ʈ
    public TextMeshProUGUI itemDescriptionText; // ������ ������ ǥ���� �ؽ�Ʈ

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ApplyItemsToExistingObjects(); // Start �޼��忡�� ȣ��
    }

    public void RandomItem()
    {
        if (!MoneyManager.Instance.SpendBossCoins(1))
        {
            Debug.LogWarning("Not enough money to summon a hero.");
        }
        else
        {
            if (occuInt.Count >= Items.Count)
            {
                Debug.LogWarning("All items have been selected. No more unique items to choose.");
                return;
            }

            int index;
            do
            {
                index = Random.Range(0, Items.Count);
            } while (occuInt.Contains(index));

            // �ߺ����� �ʴ� ������ �ε����� �߰������Ƿ� ����Ʈ�� �߰�
            occuInt.Add(index);
            Item selectedItem = Items[index];
            activeItems.Add(selectedItem);

            // ���� �߰��� �������� ��� ���� ���ְ� ���Ϳ� ����
            ApplyItemToAllObjects(selectedItem);

            // ������ �̹����� UI�� �����ϰ� Ŭ�� �̺�Ʈ �߰�
            for (int i = 0; i < itemImage.Length; i++)
            {
                if (itemImage[i].sprite == null)
                {
                    itemImage[i].sprite = selectedItem.itemData.itemIcon;

                    // Ŭ�� �̺�Ʈ �߰�
                    AddClickEvent(itemImage[i], selectedItem);

                    Debug.Log("�̹��� ����");
                    break;
                }
            }
        }
    }

    // Ŭ�� �̺�Ʈ�� �߰��ϴ� �Լ�
    private void AddClickEvent(Image img, Item selectedItem)
    {
        Button button = img.gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => { ShowItemInfo(selectedItem); });
    }

    // ������ ������ �����ִ� �Լ� (������ ��ġ��)
    private void ShowItemInfo(Item item)
    {
        itemNameText.text = item.itemData.itemName;
        itemDescriptionText.text = item.itemData.itemExplain;
        
        itemInfoPanel.SetActive(!itemInfoPanel.activeSelf);

        // �г� ��ġ�� ������Ʈ���� ����, ������ ��ġ���� ǥ��
    }

    // ������ ������ ����� �Լ�
    public void HideItemInfo()
    {
        itemInfoPanel.SetActive(false);
    }
    public void RandomMoney()
    {
        // ����, ���� ������ �ϳ� �ִ��� Ȯ���մϴ�.
        if (!MoneyManager.Instance.SpendBossCoins(1))
        {
            Debug.LogWarning("���� ������ �����Ͽ� ���� ȹ���� �� �����ϴ�.");
            return;
        }

        // ���� ���� �����ϰ�, �� Ȯ���� ���� ���� �����մϴ�.
        float randomValue = UnityEngine.Random.value; // 0.0�� 1.0 ������ ���� ��
        int moneyToAdd = 0;

        if (randomValue <= 0.50f) // 50% Ȯ���� 30 ���
        {
            moneyToAdd = 30;
        }
        else if (randomValue <= 0.80f) // 30% Ȯ���� 70 ���
        {
            moneyToAdd = 70;
        }
        else if (randomValue <= 0.90f) // 10% Ȯ���� 100 ���
        {
            moneyToAdd = 100;
        }
        else if (randomValue <= 0.97f) // 7% Ȯ���� 150 ���
        {
            moneyToAdd = 150;
        }
        else // 3% Ȯ���� 300 ���
        {
            moneyToAdd = 300;
        }

        // MoneyManager�� ���� ���� �߰��մϴ�.
        MoneyManager.Instance.AddMoney(moneyToAdd);
        DamageTextManager.Instance.ShowGetMoneyText(textTransform, moneyToAdd);
        Debug.Log($"{moneyToAdd} ��带 ȹ���߽��ϴ�!");
    }
    private void ApplyItemsToExistingObjects()
    {
        // HeroManager.Instance �� MonsterSpawnManager.Instance�� �ʱ�ȭ�Ǿ����� Ȯ��
        if (HeroManager.Instance != null && HeroManager.Instance.summonedHeroInstances != null)
        {
            // ���� ��ȯ�� ��� �������� ������ ȿ�� ����
            foreach (GameObject heroObject in HeroManager.Instance.summonedHeroInstances)
            {
                CharacterInfo character = heroObject.GetComponent<CharacterInfo>();
                if (character != null)
                {
                    ApplyActiveItems(character);
                }
            }
        }

        if (MonsterSpawnManager.Instance != null && MonsterSpawnManager.Instance.spawnedMonsters != null)
        {
            // ���� ��ȯ�� ��� ���Ϳ��� ������ ȿ�� ����
            foreach (Monster monster in MonsterSpawnManager.Instance.spawnedMonsters)
            {
                ApplyActiveItems(monster);
            }
        }
    }

    public void ApplyActiveItems(Monster monster)
    {
        foreach (var item in activeItems)
        {
            item.ApplyEffect(monster, this);
        }
    }

    public void ApplyActiveItems(CharacterInfo character)
    {
        if (character == null)
        {
            Debug.LogError("CharacterInfo is null in ApplyActiveItems");
            return;
        }

        foreach (var item in activeItems)
        {
            if (item == null)
            {
                Debug.LogError("An item in activeItems is null");
                continue;
            }

            item.ApplyEffect(character, this);
        }
    }

    public void ActivateItem(Item item)
    {
        activeItems.Add(item);
        ApplyItemToAllObjects(item);
    }

    private void ApplyItemToAllObjects(Item item)
    {
        // ��� �������� ������ ȿ�� ����
        foreach (GameObject heroObject in HeroManager.Instance.summonedHeroInstances)
        {
            CharacterInfo character = heroObject.GetComponent<CharacterInfo>();
            if (character != null)
            {
                item.ApplyEffect(character, this);
            }
        }

        // ��� ���Ϳ��� ������ ȿ�� ����
        foreach (Monster monster in MonsterSpawnManager.Instance.spawnedMonsters)
        {
            item.ApplyEffect(monster, this);
        }
    }

    private void RegisterMonster(Monster monster)
    {
        ApplyActiveItems(monster);
    }

    private void RegisterHero(CharacterInfo hero)
    {
        ApplyActiveItems(hero);
    }

    public void StartCoroutineFromItem(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}