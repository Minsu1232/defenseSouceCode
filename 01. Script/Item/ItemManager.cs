using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    public List<Item> Items = new List<Item>(); // 뽑을 수 있는 아이템 리스트
    public List<Item> activeItems = new List<Item>(); // 현재 활성화된 모든 아이템 리스트
    List<int> occuInt = new List<int>(); // 아이템 중복 등장을 막기 위한 리스트
    public Image[] itemImage;
    public Transform textTransform;

    public GameObject itemInfoPanel; // 아이템 정보를 표시할 패널
    public TextMeshProUGUI itemNameText; // 아이템 이름을 표시할 텍스트
    public TextMeshProUGUI itemDescriptionText; // 아이템 설명을 표시할 텍스트

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
        ApplyItemsToExistingObjects(); // Start 메서드에서 호출
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

            // 중복되지 않는 아이템 인덱스를 발견했으므로 리스트에 추가
            occuInt.Add(index);
            Item selectedItem = Items[index];
            activeItems.Add(selectedItem);

            // 새로 추가된 아이템을 모든 기존 유닛과 몬스터에 적용
            ApplyItemToAllObjects(selectedItem);

            // 아이템 이미지를 UI에 적용하고 클릭 이벤트 추가
            for (int i = 0; i < itemImage.Length; i++)
            {
                if (itemImage[i].sprite == null)
                {
                    itemImage[i].sprite = selectedItem.itemData.itemIcon;

                    // 클릭 이벤트 추가
                    AddClickEvent(itemImage[i], selectedItem);

                    Debug.Log("이미지 삽입");
                    break;
                }
            }
        }
    }

    // 클릭 이벤트를 추가하는 함수
    private void AddClickEvent(Image img, Item selectedItem)
    {
        Button button = img.gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => { ShowItemInfo(selectedItem); });
    }

    // 아이템 정보를 보여주는 함수 (고정된 위치로)
    private void ShowItemInfo(Item item)
    {
        itemNameText.text = item.itemData.itemName;
        itemDescriptionText.text = item.itemData.itemExplain;
        
        itemInfoPanel.SetActive(!itemInfoPanel.activeSelf);

        // 패널 위치를 업데이트하지 않음, 고정된 위치에서 표시
    }

    // 아이템 정보를 숨기는 함수
    public void HideItemInfo()
    {
        itemInfoPanel.SetActive(false);
    }
    public void RandomMoney()
    {
        // 먼저, 보스 코인이 하나 있는지 확인합니다.
        if (!MoneyManager.Instance.SpendBossCoins(1))
        {
            Debug.LogWarning("보스 코인이 부족하여 돈을 획득할 수 없습니다.");
            return;
        }

        // 랜덤 값을 생성하고, 각 확률에 따라 돈을 설정합니다.
        float randomValue = UnityEngine.Random.value; // 0.0과 1.0 사이의 랜덤 값
        int moneyToAdd = 0;

        if (randomValue <= 0.50f) // 50% 확률로 30 골드
        {
            moneyToAdd = 30;
        }
        else if (randomValue <= 0.80f) // 30% 확률로 70 골드
        {
            moneyToAdd = 70;
        }
        else if (randomValue <= 0.90f) // 10% 확률로 100 골드
        {
            moneyToAdd = 100;
        }
        else if (randomValue <= 0.97f) // 7% 확률로 150 골드
        {
            moneyToAdd = 150;
        }
        else // 3% 확률로 300 골드
        {
            moneyToAdd = 300;
        }

        // MoneyManager를 통해 돈을 추가합니다.
        MoneyManager.Instance.AddMoney(moneyToAdd);
        DamageTextManager.Instance.ShowGetMoneyText(textTransform, moneyToAdd);
        Debug.Log($"{moneyToAdd} 골드를 획득했습니다!");
    }
    private void ApplyItemsToExistingObjects()
    {
        // HeroManager.Instance 및 MonsterSpawnManager.Instance가 초기화되었는지 확인
        if (HeroManager.Instance != null && HeroManager.Instance.summonedHeroInstances != null)
        {
            // 현재 소환된 모든 영웅에게 아이템 효과 적용
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
            // 현재 소환된 모든 몬스터에게 아이템 효과 적용
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
        // 모든 영웅에게 아이템 효과 적용
        foreach (GameObject heroObject in HeroManager.Instance.summonedHeroInstances)
        {
            CharacterInfo character = heroObject.GetComponent<CharacterInfo>();
            if (character != null)
            {
                item.ApplyEffect(character, this);
            }
        }

        // 모든 몬스터에게 아이템 효과 적용
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