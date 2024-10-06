using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CharacterData;
/// <summary>
/// ���潺�������� ����� ��ȯ �� ������ �����մϴ�.
/// </summary>
public class HeroManager : MonoBehaviour
{
    public static HeroManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    public List<CharacterData> allHeroes; // ��� ���� ������ ����Ʈ
    public List<GameObject> summonedHeroInstances; // ��ȯ�� ���� �ν��Ͻ� ����Ʈ
    
    public List<SpecialCombination> specialCombinations; // Ư�� ���� ����Ʈ
    public Transform spawnParent; // ���� ��ġ�� �θ� ������Ʈ
    public int cols = 7; // ���� ����
    public int rows = 3; // ���� ����
    private Vector3[,] spawnPoints; // ���� ��ġ �迭
    private bool[,] occupiedPoints; // ���� ��ġ ���� ���� �迭

    public int summonCost = 10; // ���� ��ȯ�� �ʿ��� �ʱ� ��
    public CharacterInfoPanel infoPanel; // ���̾��Ű�� �����ϴ� InfoPanel
    public GraphicRaycaster mainGraphicRaycaster;
    public EventSystem mainEventSystem;
    [SerializeField] private TextMeshProUGUI summonCostText; //  ��ȯ �䱸ġ
    [SerializeField] private TextMeshProUGUI summonHeroes; // ���� ��ȯ�� ������ ��
    [SerializeField] private TextMeshProUGUI summonHeroeText; // ���� ��ȯ�� ������ ��
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // �ν��Ͻ� ����
            
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� ������ ���, ���ο� �ν��Ͻ��� �ı�
        }
    }
    private void OnEnable()
    { // �� ��ȯ �� SpawnTransform�� ã�Ƽ� spawnParent�� �Ҵ�
        GameObject spawnObject = GameObject.Find("SpawnTransform");
        if (spawnObject != null)
        {
            spawnParent = spawnObject.transform;
        }
        summonedHeroInstances.Clear();
        summonCost = 10;
        InitializeSpawnPoints(); // ���� ��ġ �ʱ�ȭ
        UpdateUI();
        Debug.Log("����� �Ŵ��� �ʱ�ȭ");
    }
    void Start()
    {
       
        
    }

    public void InitializeSpawnPoints() // ���� ��ġ�� �ʱ�ȭ�ϴ� �޼���
    {
        spawnPoints = new Vector3[rows, cols];
        occupiedPoints = new bool[rows, cols];
        float spacingX = 1.1f;
        float spacingY = -0.8f;

        Vector3 startPoint = spawnParent.position;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                spawnPoints[i, j] = startPoint + new Vector3(j * spacingX, i * spacingY, 0);
                occupiedPoints[i, j] = false;
            }
        }
    }

    public void Summon() // ��ȯ ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    {
        SummonHero();
    }

    public GameObject SummonHero() // ������ ��ȯ�ϴ� �޼���
    {
        if (!MoneyManager.Instance.SpendMoney(summonCost))
        {
            Debug.LogWarning("Not enough money to summon a hero.");
            return null; // ���� �����ϸ� ��ȯ���� ����
        }

        float totalProbability = 0f;
        List<CharacterData> summonableHeroes = new List<CharacterData>();

        foreach (var hero in allHeroes)
        {
            if (hero.heroGrade.isSummonable)
            {
                totalProbability += hero.heroGrade.summonProbability;
                summonableHeroes.Add(hero);
            }
        }

        if (totalProbability == 0f || summonableHeroes.Count == 0)
        {
            Debug.LogWarning("No summonable heroes available.");
            return null;
        }

        float randomPoint = UnityEngine.Random.value * totalProbability;

        foreach (var hero in summonableHeroes)
        {
            if (randomPoint < hero.heroGrade.summonProbability)
            {
                Vector3 spawnPosition = GetNextAvailableSpawnPoint();
                if (spawnPosition != Vector3.zero)
                {
                    // ���� �ν��Ͻ� ���� �� �ʱ�ȭ
                    GameObject heroInstance = Instantiate(hero.heroPrefab, spawnPosition, Quaternion.identity, spawnParent);

                    // ������ ���
                    CharacterData clonedHeroData = hero.Clone();
                    heroInstance.GetComponent<CharacterInfo>().characterData = clonedHeroData;
                    
                    // �ش� Ÿ���� ������ �ݿ�
                    UpgradeSystem upgradeSystem = FindObjectOfType<UpgradeSystem>();
                    int level = upgradeSystem.GetUpgradeLevel(clonedHeroData.selectedType);

                    // ���� �ݿ� �� ���� ����
                    for (int i = 1; i < level; i++)
                    {
                        heroInstance.GetComponent<CharacterInfo>().LevelUp(); // ���� �ݿ�
                    }
                    // �߰�: infoPanel, GraphicRaycaster, EventSystem �Ҵ�
                    CharacterInfoDisplay infoDisplay = heroInstance.GetComponent<CharacterInfoDisplay>();
                    if (infoDisplay != null)
                    {
                        infoDisplay.infoPanel = infoPanel; // InfoPanel�� �Ҵ�
                        infoDisplay.eventSystem = mainEventSystem; // GraphicRaycaster�� EventSystem �Ҵ�
                        infoDisplay.uiRaycaster = mainGraphicRaycaster;
                    }
                    // �߰�: UnitClickHandler �Ҵ�
                 

                    summonedHeroInstances.Add(heroInstance);
                    if(hero.heroGrade.gradeName == "����")
                    {
                        summonHeroeText.text = "���� ����";
                        summonHeroeText.color = Color.blue;
                        StartCoroutine(SummonText());
                    }
                    else if(hero.heroGrade.gradeName == "����")
                    {
                        summonHeroeText.text = "���� ����";
                        summonHeroeText.color = Color.yellow;
                        StartCoroutine(SummonText());
                    }
                    // ItemManager�� ���� Ȱ��ȭ�� ������ ȿ�� ����
                    ItemManager.Instance.ApplyActiveItems(heroInstance.GetComponent<CharacterInfo>());
                    Debug.Log($"Summoned Hero: {hero.heroName} at level {level}");

                    // ��ȯ ��� 5% ����
                    summonCost = Mathf.CeilToInt(summonCost * 1.05f);

                    UpdateUI();
                    return heroInstance;
                }
                else
                {
                    Debug.LogWarning("No available spawn points.");
                    return null;
                }
            }
            else
            {
                randomPoint -= hero.heroGrade.summonProbability;
            }
        }

        Debug.LogWarning("Failed to summon a hero. This should not happen.");
        return null;
    }
    IEnumerator SummonText()
    {
       summonHeroeText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        summonHeroeText.gameObject.SetActive(false);
    }
    private Vector3 GetNextAvailableSpawnPoint() // ���� ��� ������ ���� ��ġ�� �������� �޼���
    {
        for (int j = 0; j < cols; j++) // ���� �켱���� ��ȸ
        {
            for (int i = 0; i < rows; i++) // ���� ��ȸ
            {
                if (!occupiedPoints[i, j]) // ���� ��ġ�� ��� �ִ��� Ȯ��
                {
                    Vector3 spawnPoint = spawnPoints[i, j]; // ���� ��ġ ��������
                    occupiedPoints[i, j] = true; // ��ġ�� ���� ���·� ����
                    return spawnPoint;
                }
            }
        }

        return Vector3.zero; // ���� ���� ����Ʈ�� ���� ���
    }

    public void UpdateHeroPosition(GameObject hero, Vector3 newPosition, Vector3 originalPosition)
    {
        int newRow = -1;  // ���ο� ��ġ�� �� �ε��� �ʱ�ȭ
        int newCol = -1;  // ���ο� ��ġ�� �� �ε��� �ʱ�ȭ
        int oldRow = -1;  // ���� ��ġ�� �� �ε��� �ʱ�ȭ
        int oldCol = -1;  // ���� ��ġ�� �� �ε��� �ʱ�ȭ
        float minDistance = float.MaxValue;  // �ּ� �Ÿ� �ʱ�ȭ

        // ���� ��ġ�� �ε��� ã��
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (spawnPoints[i, j] == originalPosition)
                {
                    oldRow = i;
                    oldCol = j;
                    break;
                }
            }
        }

        // ���ο� ��ġ�� ���� ����� �׸��� ����Ʈ ã��
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                float distance = Vector3.Distance(newPosition, spawnPoints[i, j]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    newRow = i;
                    newCol = j;
                }
            }
        }

        // ���ο� ��ġ�� ��ȿ���� Ȯ��
        if (newRow != -1 && newCol != -1)
        {
            // ���� ��ġ ����
            if (oldRow != -1 && oldCol != -1)
            {
                occupiedPoints[oldRow, oldCol] = false;
            }

            // �� ��ġ�� �ٸ� ������ �ִ��� Ȯ��
            if (occupiedPoints[newRow, newCol])
            {
                // �ٸ� ������ ��ġ ��ȯ
                GameObject otherHero = null;
                foreach (var summonedHero in summonedHeroInstances)
                {
                    if (summonedHero.transform.position == spawnPoints[newRow, newCol])
                    {
                        otherHero = summonedHero;
                        break;
                    }
                }

                if (otherHero != null)
                {
                    otherHero.transform.position = originalPosition; // �ٸ� ������ ���� ��ġ�� �̵�
                    occupiedPoints[oldRow, oldCol] = true; // ���� ��ġ�� ���� ���·� ����
                }
            }

            // �� ��ġ�� �̵�
            hero.transform.position = spawnPoints[newRow, newCol];
            occupiedPoints[newRow, newCol] = true;
        }
        else
        {
            // ��ȿ�� ��ġ�� ã�� ���� ��� ���� ��ġ�� �ǵ���
            hero.transform.position = originalPosition;
            if (oldRow != -1 && oldCol != -1)
            {
                occupiedPoints[oldRow, oldCol] = true;
            }
        }
    }

    public GameObject CombineHeroes(GameObject hero1, GameObject hero2, GameObject hero3) // ������ �ռ��ϴ� �޼���
    {
        // Ư�� ������ ���� �õ��մϴ�.
        GameObject specialHero = TrySpecialCombine(hero1,hero2,hero3);
        if (specialHero != null)
        {
            Debug.Log("Special combination successful.");
            return specialHero;
        }
        Debug.Log("Special combination failed, trying normal combination.");


        // Ư�� ������ ������ ���, �Ϲ� �ռ� ������ �����մϴ�.
        CharacterData data1 = hero1.GetComponent<CharacterInfo>().characterData;
        CharacterData data2 = hero2.GetComponent<CharacterInfo>().characterData;
        CharacterData data3 = hero3.GetComponent<CharacterInfo>().characterData;

        // �� ������ ��ް� Ÿ���� �������� Ȯ��
        if (data1.heroGrade == data2.heroGrade && data2.heroGrade == data3.heroGrade &&
            data1.selectedType == data2.selectedType && data2.selectedType == data3.selectedType)
        {
            // ���� ��޿��� �ռ� ������ ���� ����� �ִ��� Ȯ��
            HeroGrade[] nextGrades = data1.heroGrade.combinableFrom;

            if (nextGrades != null && nextGrades.Length > 0)
            {
                // ���� ��� �� �ϳ��� �������� ����
                HeroGrade selectedNextGrade = nextGrades[UnityEngine.Random.Range(0, nextGrades.Length)];

                // �ش� ����� ���� �� �ϳ��� �������� ����
                List<CharacterData> nextGradeHeroes = new List<CharacterData>();
                foreach (var hero in allHeroes)
                {
                    if (hero.heroGrade == selectedNextGrade)
                    {
                        nextGradeHeroes.Add(hero);
                    }
                }

                if (nextGradeHeroes.Count > 0)
                {
                    CharacterData selectedHeroData = nextGradeHeroes[UnityEngine.Random.Range(0, nextGradeHeroes.Count)];
                    Vector3 spawnPosition = hero1.transform.position; // ���� ������ ��ġ�� ����

                    // ���� �������� �����ߴ� ��ġ ����
                    ClearOccupiedPoint(hero1.transform.position);
                    ClearOccupiedPoint(hero2.transform.position);
                    ClearOccupiedPoint(hero3.transform.position);

                    // �ռ��� ���� �ν��Ͻ� ���� �� �ʱ�ȭ
                    GameObject combinedHeroInstance = Instantiate(selectedHeroData.heroPrefab, spawnPosition, Quaternion.identity, spawnParent);
                    combinedHeroInstance.GetComponent<CharacterInfo>().characterData = selectedHeroData.Clone(); // ���� ������ �Ҵ� (������ ���)
                    CharacterInfoDisplay infoDisplay = combinedHeroInstance.GetComponent<CharacterInfoDisplay>();
                    if (infoDisplay != null)
                    {
                        infoDisplay.infoPanel = infoPanel; // InfoPanel�� �Ҵ�
                        infoDisplay.eventSystem = mainEventSystem; // GraphicRaycaster�� EventSystem �Ҵ�
                        infoDisplay.uiRaycaster = mainGraphicRaycaster;
                    }
                    // �߰�: UnitClickHandler �Ҵ�
                

                    // �� ������ ��ġ�� ���� ������ �� �ϳ��� ��ġ�� ����
                    combinedHeroInstance.transform.position = spawnPosition;

                    // ���� �ռ��� ������ ��ġ�� ���� ���·� ����
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            if (spawnPoints[i, j] == spawnPosition)
                            {
                                occupiedPoints[i, j] = true;
                                break;
                            }
                        }
                    }

                    summonedHeroInstances.Add(combinedHeroInstance);

                    // ���� ������ ����
                    summonedHeroInstances.Remove(hero1);
                    summonedHeroInstances.Remove(hero2);
                    summonedHeroInstances.Remove(hero3);
                    Destroy(hero1);
                    Destroy(hero2);
                    Destroy(hero3);
                    UpdateUI();
                    return combinedHeroInstance;
                }
                else
                {
                    Debug.LogWarning("No heroes found for the selected next grade.");
                    return null;
                }
            }
            else
            {
                Debug.LogWarning("No combinable grades found for this hero grade.");
                return null;
            }
        }

        Debug.LogWarning("Hero types or grades do not match for combination.");
        
        return null; // �ռ� ���� ��
    }

    public GameObject TrySpecialCombine(GameObject hero1, GameObject hero2, GameObject hero3)
    {
        Debug.Log("����� ���� �õ� ��");
        Debug.Log(hero1.gameObject.name + hero2.gameObject.name + hero3.gameObject.name);
        List<GameObject> heroesToCombine = new List<GameObject> { hero1, hero2, hero3 };

        foreach (var specialCombination in specialCombinations)
        {
            List<GameObject> matchingHeroes = new List<GameObject>();

            // ��� �ʿ� �������� �Ű������� ���� ���� ����Ʈ�� �ִ��� Ȯ���մϴ�.
            foreach (var reqHero in specialCombination.requiredHeroes)
            {
                foreach (var hero in heroesToCombine)
                {
                    var heroData = hero.GetComponent<CharacterInfo>().characterData;
                    if (heroData.heroName == reqHero.heroName)  // ���⼭ �ʵ带 ����� �񱳷� ����
                    {
                        matchingHeroes.Add(hero);
                        break;
                    }
                }
            }
            Debug.Log("����� ���� �õ� ��2");
            Debug.Log(specialCombination.requiredHeroes.Count);
            Debug.Log(matchingHeroes.Count);
            // �ʿ��� �������� ��� �Ű������� ���� ����Ʈ���� �߰ߵǾ����� Ȯ���մϴ�.
            if (matchingHeroes.Count == specialCombination.requiredHeroes.Count)
            {
                Debug.Log("����� ���� ����");
                Vector3 spawnPosition = matchingHeroes[0].transform.position;

                // ���� �������� �����ߴ� ��ġ ����
                foreach (var hero in matchingHeroes)
                {
                    ClearOccupiedPoint(hero.transform.position);
                    summonedHeroInstances.Remove(hero);
                    Destroy(hero);

                    
                }

                // Ư�� ���� ���� ����
                GameObject combinedHeroInstance = Instantiate(specialCombination.resultHero.heroPrefab, spawnPosition, Quaternion.identity, spawnParent);
                combinedHeroInstance.GetComponent<CharacterInfo>().characterData = specialCombination.resultHero.Clone();
                CharacterInfoDisplay infoDisplay = combinedHeroInstance.GetComponent<CharacterInfoDisplay>();
                if (infoDisplay != null)
                {
                    infoDisplay.infoPanel = infoPanel; // InfoPanel�� �Ҵ�
                    infoDisplay.eventSystem = mainEventSystem; // GraphicRaycaster�� EventSystem �Ҵ�
                    infoDisplay.uiRaycaster = mainGraphicRaycaster;
                }
          

                // �� ������ ��ġ�� ���� ���·� ����
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (spawnPoints[i, j] == spawnPosition)
                        {
                            occupiedPoints[i, j] = true;
                            break;
                        }
                    }
                }

                summonedHeroInstances.Add(combinedHeroInstance);

                UpdateUI();
                return combinedHeroInstance;
            }
        }

        Debug.Log("����� ���� ����");
        return null; // Ư�� ���� ���� ��
    }
    private void ClearOccupiedPoint(Vector3 position)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (spawnPoints[i, j] == position)
                {
                    occupiedPoints[i, j] = false; // �ش� ��ġ�� ���� ���� ����
                    return;
                }
            }
        }
    }

    public void TryCombineHero(GameObject selectedHero)
    {
        CharacterInfo selectedHeroInfo = selectedHero.GetComponent<CharacterInfo>();

        // Ư�� ���տ� �ʿ��� �������� ���� Ȯ���մϴ�.
        foreach (var specialCombination in specialCombinations)
        {
            // specialCombination�� �ʿ��� �������� ��ȯ�� ���� ����Ʈ�� �ִ��� Ȯ��
            List<GameObject> matchingHeroes = new List<GameObject>();
            foreach (CharacterData requiredHero in specialCombination.requiredHeroes)
            {
                foreach (var hero in summonedHeroInstances)
                {
                    Debug.Log("���ƾƾƾ�");
                    CharacterInfo heroInfo = hero.GetComponent<CharacterInfo>();
                    if (heroInfo.characterData.heroName == requiredHero.heroName)
                    {
                        matchingHeroes.Add(hero);
                        break;
                    }
                }
            }

            // ��� �ʿ��� �������� ����Ʈ�� ���ԵǾ����� Ȯ��
            if (matchingHeroes.Count == specialCombination.requiredHeroes.Count)
            {
                // Ư�� ������ �õ��մϴ�.
                GameObject specialHero = TrySpecialCombine(matchingHeroes[0], matchingHeroes[1], matchingHeroes[2]);

                if (specialHero != null)
                {
                    Debug.Log("Special combination successful.");
                    return;
                }
            }
        }

        // Ư�� ������ ������ ���, �Ϲ� ������ �õ��մϴ�.
        List<GameObject> normalMatchingHeroes = new List<GameObject>();
        foreach (var hero in summonedHeroInstances)
        {
            CharacterInfo heroInfo = hero.GetComponent<CharacterInfo>();
            if (heroInfo.characterData.heroGrade == selectedHeroInfo.characterData.heroGrade &&
                heroInfo.characterData.selectedType == selectedHeroInfo.characterData.selectedType)
            {
                normalMatchingHeroes.Add(hero);
            }
        }

        if (normalMatchingHeroes.Count >= 3)
        {
            CombineHeroes(normalMatchingHeroes[0], normalMatchingHeroes[1], normalMatchingHeroes[2]);
        }
        else
        {
            Debug.Log("Not enough matching heroes to combine.");
        }
    }
    public void UpdateUI()
    {
        // ��ȯ �䱸ġ ������Ʈ
        if (summonCostText != null)
        {
            summonCostText.text = summonCost.ToString();
        }

        // ���� ��ȯ�� ������ �� ������Ʈ
        if (summonHeroes != null)
        {
            summonHeroes.text = summonedHeroInstances.Count.ToString() + "/21";
        }
    }
    public CharacterInfo FindMVP() // mvp���� �ż���
    {
        CharacterInfo mvp = null;
        float maxDamage = 0f;

        foreach (GameObject heroInstance in summonedHeroInstances)
        {
            // �� ������ CharacterInfo ������Ʈ�� ������
            CharacterInfo character = heroInstance.GetComponent<CharacterInfo>();

            if (character != null && character.totalDamageDealt > maxDamage)
            {
                maxDamage = character.totalDamageDealt;
                mvp = character; // ���� ���� �������� ����� ĳ���͸� mvp�� ����
            }
        }

        if (mvp != null)
        {
            Debug.Log($"MVP: damage dealt!");
        }
        else
        {
            Debug.Log("No summoned heroes found for MVP selection.");
        }

        return mvp;
    }

}
