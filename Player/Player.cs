using DG.Tweening;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; } // 싱글턴 인스턴스
    public Transform[] waypoints; // 플레이어가 이동할 웨이포인트들
    public int currentWaypointIndex = 0;
    public float moveDuration = 0.5f; // 이동 시간
    public float jumpHeight = 1.0f; // 포물선 이동 시 높이

    public TextMeshProUGUI difficultyText;  // 도전 패널 난이도를 표시할 텍스트
    public TextMeshProUGUI rewardText;      // 도전 패널 보상을 표시할 텍스트
    public TextMeshProUGUI currentWaypoint; // 도전 패널 웨이포인트를 표시할 텍스트
    public TextMeshProUGUI nickNameText; // 닉네임 텍스트

    private Animator animator; // Animator 참조
    private bool isMoving = false; // 플레이어가 이동 중인지 추적
    public bool isChallenge = false; //한번 도착했을때 카드를 한번더 못뽑기 위한 불타입

    private int ground; // 몇바퀴돌았는지 궁금해서 넣은 변수

    [SerializeField] private GameObject challengePanel; // 도전 패널 (유니티 에디터에서 할당)
    [SerializeField] private Button challengeButton; // 도전 버튼 (유니티 에디터에서 할당)

    #region 변수들

    public GameObject canvas;
    public int experience;
    public int level;

    private FirebaseAuth auth;
    private FirebaseUser user;
    private DatabaseReference databaseReference;
    [SerializeField] private Image experienceBar; // 경험치 바를 표시할 UI Image
    [SerializeField] private TextMeshProUGUI levelText; // 레벨을 표시할 UI Text

    #endregion

    #region 프로퍼티들

    public int Experience
    {
        get => experience;
         set
        {
            experience = value;            
            SaveExperienceToFirebase(); // 경험치 변경 시 Firebase에 저장
            UpdateExperienceUI(); // 경험치가 변경될 때 UI 업데이트
        }
    }

    public int Level
    {
        get => level;
         set
        {
            level = value;           
            SaveLevelToFirebase(); // 레벨 변경 시 Firebase에 저장
            UpdateLevelUI(); // 레벨 변경 시 UI 업데이트
        }
    }

    public bool IsMoving
    {
        get => isMoving;
        private set => isMoving = value;
    }

    #endregion

    #region Unity 라이프싸이클 메서드

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
   
        animator = GetComponent<Animator>();
        // Firebase Manager에서 Database Reference 가져오기

        animator = GetComponent<Animator>();
        StartCoroutine(WaitForFirebaseAndLoadData());
    }

    private IEnumerator WaitForFirebaseAndLoadData()
    {
        // Firebase 초기화가 완료될 때까지 대기
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // 한 프레임 대기
        }

        // Firebase 초기화 완료 후 데이터베이스 참조 얻기
        databaseReference = FireBaseManager.Instance.GetDatabaseReference();

        // 데이터 로드 등 Firebase 관련 작업 시작
        if (databaseReference != null)
        {

            // 플레이어 데이터를 로드 (위치 포함)
            LoadPlayerPositionFromFirebase();
            LoadPlayerData();

            
        }
        else
        {
            Debug.LogError("Failed to get Firebase Database Reference.");
        }
    }
   
    #endregion

    #region 게임플레이 메서드

    public void AddExperience(int amount)
    {
        Experience += amount;

        // 레벨업이 반복적으로 발생할 수 있도록 
        while (Experience >= 1000 * Level) // 예시: 레벨업 필요 경험치 = 1000 * 현재 레벨
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Experience -= 1000 * Level; // 현재 레벨에 필요한 경험치만큼 차감
        Level += 1;
        Debug.Log("레벨 업! 현재 레벨: " + Level);
    }

    public void Move(int steps)
    {
        if (!IsMoving) // 이동 중이 아닌 경우에만 이동 시작
        {
            StartCoroutine(MovePlayer(steps));
        }
    }

    private IEnumerator MovePlayer(int steps)
    {
        
        IsMoving = true; // 이동 시작
        for (int i = 0; i < steps; i++)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
           
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            
            // Jump 애니메이션 시작
            animator.SetTrigger("Jump");

            // DOTween을 사용해 포물선 경로로 이동
            transform.DOJump(targetPosition, jumpHeight, 1, moveDuration).SetEase(Ease.OutQuad);

            // 이동 완료 후 대기
            yield return new WaitForSeconds(moveDuration);

            // 이동이 끝나면 Idle 애니메이션으로 전환
            animator.SetTrigger("Idle");
        }
        IsMoving = false; // 이동 완료
        SavePlayerPositionToFirebase();
        // 도착한 웨이포인트에서 이벤트 처리
        OnReachWaypoint(currentWaypointIndex);
    }

    #endregion

    #region 칸 도착 시 연동 메서드

    public void OnReachWaypoint(int waypointIndex)
    {   if(currentWaypointIndex == 0)
        {
            PlayerMoneyManager.Instance.AddMoney(3000); // 한바퀴 돌면 3000원 추가
            isChallenge = false;
        }
        else
        {
            // 도전 패널 표시
            ShowChallengePanel();
            UpdateChallengePanel(currentWaypointIndex);
            isChallenge = true;
        }
        
    }

    private void ShowChallengePanel()
    {
        if (challengePanel != null)
        {
            challengePanel.SetActive(true);
        }
        
    }

    private void HideChallengePanel()
    {
        if (challengePanel != null)
        {
            challengePanel.SetActive(false);
        }
    }
    public void UpdateChallengePanel(int waypointIndex)
    {
        if (GuideManager.Instance.isFirstTime)
        {
            // 난이도와 보상을 UI에 표시
            difficultyText.text = $"난이도: 튜토리얼";
            rewardText.text = $"보상 \n 경험치 0\n 금화 0";
            currentWaypoint.text = $" 1-{currentWaypointIndex}".ToString();
        }
        else
        {
            // 난이도를 가져와서 보상을 계산한 후 UI에 표시
            SaveLoadManager.Instance.GetDifficultyForWaypoint(waypointIndex, (difficulty) =>
            {

                int experienceReward = difficulty * 500;
                int goldReward = difficulty * 1000;

                // 난이도와 보상을 UI에 표시
                difficultyText.text = $"난이도: {difficulty}";
                rewardText.text = $"보상 \n 경험치 {experienceReward}\n 금화 {goldReward}";
                currentWaypoint.text = $" 1-{currentWaypointIndex}".ToString();
            });
        }
       
    }
    public void OnChallengeButtonClick()
    {
        // 비동기 씬 로드 시작 (SceneLoader로 페이드 인/아웃 추가)
        SceneLoader.Instance.LoadScene("MainLobby");
        // 씬 로드 완료 후 UI와 게임 오브젝트 비활성화
        challengePanel.SetActive(false);
        canvas.SetActive(false);
        gameObject.SetActive(false);
        CameraFollow.instance.gameObject.SetActive(false);
        DD6.instance.gameObject.SetActive(false);
        WaypointManager.Instance.gameObject.SetActive(false);
    }

    // 비동기로 MainLobby 씬을 로드하는 코루틴
    private IEnumerator LoadMainLobbyAsync()
    {
        // 씬 로드가 완료될 때까지 비활성화를 연기
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainLobby");

        // 씬 로드가 완료될 때까지 대기
        while (!operation.isDone)
        {
            yield return null; // 한 프레임 대기
        }

        // 씬 로드 완료 후 UI와 게임 오브젝트 비활성화
        challengePanel.SetActive(false);
        canvas.SetActive(false);
        gameObject.SetActive(false);
        CameraFollow.instance.gameObject.SetActive(false);
        DD6.instance.gameObject.SetActive(false);
        WaypointManager.Instance.gameObject.SetActive(false);

    }
    #endregion

    #region UI 업데이트 메서드
    private void LoadPlayerData()
    {
        
        if (SaveLoadManager.Instance != null)
        {
            bool isLevelLoaded = false;
            bool isExperienceLoaded = false;

            // 레벨 로드
            SaveLoadManager.Instance.LoadLevel(() =>
            {
                isLevelLoaded = true; // 로드 완료 플래그 설정
                if (isLevelLoaded && isExperienceLoaded)
                {
                    UpdateUI(); // 모든 데이터 로드 후 UI 업데이트
                }
            });

            // 경험치 로드
            SaveLoadManager.Instance.LoadExperience(() =>
            {
                isExperienceLoaded = true; // 로드 완료 플래그 설정
                if (isLevelLoaded && isExperienceLoaded)
                {
                    UpdateUI(); // 모든 데이터 로드 후 UI 업데이트
                }
            });
        }
        else
        {
            Debug.LogError("SaveLoadManager.Instance is null. Cannot load player data.");
        }
        LoadNickNameFromDatabase();
    }
    private void LoadNickNameFromDatabase()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("nickName").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    string nickName = task.Result.Value.ToString();
                   nickNameText.text = nickName;
                }
                else
                {
                    nickNameText.text = "재접속권장";
                }
            }
            else
            {
                Debug.LogError("닉네임 로드 실패: " + task.Exception?.Message);
            }
        });
    }
    private void UpdateUI()
    {
        UpdateLevelUI();
        UpdateExperienceUI();
    }
    private void UpdateExperienceUI()
    {
        if (experienceBar != null)
        {
            float fillAmount = (float)Experience / (1000 * Level); // 경험치 바의 fillAmount 계산
            experienceBar.fillAmount = fillAmount;
        }
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = Level.ToString();
        }
    }

    #endregion

    #region 데이터 저장 메서드

    private void SaveExperienceToFirebase()
    {
        SaveLoadManager.Instance.SaveExperience(Experience);
    }

    private void SaveLevelToFirebase()
    {
        SaveLoadManager.Instance.SaveLevel(Level);
    }
    private void SavePlayerPositionToFirebase()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        if (databaseReference != null)
        {
            // 현재 웨이포인트 인덱스를 Firebase에 저장
            databaseReference.Child("users").Child(userId).Child("currentWaypointIndex").SetValueAsync(currentWaypointIndex);
            Debug.Log("캐릭터 위치 저장 " + currentWaypointIndex);
        }
    }
    private void LoadPlayerPositionFromFirebase()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        if (databaseReference != null)
        {
            databaseReference.Child("users").Child(userId).Child("currentWaypointIndex").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        currentWaypointIndex = int.Parse(snapshot.Value.ToString());
                        // 저장된 웨이포인트 위치로 캐릭터를 이동
                        gameObject.transform.position = waypoints[currentWaypointIndex].position;
                    }
                    else
                    {
                        Debug.Log("Player position not found, starting from default position.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to load player position.");
                }
            });
        }
    }

    #endregion
}
