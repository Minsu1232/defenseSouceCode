using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterSpawnManager : MonoBehaviour
{
    public static MonsterSpawnManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    
    public List<GameObject> spawnList = new List<GameObject>(); // ������ ���� ����Ʈ
    public List<GameObject> bossSpawnList = new List<GameObject>(); // ������ ���� ����Ʈ
 
    public Transform spawnPoint; // ���Ͱ� ������ ��ġ
    public float spawnInterval = 0.5f; // ���� ���� ����
    public int normalRoundSpawnCount = 20; // �Ϲ� ���忡�� ������ ���� ��
    public float normalRoundDuration = 20.0f; // �Ϲ� ���� �ð� (��)
    public float bossRoundDuration = 45.0f; // ���� ���� �ð� (��)
    public List<Transform> waypoints = new List<Transform>(); // ���͵��� �̵��� ��� ����Ʈ

    public int currentMonsterCount = 0; // ��ȯ�Ǿ� �ִ� ���� ��
   
    public int currentRound = 1; // ���� ����
    public TextMeshProUGUI roundTimerText; 
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI waypointTaxt;
    public TextMeshProUGUI tapStart;
    public TextMeshProUGUI currenMonsterCountText;
    private bool isSpawning = false; // ���� ������ ����

    public List<Monster> spawnedMonsters = new List<Monster>(); // ������ ���� ����Ʈ

    public GameObject panel; // ���� �� ���н� �����ϴ� �г�
    public TextMeshProUGUI resultMessage;
    public TextMeshProUGUI buttonMessage;

    private float currentSpeedReductionPercentage = 0f; // ���� ����� �ӵ� ���� �ۼ�Ʈ

    public bool isGameOver = false; // ���� ���� ���¸� ��Ÿ���� ����
    public bool isStart = false;
    public bool isCleared = false;
    private int difficultyLevel = 1; // ���̵� ����
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
       
        // Ư�� ��������Ʈ�� Ŭ���� Ƚ���� �ҷ���
        SaveLoadManager.Instance.GetClearedTimesForWaypoint(Player.Instance.currentWaypointIndex, clearedTimes =>
        {
            // Ŭ���� Ƚ���� ���� ���̵��� ����
            int newDifficultyLevel;
            if (GuideManager.Instance.isFirstTime) // ù ������ ���
            {
                newDifficultyLevel = 0; // ù ���� �� ���̵��� 0���� ����
            }
            else if (clearedTimes >= 2)
            {
                newDifficultyLevel = 3; // Ŭ���� Ƚ���� 2ȸ �̻��� ���
            }
            else if (clearedTimes >= 1)
            {
                newDifficultyLevel = 2; // Ŭ���� Ƚ���� 1ȸ �̻��� ���
            }
            else
            {
                newDifficultyLevel = 1; // Ŭ���� Ƚ���� 0ȸ�� ���
            }

            difficultyLevel = newDifficultyLevel;

            // ���̵��� ���� ���� ����
            SaveLoadManager.Instance.SetDifficultyForWaypoint(Player.Instance.currentWaypointIndex, difficultyLevel);
            
            // ���� ��ƾ ����
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
            return $"{damage / 1000000f:0.#}M"; // �鸸 ������ ��ȯ
        }
        else if (damage >= 1000)
        {
            return $"{damage / 1000f:0.#}K"; // õ ������ ��ȯ
        }
        else
        {
            return damage.ToString(); // õ �̸��� �״�� ���
        }
    }
    void OpenPanel(bool isSuccess)
    {
        panel.SetActive(true); // �г� Ȱ��ȭ
        CharacterInfo mvp = HeroManager.Instance.FindMVP();
        // ������ ����ؼ� ǥ�� (K, M)
        string formattedDamage = FormatDamage(mvp.totalDamageDealt);
        // ���� �Ǵ� ���п� ���� �޽����� ����
        if (isSuccess)
        {
           
            resultMessage.text = $"1-{Player.Instance.currentWaypointIndex} ����!!\n\n\n\n\n����\n\n��ȭ : {difficultyLevel*1000}\n����ġ : {difficultyLevel*500}\n\nMVP\n\n{mvp.characterData.heroName} ���� : {formattedDamage}"; // ���� �޽���
            buttonMessage.text = "ȹ��";
            resultMessage.color = Color.white; // 
        }
        else
        {
            resultMessage.text = $"1-{Player.Instance.currentWaypointIndex} ����...\n\n\n\n\n\n\n ������ �ٽ�.. \n\nMVP\n\n{mvp.characterData.heroName} ���� : {formattedDamage}" ;
            buttonMessage.text = "��";
            resultMessage.color = Color.white; //
        }
    }
    public void LoadScene() // �ڵ� ���� �Ҳ��� >> ���Ŵ��� �̺�Ʈ
    {
        panel.gameObject.SetActive(false);
        HeroManager.Instance.summonedHeroInstances.Clear();
        HeroManager.Instance.summonCost = 10;
        HeroManager.Instance.InitializeSpawnPoints(); // ���� ��ġ �ʱ�ȭ
        HeroManager.Instance.UpdateUI();
        Player.Instance.gameObject.SetActive(true);
        Player.Instance.canvas.SetActive(true);
        Player.Instance.isChallenge = false;
        WaypointManager.Instance.gameObject.SetActive(true);
        CameraFollow.instance.gameObject.SetActive(true);
        DD6.instance.gameObject.SetActive(true);
        Debug.Log("����� �Ŵ��� �ʱ�ȭ");
        
        // �� �ε� �̺�Ʈ ����
        if (isCleared) // Ŭ�����ߴٸ� 
        {
            SceneManager.sceneLoaded += OnSceneLoaded; // �̺�Ʈ ����
        }
        

        // **SceneLoader�� ����Ͽ� ���̵� ��/�ƿ��� �Բ� �� �ε�**
        SceneLoader.Instance.LoadScene("PlayerScene");
    }

  


    // ���� �ε�� �� ȣ��Ǵ� �Լ� Ŭ����� ��� ������ �����ϱ� ����
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� �ε�� �� BuildingTaxManager�� �����Ͽ� ������ ����
        BuildingTaxManager[] buildingManagers = FindObjectsOfType<BuildingTaxManager>();

        foreach (var manager in buildingManagers)
        {
            if (manager.waypointIndex == Player.Instance.currentWaypointIndex)
            {
                // �ʿ��� ������ ����
                manager.moneyReadyForWaypoint = true;
                //manager.CollectMoneyForWaypoint(); // �� ������ �ּ� �����ϸ� �ڵ� ���� ����
                Debug.Log("��ü��Ű���");
            }
        }

        // �� ���� �����ϰ� �ϱ� ���� �̺�Ʈ ���� ���� (�ʿ�� �ּ� ó�� ����)
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Update()
    {
        if (currentMonsterCount >= 100 && !isGameOver) // ���� ���� ���� 100�� �����ϸ� ���� ���� ó��
        {
            isGameOver = true;
            OpenPanel(false);
            Debug.Log("Game Over! Too many monsters!");
        }
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && !isStart)
        {
            isStart = true;  // �ڷ�ƾ�� �� ���� ����ǵ���
            tapStart.gameObject.SetActive(false); // ���� �ؽ�Ʈ ����
            StartCoroutine(SpawnRoutine());  // �ڷ�ƾ ����
        }
    }

    IEnumerator SpawnRoutine()
    {
        // currentRound�� minRound�� �ʱ�ȭ
        GetRoundRangeForCurrentLevel(out int minRound, out int maxRound);
        currentRound = minRound;

        if (difficultyLevel == 0) // ù ���� �� ����ȭ�� ����
        {
            List<int> simplifiedRounds = new List<int> { 1, 10, 20 };

            foreach (int round in simplifiedRounds)
            {
                currentRound = round;

                if (currentRound == 20) // ������ ���� ó��
                {
                    RoundText(currentRound);
                    yield return StartCoroutine(HandleFinalRound());
                }
                else if (currentRound % 10 == 0) // ���� ����
                {
                    RoundText(currentRound);
                    yield return StartCoroutine(SpawnBossRound());
                }
                else // �Ϲ� ����
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
                if (currentRound == maxRound) // ������ ���� ó��
                {
                    RoundText(currentRound);
                    yield return StartCoroutine(HandleFinalRound());
                }
                else if (currentRound % 10 == 0) // ���� ����
                {
                    RoundText(currentRound);
                    yield return StartCoroutine(SpawnBossRound());
                }
                else // �Ϲ� ����
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

        if (difficultyLevel == 1 || difficultyLevel == 0) // ���̵��� 0 �Ǵ� 1�� ���� �״�� ǥ��
        {
            displayRound = currentRound;
        }
        else if (difficultyLevel == 2) // ���̵��� 2�� ���� 20�� ���� 1~20���� ǥ��
        {
            displayRound = currentRound - 20;
        }
        else // ���̵��� 3�� ���� 40�� ���� 1~20���� ǥ��
        {
            displayRound = currentRound - 40;
        }

        // ���带 1~20���� ǥ��
        roundText.text = $"{displayRound}/20";
    }
    void GetRoundRangeForCurrentLevel(out int minRound, out int maxRound)
    {
        if (difficultyLevel == 0) // ù ������ ���
        {
            minRound = 1;
            maxRound = 20; // ù ���� �� ���� 1���� 20����
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
        if (isGameOver || currentMonsterCount >= 100) // ���� ���� �Ǵ� ���� ���� 100 �̻��̸� ���� ����
        {
            isGameOver = true;
            yield break;
        }
        int bossIndex = (currentRound / 10) - 1; // ���� ���忡 �´� ���� �ε���
        SpawnBoss(bossIndex);

        // 1�� ���
        float timer = 60f;
        while (timer > 0f && currentMonsterCount > 0 && !isGameOver)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (currentMonsterCount == 0 && !isGameOver) // ��� ���Ͱ� óġ�Ǿ�����
        {
            HandleLevelCleared();
            OpenPanel(true);
        }
        else // �ð��� �� �Ǿ��ų� ���Ͱ� ����������
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

        // ����ġ�� �� ����
        Player.Instance.AddExperience(addExperience); // ���÷� 1000 ����ġ ����
        PlayerMoneyManager.Instance.AddMoney(addMoney); // ���÷� 500 �� ����
        AchievementsManager.Instance.CheckAchievement("Ŭ���� Ƚ��", AchievementsManager.Instance.totalClearTime);
        // Ŭ���� ���� ����
        SaveLoadManager.Instance.MarkWaypointAsCleared(Player.Instance.currentWaypointIndex);
        // Ŭ���� Ƚ�� ����
        SaveLoadManager.Instance.IncrementClearedTimesForWaypoint(Player.Instance.currentWaypointIndex);
    }
    void HandleGameOver()
    {
        Debug.Log("Game Over! Handling Game Over...");
        isGameOver = true;
        // ���⿡ ���� ���� �� ó���� ���� �߰� (�� ��ȯ, UI ǥ�� ��)
        OpenPanel(false);
    }

    IEnumerator SpawnNormalRound()
    {
        int monsterIndex = (currentRound - 1) / 10;
        float roundEndTime = Time.time + normalRoundDuration;

        // ���� Ÿ�̸� ������Ʈ ����
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
        // ���� ���� �ð� ������Ʈ
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
            // ���� �ð��� ������Ʈ�Ͽ� ǥ��
            timeRemaining -= Time.deltaTime;
            roundTimerText.text = $"���� �ð�: {Mathf.Ceil(timeRemaining)} ��";

            yield return null;
        }

        // ���� ���� �� Ÿ�̸� ����
        roundTimerText.text = "����!";
    }
    void SpawnMonster(int index)
    {
        if (index >= 0 && index < spawnList.Count)
        {
            GameObject monster = Instantiate(spawnList[index], spawnPoint.position, spawnPoint.rotation);
            Monster monsterComponent = monster.GetComponent<Monster>();
            monsterComponent.waypoints.AddRange(waypoints); // �ν��Ͻÿ���Ʈ�� �����տ��� ��������Ʈ �Ҵ�
            currentMonsterCount++; // ���� ���� �� ����
            currenMonsterCountText.text = $"{currentMonsterCount}/100";
            spawnedMonsters.Add(monsterComponent); // ������ ���� ����Ʈ�� �߰�
            monsterComponent.ApplyPassiveEffect(currentSpeedReductionPercentage); // �нú� ȿ�� ����

            // Ȱ��ȭ�� ��� �������� ȿ���� ���� ������ ���Ϳ� ����
            ItemManager.Instance.ApplyActiveItems(monsterComponent);
        }
    }

    void SpawnBoss(int index)
    {
        if (index >= 0 && index < bossSpawnList.Count)
        {
            GameObject monster = Instantiate(bossSpawnList[index], spawnPoint.position, spawnPoint.rotation);
            Monster monsterComponent = monster.GetComponent<Monster>();
            monsterComponent.waypoints.AddRange(waypoints); // �ν��Ͻÿ���Ʈ�� �����տ��� ��������Ʈ �Ҵ�
            currentMonsterCount++; // ���� ���� �� ����
            currenMonsterCountText.text = $"{currentMonsterCount}/100";
            spawnedMonsters.Add(monsterComponent); // ������ ���� ����Ʈ�� �߰�
            monsterComponent.ApplyPassiveEffect(currentSpeedReductionPercentage); // �нú� ȿ�� ����

            if (currentMonsterCount >= 100)
            {
                isGameOver = true;
            }
        }
    }

    public void RegisterMonster(Monster monster)
    {
        spawnedMonsters.Add(monster);
        monster.ApplyPassiveEffect(currentSpeedReductionPercentage); // ��� �� �нú� ȿ�� ����
    }

    public void UnregisterMonster(Monster monster)
    {
        spawnedMonsters.Remove(monster);
    }

    public void ApplyPassiveEffect(float speedReductionPercentage)
    {
        // ���� ����� �ӵ� ���Ҹ� �����ϰ� �� �ӵ� ���� ����
        foreach (Monster monster in spawnedMonsters)
        {
            monster.ApplyPassiveEffect(speedReductionPercentage);
        }
        currentSpeedReductionPercentage = speedReductionPercentage; // ���� ����� �ӵ� ���� �ۼ�Ʈ�� ������Ʈ
    }
}
