using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterSpawnManager : MonoBehaviour
{
    public static MonsterSpawnManager Instance { get; private set; } // 싱글턴 인스턴스

    
    public List<GameObject> spawnList = new List<GameObject>(); // 스폰할 몬스터 리스트
    public List<GameObject> bossSpawnList = new List<GameObject>(); // 스폰할 보스 리스트
 
    public Transform spawnPoint; // 몬스터가 스폰될 위치
    public float spawnInterval = 0.5f; // 몬스터 스폰 간격
    public int normalRoundSpawnCount = 20; // 일반 라운드에서 스폰할 몬스터 수
    public float normalRoundDuration = 20.0f; // 일반 라운드 시간 (초)
    public float bossRoundDuration = 45.0f; // 보스 라운드 시간 (초)
    public List<Transform> waypoints = new List<Transform>(); // 몬스터들이 이동할 경로 리스트

    public int currentMonsterCount = 0; // 소환되어 있는 몬스터 수
   
    public int currentRound = 1; // 현재 라운드
    public TextMeshProUGUI roundTimerText; 
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI waypointTaxt;
    public TextMeshProUGUI tapStart;
    public TextMeshProUGUI currenMonsterCountText;
    private bool isSpawning = false; // 스폰 중인지 여부

    public List<Monster> spawnedMonsters = new List<Monster>(); // 스폰된 몬스터 리스트

    public GameObject panel; // 성공 및 실패시 등장하는 패널
    public TextMeshProUGUI resultMessage;
    public TextMeshProUGUI buttonMessage;

    private float currentSpeedReductionPercentage = 0f; // 현재 적용된 속도 감소 퍼센트

    public bool isGameOver = false; // 게임 오버 상태를 나타내는 변수
    public bool isStart = false;
    public bool isCleared = false;
    private int difficultyLevel = 1; // 난이도 레벨
    private void OnEnable()
    {
    
    }
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

    void Start()
    {
       
        // 특정 웨이포인트의 클리어 횟수를 불러옴
        SaveLoadManager.Instance.GetClearedTimesForWaypoint(Player.Instance.currentWaypointIndex, clearedTimes =>
        {
            // 클리어 횟수에 따라 난이도를 설정
            int newDifficultyLevel;
            if (GuideManager.Instance.isFirstTime) // 첫 접속일 경우
            {
                newDifficultyLevel = 0; // 첫 접속 시 난이도를 0으로 설정
            }
            else if (clearedTimes >= 2)
            {
                newDifficultyLevel = 3; // 클리어 횟수가 2회 이상일 경우
            }
            else if (clearedTimes >= 1)
            {
                newDifficultyLevel = 2; // 클리어 횟수가 1회 이상일 경우
            }
            else
            {
                newDifficultyLevel = 1; // 클리어 횟수가 0회일 경우
            }

            difficultyLevel = newDifficultyLevel;

            // 난이도에 따른 라운드 설정
            SaveLoadManager.Instance.SetDifficultyForWaypoint(Player.Instance.currentWaypointIndex, difficultyLevel);
            
            // 스폰 루틴 시작
            //StartCoroutine(SpawnRoutine());
            waypointTaxt.text = $"1-{Player.Instance.currentWaypointIndex}";
        });
        if (SaveLoadManager.Instance == null)
        {
            Debug.LogError("SaveLoadManager.Instance is null. Make sure it is initialized correctly.");
            return;
        }

        if (Player.Instance == null)
        {
            Debug.LogError("Player.Instance is null. Make sure it is initialized correctly.");
            return;
        }

    }

    private string FormatDamage(float damage)
    {
        if (damage >= 1000000)
        {
            return $"{damage / 1000000f:0.#}M"; // 백만 단위로 변환
        }
        else if (damage >= 1000)
        {
            return $"{damage / 1000f:0.#}K"; // 천 단위로 변환
        }
        else
        {
            return damage.ToString(); // 천 미만은 그대로 출력
        }
    }
    void OpenPanel(bool isSuccess)
    {
        panel.SetActive(true); // 패널 활성화
        CharacterInfo mvp = HeroManager.Instance.FindMVP();
        // 딜량을 축약해서 표시 (K, M)
        string formattedDamage = FormatDamage(mvp.totalDamageDealt);
        // 성공 또는 실패에 따라 메시지를 설정
        if (isSuccess)
        {
           
            resultMessage.text = $"1-{Player.Instance.currentWaypointIndex} 성공!!\n\n\n\n\n보상\n\n금화 : {difficultyLevel*1000}\n경험치 : {difficultyLevel*500}\n\nMVP\n\n{mvp.characterData.heroName} 딜량 : {formattedDamage}"; // 성공 메시지
            buttonMessage.text = "획득";
            resultMessage.color = Color.white; // 
        }
        else
        {
            resultMessage.text = $"1-{Player.Instance.currentWaypointIndex} 실패...\n\n\n\n\n\n\n 다음에 다시.. \n\nMVP\n\n{mvp.characterData.heroName} 딜량 : {formattedDamage}" ;
            buttonMessage.text = "퇴각";
            resultMessage.color = Color.white; //
        }
    }
    public void LoadScene() // 코드 어필 할꺼임 >> 씬매니저 이벤트
    {
        panel.gameObject.SetActive(false);
        HeroManager.Instance.summonedHeroInstances.Clear();
        HeroManager.Instance.summonCost = 10;
        HeroManager.Instance.InitializeSpawnPoints(); // 스폰 위치 초기화
        HeroManager.Instance.UpdateUI();
        Player.Instance.gameObject.SetActive(true);
        Player.Instance.canvas.SetActive(true);
        Player.Instance.isChallenge = false;
        WaypointManager.Instance.gameObject.SetActive(true);
        CameraFollow.instance.gameObject.SetActive(true);
        DD6.instance.gameObject.SetActive(true);
        Debug.Log("히어로 매니저 초기화");
        
        // 씬 로드 이벤트 구독
        if (isCleared) // 클리어했다면 
        {
            SceneManager.sceneLoaded += OnSceneLoaded; // 이벤트 구독
        }
        

        // **SceneLoader를 사용하여 페이드 인/아웃과 함께 씬 로드**
        SceneLoader.Instance.LoadScene("PlayerScene");
    }

  


    // 씬이 로드된 후 호출되는 함수 클리어시 즉시 보상을 지급하기 위함
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드된 후 BuildingTaxManager에 접근하여 데이터 설정
        BuildingTaxManager[] buildingManagers = FindObjectsOfType<BuildingTaxManager>();

        foreach (var manager in buildingManagers)
        {
            if (manager.waypointIndex == Player.Instance.currentWaypointIndex)
            {
                // 필요한 데이터 설정
                manager.moneyReadyForWaypoint = true;
                //manager.CollectMoneyForWaypoint(); // 이 라인을 주석 해제하면 자동 수거 가능
                Debug.Log("즉시수거가능");
            }
        }

        // 한 번만 동작하게 하기 위해 이벤트 구독 해제 (필요시 주석 처리 가능)
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Update()
    {
        if (currentMonsterCount >= 100 && !isGameOver) // 현재 몬스터 수가 100에 도달하면 게임 오버 처리
        {
            isGameOver = true;
            OpenPanel(false);
            Debug.Log("Game Over! Too many monsters!");
        }
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !isStart)
        {
            isStart = true;  // 코루틴이 한 번만 실행되도록
            tapStart.gameObject.SetActive(false); // 시작 텍스트 숨김
            StartCoroutine(SpawnRoutine());  // 코루틴 실행
        }
    }

    IEnumerator SpawnRoutine()
    {
        // currentRound를 minRound로 초기화
        GetRoundRangeForCurrentLevel(out int minRound, out int maxRound);
        currentRound = minRound;

        if (difficultyLevel == 0) // 첫 접속 시 간소화된 라운드
        {
            List<int> simplifiedRounds = new List<int> { 1, 10, 20 };

            foreach (int round in simplifiedRounds)
            {
                currentRound = round;

                if (currentRound == 20) // 마지막 라운드 처리
                {
                    RoundText(currentRound);
                    yield return StartCoroutine(HandleFinalRound());
                }
                else if (currentRound % 10 == 0) // 보스 라운드
                {
                    RoundText(currentRound);
                    yield return StartCoroutine(SpawnBossRound());
                }
                else // 일반 라운드
                {
                    RoundText(currentRound);
                    yield return StartCoroutine(SpawnNormalRound());
                }
            }
        }
        else
        {
            while (currentRound <= maxRound && !isGameOver)
            {
                if (currentRound == maxRound) // 마지막 라운드 처리
                {
                    RoundText(currentRound);
                    yield return StartCoroutine(HandleFinalRound());
                }
                else if (currentRound % 10 == 0) // 보스 라운드
                {
                    RoundText(currentRound);
                    yield return StartCoroutine(SpawnBossRound());
                }
                else // 일반 라운드
                {
                    RoundText(currentRound);
                    yield return StartCoroutine(SpawnNormalRound());
                    
                }

                currentRound++;
            }
          
        }
        
        Debug.Log("All rounds for the current level have been completed.");
    }
    void RoundText(int currentRound)
    {
        int displayRound;

        if (difficultyLevel == 1 || difficultyLevel == 0) // 난이도가 0 또는 1일 때는 그대로 표시
        {
            displayRound = currentRound;
        }
        else if (difficultyLevel == 2) // 난이도가 2일 때는 20을 빼서 1~20으로 표시
        {
            displayRound = currentRound - 20;
        }
        else // 난이도가 3일 때는 40을 빼서 1~20으로 표시
        {
            displayRound = currentRound - 40;
        }

        // 라운드를 1~20으로 표기
        roundText.text = $"{displayRound}/20";
    }
    void GetRoundRangeForCurrentLevel(out int minRound, out int maxRound)
    {
        if (difficultyLevel == 0) // 첫 접속일 경우
        {
            minRound = 1;
            maxRound = 20; // 첫 접속 시 라운드 1에서 20까지
            MoneyManager.Instance.InitializeMoney(350);
        }
        else
        {
            switch (difficultyLevel)
            {
                case 1:
                    minRound = 1;
                    maxRound = 20;
                    break;
                case 2:
                    minRound = 21;
                    maxRound = 40;
                    break;
                case 3:
                    minRound = 41;
                    maxRound = 60;
                    break;
                default:
                    minRound = 1;
                    maxRound = 20;
                    break;
            }
        }
    }

    IEnumerator HandleFinalRound()
    {
        if (isGameOver || currentMonsterCount >= 100) // 게임 오버 또는 몬스터 수가 100 이상이면 스폰 중지
        {
            isGameOver = true;
            yield break;
        }
        int bossIndex = (currentRound / 10) - 1; // 현재 라운드에 맞는 보스 인덱스
        SpawnBoss(bossIndex);

        // 1분 대기
        float timer = 60f;
        while (timer > 0f && currentMonsterCount > 0 && !isGameOver)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (currentMonsterCount == 0 && !isGameOver) // 모든 몬스터가 처치되었으면
        {
            HandleLevelCleared();
            OpenPanel(true);
        }
        else // 시간이 다 되었거나 몬스터가 남아있으면
        {
            HandleGameOver();
        }
    }

    void HandleLevelCleared()
    {
        Debug.Log("Level Cleared!");
        isCleared = true;
        AchievementsManager.Instance.totalClearTime++;
        int addExperience = difficultyLevel * 500;
        int addMoney = difficultyLevel * 1000;

        // 경험치와 돈 보상
        Player.Instance.AddExperience(addExperience); // 예시로 1000 경험치 지급
        PlayerMoneyManager.Instance.AddMoney(addMoney); // 예시로 500 돈 지급
        AchievementsManager.Instance.CheckAchievement("클리어 횟수", AchievementsManager.Instance.totalClearTime);
        // 클리어 상태 저장
        SaveLoadManager.Instance.MarkWaypointAsCleared(Player.Instance.currentWaypointIndex);
        // 클리어 횟수 증가
        SaveLoadManager.Instance.IncrementClearedTimesForWaypoint(Player.Instance.currentWaypointIndex);
    }
    void HandleGameOver()
    {
        Debug.Log("Game Over! Handling Game Over...");
        isGameOver = true;
        // 여기에 게임 오버 시 처리할 로직 추가 (씬 전환, UI 표시 등)
        OpenPanel(false);
    }

    IEnumerator SpawnNormalRound()
    {
        int monsterIndex = (currentRound - 1) / 10;
        float roundEndTime = Time.time + normalRoundDuration;

        // 라운드 타이머 업데이트 시작
        StartCoroutine(UpdateRoundTimer(normalRoundDuration));

        for (int i = 0; i < normalRoundSpawnCount; i++)
        {
            if (isGameOver || currentMonsterCount >= 100)
            {
                isGameOver = true;
                OpenPanel(false);
                yield break;
            }

            SpawnMonster(monsterIndex);
            yield return new WaitForSeconds(spawnInterval);

            if (Time.time >= roundEndTime)
                break;
        }

        yield return new WaitForSeconds(normalRoundDuration - (normalRoundSpawnCount * spawnInterval));
    }

    IEnumerator SpawnBossRound()
    {
        // 보스 라운드 시간 업데이트
        StartCoroutine(UpdateRoundTimer(bossRoundDuration));

        if (isGameOver || currentMonsterCount >= 100)
        {
            isGameOver = true;
            OpenPanel(false);
            yield break;
        }

        int bossIndex = (currentRound / 10) - 1;
        SpawnBoss(bossIndex);

        yield return new WaitForSeconds(bossRoundDuration);
    }
    IEnumerator UpdateRoundTimer(float roundDuration)
    {
        float timeRemaining = roundDuration;

        while (timeRemaining > 0f)
        {
            // 남은 시간을 업데이트하여 표시
            timeRemaining -= Time.deltaTime;
            roundTimerText.text = $"남은 시간: {Mathf.Ceil(timeRemaining)} 초";

            yield return null;
        }

        // 라운드 종료 시 타이머 리셋
        roundTimerText.text = "보스!";
    }
    void SpawnMonster(int index)
    {
        if (index >= 0 && index < spawnList.Count)
        {
            GameObject monster = Instantiate(spawnList[index], spawnPoint.position, spawnPoint.rotation);
            Monster monsterComponent = monster.GetComponent<Monster>();
            monsterComponent.waypoints.AddRange(waypoints); // 인스턴시에이트된 프리팹에게 웨이포인트 할당
            currentMonsterCount++; // 현재 몬스터 수 증가
            currenMonsterCountText.text = $"{currentMonsterCount}/100";
            spawnedMonsters.Add(monsterComponent); // 스폰된 몬스터 리스트에 추가
            monsterComponent.ApplyPassiveEffect(currentSpeedReductionPercentage); // 패시브 효과 적용

            // 활성화된 모든 아이템의 효과를 새로 스폰된 몬스터에 적용
            ItemManager.Instance.ApplyActiveItems(monsterComponent);
        }
    }

    void SpawnBoss(int index)
    {
        if (index >= 0 && index < bossSpawnList.Count)
        {
            GameObject monster = Instantiate(bossSpawnList[index], spawnPoint.position, spawnPoint.rotation);
            Monster monsterComponent = monster.GetComponent<Monster>();
            monsterComponent.waypoints.AddRange(waypoints); // 인스턴시에이트된 프리팹에게 웨이포인트 할당
            currentMonsterCount++; // 현재 몬스터 수 증가
            currenMonsterCountText.text = $"{currentMonsterCount}/100";
            spawnedMonsters.Add(monsterComponent); // 스폰된 몬스터 리스트에 추가
            monsterComponent.ApplyPassiveEffect(currentSpeedReductionPercentage); // 패시브 효과 적용

            if (currentMonsterCount >= 100)
            {
                isGameOver = true;
            }
        }
    }

    public void RegisterMonster(Monster monster)
    {
        spawnedMonsters.Add(monster);
        monster.ApplyPassiveEffect(currentSpeedReductionPercentage); // 등록 시 패시브 효과 적용
    }

    public void UnregisterMonster(Monster monster)
    {
        spawnedMonsters.Remove(monster);
    }

    public void ApplyPassiveEffect(float speedReductionPercentage)
    {
        // 기존 적용된 속도 감소를 제거하고 새 속도 감소 적용
        foreach (Monster monster in spawnedMonsters)
        {
            monster.ApplyPassiveEffect(speedReductionPercentage);
        }
        currentSpeedReductionPercentage = speedReductionPercentage; // 현재 적용된 속도 감소 퍼센트를 업데이트
    }
}
