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
    public static Player Instance { get; private set; } // �̱��� �ν��Ͻ�
    public Transform[] waypoints; // �÷��̾ �̵��� ��������Ʈ��
    public int currentWaypointIndex = 0;
    public float moveDuration = 0.5f; // �̵� �ð�
    public float jumpHeight = 1.0f; // ������ �̵� �� ����

    public TextMeshProUGUI difficultyText;  // ���� �г� ���̵��� ǥ���� �ؽ�Ʈ
    public TextMeshProUGUI rewardText;      // ���� �г� ������ ǥ���� �ؽ�Ʈ
    public TextMeshProUGUI currentWaypoint; // ���� �г� ��������Ʈ�� ǥ���� �ؽ�Ʈ
    public TextMeshProUGUI nickNameText; // �г��� �ؽ�Ʈ

    private Animator animator; // Animator ����
    private bool isMoving = false; // �÷��̾ �̵� ������ ����
    public bool isChallenge = false; //�ѹ� ���������� ī�带 �ѹ��� ���̱� ���� ��Ÿ��

    private int ground; // ��������Ҵ��� �ñ��ؼ� ���� ����

    [SerializeField] private GameObject challengePanel; // ���� �г� (����Ƽ �����Ϳ��� �Ҵ�)
    [SerializeField] private Button challengeButton; // ���� ��ư (����Ƽ �����Ϳ��� �Ҵ�)

    #region ������

    public GameObject canvas;
    public int experience;
    public int level;

    private FirebaseAuth auth;
    private FirebaseUser user;
    private DatabaseReference databaseReference;
    [SerializeField] private Image experienceBar; // ����ġ �ٸ� ǥ���� UI Image
    [SerializeField] private TextMeshProUGUI levelText; // ������ ǥ���� UI Text

    #endregion

    #region ������Ƽ��

    public int Experience
    {
        get => experience;
         set
        {
            experience = value;            
            SaveExperienceToFirebase(); // ����ġ ���� �� Firebase�� ����
            UpdateExperienceUI(); // ����ġ�� ����� �� UI ������Ʈ
        }
    }

    public int Level
    {
        get => level;
         set
        {
            level = value;           
            SaveLevelToFirebase(); // ���� ���� �� Firebase�� ����
            UpdateLevelUI(); // ���� ���� �� UI ������Ʈ
        }
    }

    public bool IsMoving
    {
        get => isMoving;
        private set => isMoving = value;
    }

    #endregion

    #region Unity ����������Ŭ �޼���

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
        // Firebase Manager���� Database Reference ��������

        animator = GetComponent<Animator>();
        StartCoroutine(WaitForFirebaseAndLoadData());
    }

    private IEnumerator WaitForFirebaseAndLoadData()
    {
        // Firebase �ʱ�ȭ�� �Ϸ�� ������ ���
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // �� ������ ���
        }

        // Firebase �ʱ�ȭ �Ϸ� �� �����ͺ��̽� ���� ���
        databaseReference = FireBaseManager.Instance.GetDatabaseReference();

        // ������ �ε� �� Firebase ���� �۾� ����
        if (databaseReference != null)
        {

            // �÷��̾� �����͸� �ε� (��ġ ����)
            LoadPlayerPositionFromFirebase();
            LoadPlayerData();

            
        }
        else
        {
            Debug.LogError("Failed to get Firebase Database Reference.");
        }
    }
   
    #endregion

    #region �����÷��� �޼���

    public void AddExperience(int amount)
    {
        Experience += amount;

        // �������� �ݺ������� �߻��� �� �ֵ��� 
        while (Experience >= 1000 * Level) // ����: ������ �ʿ� ����ġ = 1000 * ���� ����
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        Experience -= 1000 * Level; // ���� ������ �ʿ��� ����ġ��ŭ ����
        Level += 1;
        Debug.Log("���� ��! ���� ����: " + Level);
    }

    public void Move(int steps)
    {
        if (!IsMoving) // �̵� ���� �ƴ� ��쿡�� �̵� ����
        {
            StartCoroutine(MovePlayer(steps));
        }
    }

    private IEnumerator MovePlayer(int steps)
    {
        
        IsMoving = true; // �̵� ����
        for (int i = 0; i < steps; i++)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
           
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            
            // Jump �ִϸ��̼� ����
            animator.SetTrigger("Jump");

            // DOTween�� ����� ������ ��η� �̵�
            transform.DOJump(targetPosition, jumpHeight, 1, moveDuration).SetEase(Ease.OutQuad);

            // �̵� �Ϸ� �� ���
            yield return new WaitForSeconds(moveDuration);

            // �̵��� ������ Idle �ִϸ��̼����� ��ȯ
            animator.SetTrigger("Idle");
        }
        IsMoving = false; // �̵� �Ϸ�
        SavePlayerPositionToFirebase();
        // ������ ��������Ʈ���� �̺�Ʈ ó��
        OnReachWaypoint(currentWaypointIndex);
    }

    #endregion

    #region ĭ ���� �� ���� �޼���

    public void OnReachWaypoint(int waypointIndex)
    {   if(currentWaypointIndex == 0)
        {
            PlayerMoneyManager.Instance.AddMoney(3000); // �ѹ��� ���� 3000�� �߰�
            isChallenge = false;
        }
        else
        {
            // ���� �г� ǥ��
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
            // ���̵��� ������ UI�� ǥ��
            difficultyText.text = $"���̵�: Ʃ�丮��";
            rewardText.text = $"���� \n ����ġ 0\n ��ȭ 0";
            currentWaypoint.text = $" 1-{currentWaypointIndex}".ToString();
        }
        else
        {
            // ���̵��� �����ͼ� ������ ����� �� UI�� ǥ��
            SaveLoadManager.Instance.GetDifficultyForWaypoint(waypointIndex, (difficulty) =>
            {

                int experienceReward = difficulty * 500;
                int goldReward = difficulty * 1000;

                // ���̵��� ������ UI�� ǥ��
                difficultyText.text = $"���̵�: {difficulty}";
                rewardText.text = $"���� \n ����ġ {experienceReward}\n ��ȭ {goldReward}";
                currentWaypoint.text = $" 1-{currentWaypointIndex}".ToString();
            });
        }
       
    }
    public void OnChallengeButtonClick()
    {
        // �񵿱� �� �ε� ���� (SceneLoader�� ���̵� ��/�ƿ� �߰�)
        SceneLoader.Instance.LoadScene("MainLobby");
        // �� �ε� �Ϸ� �� UI�� ���� ������Ʈ ��Ȱ��ȭ
        challengePanel.SetActive(false);
        canvas.SetActive(false);
        gameObject.SetActive(false);
        CameraFollow.instance.gameObject.SetActive(false);
        DD6.instance.gameObject.SetActive(false);
        WaypointManager.Instance.gameObject.SetActive(false);
    }

    // �񵿱�� MainLobby ���� �ε��ϴ� �ڷ�ƾ
    private IEnumerator LoadMainLobbyAsync()
    {
        // �� �ε尡 �Ϸ�� ������ ��Ȱ��ȭ�� ����
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainLobby");

        // �� �ε尡 �Ϸ�� ������ ���
        while (!operation.isDone)
        {
            yield return null; // �� ������ ���
        }

        // �� �ε� �Ϸ� �� UI�� ���� ������Ʈ ��Ȱ��ȭ
        challengePanel.SetActive(false);
        canvas.SetActive(false);
        gameObject.SetActive(false);
        CameraFollow.instance.gameObject.SetActive(false);
        DD6.instance.gameObject.SetActive(false);
        WaypointManager.Instance.gameObject.SetActive(false);

    }
    #endregion

    #region UI ������Ʈ �޼���
    private void LoadPlayerData()
    {
        
        if (SaveLoadManager.Instance != null)
        {
            bool isLevelLoaded = false;
            bool isExperienceLoaded = false;

            // ���� �ε�
            SaveLoadManager.Instance.LoadLevel(() =>
            {
                isLevelLoaded = true; // �ε� �Ϸ� �÷��� ����
                if (isLevelLoaded && isExperienceLoaded)
                {
                    UpdateUI(); // ��� ������ �ε� �� UI ������Ʈ
                }
            });

            // ����ġ �ε�
            SaveLoadManager.Instance.LoadExperience(() =>
            {
                isExperienceLoaded = true; // �ε� �Ϸ� �÷��� ����
                if (isLevelLoaded && isExperienceLoaded)
                {
                    UpdateUI(); // ��� ������ �ε� �� UI ������Ʈ
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
                    nickNameText.text = "�����ӱ���";
                }
            }
            else
            {
                Debug.LogError("�г��� �ε� ����: " + task.Exception?.Message);
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
            float fillAmount = (float)Experience / (1000 * Level); // ����ġ ���� fillAmount ���
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

    #region ������ ���� �޼���

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
            // ���� ��������Ʈ �ε����� Firebase�� ����
            databaseReference.Child("users").Child(userId).Child("currentWaypointIndex").SetValueAsync(currentWaypointIndex);
            Debug.Log("ĳ���� ��ġ ���� " + currentWaypointIndex);
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
                        // ����� ��������Ʈ ��ġ�� ĳ���͸� �̵�
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
