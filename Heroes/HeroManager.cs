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
/// 디펜스존에서의 히어로 소환 및 조합을 관할합니다.
/// </summary>
public class HeroManager : MonoBehaviour
{
    public static HeroManager Instance { get; private set; } // 싱글턴 인스턴스

    public List<CharacterData> allHeroes; // 모든 영웅 데이터 리스트
    public List<GameObject> summonedHeroInstances; // 소환된 영웅 인스턴스 리스트
    
    public List<SpecialCombination> specialCombinations; // 특수 조합 리스트
    public Transform spawnParent; // 스폰 위치의 부모 오브젝트
    public int cols = 7; // 열의 개수
    public int rows = 3; // 행의 개수
    private Vector3[,] spawnPoints; // 스폰 위치 배열
    private bool[,] occupiedPoints; // 스폰 위치 점유 상태 배열

    public int summonCost = 10; // 영웅 소환에 필요한 초기 돈
    public CharacterInfoPanel infoPanel; // 하이어라키에 존재하는 InfoPanel
    public GraphicRaycaster mainGraphicRaycaster;
    public EventSystem mainEventSystem;
    [SerializeField] private TextMeshProUGUI summonCostText; //  소환 요구치
    [SerializeField] private TextMeshProUGUI summonHeroes; // 현재 소환된 영웅의 수
    [SerializeField] private TextMeshProUGUI summonHeroeText; // 현재 소환된 영웅의 수
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 인스턴스 설정
            
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재할 경우, 새로운 인스턴스를 파괴
        }
    }
    private void OnEnable()
    { // 씬 전환 후 SpawnTransform을 찾아서 spawnParent에 할당
        GameObject spawnObject = GameObject.Find("SpawnTransform");
        if (spawnObject != null)
        {
            spawnParent = spawnObject.transform;
        }
        summonedHeroInstances.Clear();
        summonCost = 10;
        InitializeSpawnPoints(); // 스폰 위치 초기화
        UpdateUI();
        Debug.Log("히어로 매니저 초기화");
    }
    void Start()
    {
       
        
    }

    public void InitializeSpawnPoints() // 스폰 위치를 초기화하는 메서드
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

    public void Summon() // 소환 버튼 클릭 시 호출되는 메서드
    {
        SummonHero();
    }

    public GameObject SummonHero() // 영웅을 소환하는 메서드
    {
        if (!MoneyManager.Instance.SpendMoney(summonCost))
        {
            Debug.LogWarning("Not enough money to summon a hero.");
            return null; // 돈이 부족하면 소환하지 않음
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
                    // 영웅 인스턴스 생성 및 초기화
                    GameObject heroInstance = Instantiate(hero.heroPrefab, spawnPosition, Quaternion.identity, spawnParent);

                    // 복제본 사용
                    CharacterData clonedHeroData = hero.Clone();
                    heroInstance.GetComponent<CharacterInfo>().characterData = clonedHeroData;
                    
                    // 해당 타입의 레벨을 반영
                    UpgradeSystem upgradeSystem = FindObjectOfType<UpgradeSystem>();
                    int level = upgradeSystem.GetUpgradeLevel(clonedHeroData.selectedType);

                    // 레벨 반영 및 스탯 적용
                    for (int i = 1; i < level; i++)
                    {
                        heroInstance.GetComponent<CharacterInfo>().LevelUp(); // 레벨 반영
                    }
                    // 추가: infoPanel, GraphicRaycaster, EventSystem 할당
                    CharacterInfoDisplay infoDisplay = heroInstance.GetComponent<CharacterInfoDisplay>();
                    if (infoDisplay != null)
                    {
                        infoDisplay.infoPanel = infoPanel; // InfoPanel을 할당
                        infoDisplay.eventSystem = mainEventSystem; // GraphicRaycaster와 EventSystem 할당
                        infoDisplay.uiRaycaster = mainGraphicRaycaster;
                    }
                    // 추가: UnitClickHandler 할당
                 

                    summonedHeroInstances.Add(heroInstance);
                    if(hero.heroGrade.gradeName == "태초")
                    {
                        summonHeroeText.text = "태초 등장";
                        summonHeroeText.color = Color.blue;
                        StartCoroutine(SummonText());
                    }
                    else if(hero.heroGrade.gradeName == "전설")
                    {
                        summonHeroeText.text = "전설 등장";
                        summonHeroeText.color = Color.yellow;
                        StartCoroutine(SummonText());
                    }
                    // ItemManager를 통해 활성화된 아이템 효과 적용
                    ItemManager.Instance.ApplyActiveItems(heroInstance.GetComponent<CharacterInfo>());
                    Debug.Log($"Summoned Hero: {hero.heroName} at level {level}");

                    // 소환 비용 5% 증가
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
    private Vector3 GetNextAvailableSpawnPoint() // 다음 사용 가능한 스폰 위치를 가져오는 메서드
    {
        for (int j = 0; j < cols; j++) // 열을 우선으로 순회
        {
            for (int i = 0; i < rows; i++) // 행을 순회
            {
                if (!occupiedPoints[i, j]) // 현재 위치가 비어 있는지 확인
                {
                    Vector3 spawnPoint = spawnPoints[i, j]; // 스폰 위치 가져오기
                    occupiedPoints[i, j] = true; // 위치를 점유 상태로 변경
                    return spawnPoint;
                }
            }
        }

        return Vector3.zero; // 가용 스폰 포인트가 없을 경우
    }

    public void UpdateHeroPosition(GameObject hero, Vector3 newPosition, Vector3 originalPosition)
    {
        int newRow = -1;  // 새로운 위치의 행 인덱스 초기화
        int newCol = -1;  // 새로운 위치의 열 인덱스 초기화
        int oldRow = -1;  // 원래 위치의 행 인덱스 초기화
        int oldCol = -1;  // 원래 위치의 열 인덱스 초기화
        float minDistance = float.MaxValue;  // 최소 거리 초기화

        // 원래 위치의 인덱스 찾기
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

        // 새로운 위치에 가장 가까운 그리드 포인트 찾기
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

        // 새로운 위치가 유효한지 확인
        if (newRow != -1 && newCol != -1)
        {
            // 이전 위치 비우기
            if (oldRow != -1 && oldCol != -1)
            {
                occupiedPoints[oldRow, oldCol] = false;
            }

            // 새 위치에 다른 영웅이 있는지 확인
            if (occupiedPoints[newRow, newCol])
            {
                // 다른 영웅과 위치 교환
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
                    otherHero.transform.position = originalPosition; // 다른 영웅을 원래 위치로 이동
                    occupiedPoints[oldRow, oldCol] = true; // 원래 위치를 점유 상태로 변경
                }
            }

            // 새 위치로 이동
            hero.transform.position = spawnPoints[newRow, newCol];
            occupiedPoints[newRow, newCol] = true;
        }
        else
        {
            // 유효한 위치를 찾지 못한 경우 원래 위치로 되돌림
            hero.transform.position = originalPosition;
            if (oldRow != -1 && oldCol != -1)
            {
                occupiedPoints[oldRow, oldCol] = true;
            }
        }
    }

    public GameObject CombineHeroes(GameObject hero1, GameObject hero2, GameObject hero3) // 영웅을 합성하는 메서드
    {
        // 특수 조합을 먼저 시도합니다.
        GameObject specialHero = TrySpecialCombine(hero1,hero2,hero3);
        if (specialHero != null)
        {
            Debug.Log("Special combination successful.");
            return specialHero;
        }
        Debug.Log("Special combination failed, trying normal combination.");


        // 특수 조합이 실패한 경우, 일반 합성 로직을 수행합니다.
        CharacterData data1 = hero1.GetComponent<CharacterInfo>().characterData;
        CharacterData data2 = hero2.GetComponent<CharacterInfo>().characterData;
        CharacterData data3 = hero3.GetComponent<CharacterInfo>().characterData;

        // 세 영웅의 등급과 타입이 동일한지 확인
        if (data1.heroGrade == data2.heroGrade && data2.heroGrade == data3.heroGrade &&
            data1.selectedType == data2.selectedType && data2.selectedType == data3.selectedType)
        {
            // 현재 등급에서 합성 가능한 다음 등급이 있는지 확인
            HeroGrade[] nextGrades = data1.heroGrade.combinableFrom;

            if (nextGrades != null && nextGrades.Length > 0)
            {
                // 다음 등급 중 하나를 랜덤으로 선택
                HeroGrade selectedNextGrade = nextGrades[UnityEngine.Random.Range(0, nextGrades.Length)];

                // 해당 등급의 영웅 중 하나를 랜덤으로 선택
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
                    Vector3 spawnPosition = hero1.transform.position; // 기존 영웅의 위치에 생성

                    // 기존 영웅들이 점유했던 위치 해제
                    ClearOccupiedPoint(hero1.transform.position);
                    ClearOccupiedPoint(hero2.transform.position);
                    ClearOccupiedPoint(hero3.transform.position);

                    // 합성된 영웅 인스턴스 생성 및 초기화
                    GameObject combinedHeroInstance = Instantiate(selectedHeroData.heroPrefab, spawnPosition, Quaternion.identity, spawnParent);
                    combinedHeroInstance.GetComponent<CharacterInfo>().characterData = selectedHeroData.Clone(); // 영웅 데이터 할당 (복제본 사용)
                    CharacterInfoDisplay infoDisplay = combinedHeroInstance.GetComponent<CharacterInfoDisplay>();
                    if (infoDisplay != null)
                    {
                        infoDisplay.infoPanel = infoPanel; // InfoPanel을 할당
                        infoDisplay.eventSystem = mainEventSystem; // GraphicRaycaster와 EventSystem 할당
                        infoDisplay.uiRaycaster = mainGraphicRaycaster;
                    }
                    // 추가: UnitClickHandler 할당
                

                    // 새 영웅의 위치를 이전 영웅들 중 하나의 위치로 설정
                    combinedHeroInstance.transform.position = spawnPosition;

                    // 새로 합성된 영웅의 위치를 점유 상태로 설정
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

                    // 기존 영웅들 제거
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
        
        return null; // 합성 실패 시
    }

    public GameObject TrySpecialCombine(GameObject hero1, GameObject hero2, GameObject hero3)
    {
        Debug.Log("스페셜 조합 시도 중");
        Debug.Log(hero1.gameObject.name + hero2.gameObject.name + hero3.gameObject.name);
        List<GameObject> heroesToCombine = new List<GameObject> { hero1, hero2, hero3 };

        foreach (var specialCombination in specialCombinations)
        {
            List<GameObject> matchingHeroes = new List<GameObject>();

            // 모든 필요 영웅들이 매개변수로 받은 영웅 리스트에 있는지 확인합니다.
            foreach (var reqHero in specialCombination.requiredHeroes)
            {
                foreach (var hero in heroesToCombine)
                {
                    var heroData = hero.GetComponent<CharacterInfo>().characterData;
                    if (heroData.heroName == reqHero.heroName)  // 여기서 필드를 사용한 비교로 변경
                    {
                        matchingHeroes.Add(hero);
                        break;
                    }
                }
            }
            Debug.Log("스페셜 조합 시도 중2");
            Debug.Log(specialCombination.requiredHeroes.Count);
            Debug.Log(matchingHeroes.Count);
            // 필요한 영웅들이 모두 매개변수로 받은 리스트에서 발견되었는지 확인합니다.
            if (matchingHeroes.Count == specialCombination.requiredHeroes.Count)
            {
                Debug.Log("스페셜 조합 성공");
                Vector3 spawnPosition = matchingHeroes[0].transform.position;

                // 기존 영웅들이 점유했던 위치 해제
                foreach (var hero in matchingHeroes)
                {
                    ClearOccupiedPoint(hero.transform.position);
                    summonedHeroInstances.Remove(hero);
                    Destroy(hero);

                    
                }

                // 특수 조합 영웅 생성
                GameObject combinedHeroInstance = Instantiate(specialCombination.resultHero.heroPrefab, spawnPosition, Quaternion.identity, spawnParent);
                combinedHeroInstance.GetComponent<CharacterInfo>().characterData = specialCombination.resultHero.Clone();
                CharacterInfoDisplay infoDisplay = combinedHeroInstance.GetComponent<CharacterInfoDisplay>();
                if (infoDisplay != null)
                {
                    infoDisplay.infoPanel = infoPanel; // InfoPanel을 할당
                    infoDisplay.eventSystem = mainEventSystem; // GraphicRaycaster와 EventSystem 할당
                    infoDisplay.uiRaycaster = mainGraphicRaycaster;
                }
          

                // 새 영웅의 위치를 점유 상태로 설정
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

        Debug.Log("스페셜 조합 실패");
        return null; // 특수 조합 실패 시
    }
    private void ClearOccupiedPoint(Vector3 position)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (spawnPoints[i, j] == position)
                {
                    occupiedPoints[i, j] = false; // 해당 위치의 점유 상태 해제
                    return;
                }
            }
        }
    }

    public void TryCombineHero(GameObject selectedHero)
    {
        CharacterInfo selectedHeroInfo = selectedHero.GetComponent<CharacterInfo>();

        // 특수 조합에 필요한 영웅들을 먼저 확인합니다.
        foreach (var specialCombination in specialCombinations)
        {
            // specialCombination에 필요한 영웅들이 소환된 영웅 리스트에 있는지 확인
            List<GameObject> matchingHeroes = new List<GameObject>();
            foreach (CharacterData requiredHero in specialCombination.requiredHeroes)
            {
                foreach (var hero in summonedHeroInstances)
                {
                    Debug.Log("ㅇ아아아아");
                    CharacterInfo heroInfo = hero.GetComponent<CharacterInfo>();
                    if (heroInfo.characterData.heroName == requiredHero.heroName)
                    {
                        matchingHeroes.Add(hero);
                        break;
                    }
                }
            }

            // 모든 필요한 영웅들이 리스트에 포함되었는지 확인
            if (matchingHeroes.Count == specialCombination.requiredHeroes.Count)
            {
                // 특수 조합을 시도합니다.
                GameObject specialHero = TrySpecialCombine(matchingHeroes[0], matchingHeroes[1], matchingHeroes[2]);

                if (specialHero != null)
                {
                    Debug.Log("Special combination successful.");
                    return;
                }
            }
        }

        // 특수 조합이 실패한 경우, 일반 조합을 시도합니다.
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
        // 소환 요구치 업데이트
        if (summonCostText != null)
        {
            summonCostText.text = summonCost.ToString();
        }

        // 현재 소환된 영웅의 수 업데이트
        if (summonHeroes != null)
        {
            summonHeroes.text = summonedHeroInstances.Count.ToString() + "/21";
        }
    }
    public CharacterInfo FindMVP() // mvp선정 매서드
    {
        CharacterInfo mvp = null;
        float maxDamage = 0f;

        foreach (GameObject heroInstance in summonedHeroInstances)
        {
            // 각 영웅의 CharacterInfo 컴포넌트를 가져옴
            CharacterInfo character = heroInstance.GetComponent<CharacterInfo>();

            if (character != null && character.totalDamageDealt > maxDamage)
            {
                maxDamage = character.totalDamageDealt;
                mvp = character; // 가장 높은 데미지를 기록한 캐릭터를 mvp로 설정
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
