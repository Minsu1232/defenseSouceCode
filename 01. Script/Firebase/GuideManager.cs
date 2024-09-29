using DG.Tweening;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuideManager : MonoBehaviour
{
    public static GuideManager Instance { get; private set; }

    // ���̵� ���� UI
    [SerializeField] private GameObject guidePanel;
    public GameObject defenseGuidePanel;
    public TextMeshProUGUI guideTitle;
    public TextMeshProUGUI guideText;
    public Button nextButton;
    public Button defenseNextButton;
    public TextMeshProUGUI defenseguideTitle;
    public TextMeshProUGUI defenseguideText;
    // �̺�Ʈ, �ڷΰ���, ī�� �̱� ���� UI
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private Button backButton;
    [SerializeField] private Button cardButton;
    [SerializeField] private Button challengeButton;
    public Button itemButton;
    public Button moneyButton;
    public Button sumonButton;
    public Button[] taxButton;
    public Button gradeButton;
    public Button[] gradeButton2;
    bool isMoney= false;
    bool isItem=false;

    private int guideStep = 0;
    public bool isFirstTime = false; // ���� ���� ���� �÷���
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private FirebaseUser user;

    //private Coroutine nextButtonHighlightCoroutine;
    //private Coroutine defenseNextButtonHighlightCoroutine;
    private Color originalColor;

    // ȭ��ǥ ���� ����
    public RectTransform arrowTransform;  // ȭ��ǥ �̹����� RectTransform
    [SerializeField] private float arrowMoveDistance = 50f;  // ȭ��ǥ �̵� �Ÿ�
    [SerializeField] private float arrowMoveDuration = 0.5f;  // ȭ��ǥ �ִϸ��̼� ���� �ð�
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(WaitForFirebaseAndInitialize());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator WaitForFirebaseAndInitialize()
    {
        // Firebase �ʱ�ȭ �Ϸ� ���
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // �� ������ ���
        }

        // Firebase ���� ����
        databaseReference = FireBaseManager.Instance.GetDatabaseReference();

        if (databaseReference != null)
        {
            Debug.Log("Firebase Initialized in GuideManager");
            LoadFirstTimeStatus(); // ���� ���� ���� �ε�
        }
        else
        {
            Debug.LogError("Failed to get Firebase Database Reference in GuideManager.");
        }
    }

    // ���� ���� ���θ� �����ϴ� �޼���
    public void SaveFirstTimeStatus(bool isFirstTime)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("firstTime").SetValueAsync(isFirstTime).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("First time status saved successfully.");
            }
            else
            {
                Debug.LogError("Failed to save first time status.");
            }
        });
    }

    // ���� ���� ���θ� �ε��ϴ� �޼���
    public void LoadFirstTimeStatus()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // ���� ���� ���� Ȯ��
        databaseReference.Child("users").Child(userId).Child("firstTime").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to check first time access from Firebase.");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // ���� firstTime �����Ͱ� ������, ���� �������� ����
                if (!snapshot.Exists || snapshot.Value.ToString() == "false")
                {
                    Debug.Log("First time access detected. Showing guide.");
                    StartGuide(); // ���� �����̸� ���̵� ����
                    SaveFirstTimeStatus(true); // ���� ���� ���� ����
                    isFirstTime = true;
                }
                else
                {
                    Debug.Log("User has accessed before. Skipping guide.");
                    isFirstTime = false;
                    // ���̵� ��ŵ �� �Ϲ� ���� ���� ���� �߰� ����
                }
            }
        });
    }

    public void StartGuide()
    {
        guideStep = 0;
        guidePanel.SetActive(true);
        ShowNextStep();
    }

    public void ShowNextStep()
    {
        switch (guideStep)
        {
            case 0:
                ShowEventGuide();
                break;
            case 1:
                ShowBackButtonGuide();
                break;
            case 2:
                ShowCardDrawGuide();
                break;
            case 3:
                ShowWaypointGuide();
                break;
            case 4:
                DefenseGuide();
                break;
            case 5:
                DefenseBoss();
                break;
            case 6:
                BossCoinActive();
                break;
            case 7:
                FinalBoss();
                break;
            case 8:
                FinalBoss2();
                break;
            case 9:
                UseMoney();
                break;
            case 10:
                UseGrade();
                break;
            case 11:
                UseGrade2();
                break;
            case 12:
                EndGuide();
                break;
            default:
                EndGuide();
                break;
        }
        guideStep++;
    }

    // ȭ��ǥ�� Ư�� ��ư ���� ǥ���ϴ� �Լ�
    public void ShowArrowAboveButton(Button targetButton)
    {
        // ȭ��ǥ�� ��ư�� �ڽ����� ����
        arrowTransform.SetParent(targetButton.transform, false);

        // ȭ��ǥ�� ��ư�� �������� �̵�
        arrowTransform.anchoredPosition = new Vector2(0, arrowMoveDistance);  // �θ�(Button)�� �߽� �������� ���� �̵�

        // ȭ��ǥ�� Ȱ��ȭ
        arrowTransform.gameObject.SetActive(true);

        // ���Ʒ��� �ݺ��ؼ� �����̴� �ִϸ��̼�
        arrowTransform.DOAnchorPosY(arrowTransform.anchoredPosition.y + arrowMoveDistance, arrowMoveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);  // ���� �ݺ�
    }

    // ȭ��ǥ�� ����� �Լ�
    public void HideArrow()
    {
        arrowTransform.gameObject.SetActive(false);  // ȭ��ǥ ��Ȱ��ȭ
        DOTween.Kill(arrowTransform);  // DOTween �ִϸ��̼� �ߴ�
    }

    private void ShowEventGuide()
    {
        guideText.text = "ó�� ���̱���? ���ӿ� ���� �˷� �帱����. ���� ��ư�� ���� ������ �ּ���";
        guideTitle.text = "ȯ���մϴ�!";

        //nextButtonHighlightCoroutine = StartCoroutine(HighlightNextButton());
        ShowArrowAboveButton(nextButton);  // ȭ��ǥ �߰�

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            //StopCoroutine(nextButtonHighlightCoroutine);
            nextButton.image.color = originalColor;  // ���� �������� ����
            HideArrow();  // ȭ��ǥ �����
            ShowNextStep();  // ���� �ܰ�� �̵�
            nextButton.onClick.RemoveAllListeners();
        });
    }

    private void ShowBackButtonGuide()
    {
        guideText.text = "�켱 �ڷΰ��� ��ư�� ������ â�� �ݾ��ּ���.";
        guideTitle.text = "�ڷΰ���";
        backButton.gameObject.SetActive(true);

        ShowArrowAboveButton(backButton);  // ȭ��ǥ �߰�

        backButton.onClick.AddListener(() =>
        {
            HideArrow();  // ȭ��ǥ �����
            ShowNextStep();
            backButton.onClick.RemoveListener(HideArrow);
            backButton.onClick.RemoveListener(ShowNextStep);
        });
    }

    private void ShowCardDrawGuide()
    {
        guideText.text = "ī�带 �̾ƺ�����!";
        guideTitle.text = "���� ����";

        ShowArrowAboveButton(cardButton);  // ȭ��ǥ �߰�

        cardButton.onClick.AddListener(() =>
        {
            HideArrow();  // ȭ��ǥ �����
            ShowNextStep();
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(CardDrawer.Instance.OnDrawButtonClick);
        });
    }

    private void ShowWaypointGuide()
    {
        guideText.text = "�ѹ� ���ƺ�����!";
        guideTitle.text = "���� ����";

        ShowArrowAboveButton(challengeButton);  // ȭ��ǥ �߰�

        challengeButton.onClick.AddListener(() =>
        {
            HideArrow();  // ȭ��ǥ �����
            ShowNextStep();
        });
    }

    private void DefenseGuide()
    {
        // ���⿡ ���潺 ���� �ȳ��� �߰��� �� �ֽ��ϴ�.
    }

    private void DefenseBoss()
    {
        defenseguideText.text = "���ϼ̽��ϴ�! \n�� 10���帶�� ������ ���� �մϴ�. ���潺�� �� 20���� ������.. �̹��� Ư���� �ٷ� ������ ������ ������ ������!";
        defenseguideTitle.text = "���� ����";
    }

    private void BossCoinActive()
    {
        defenseguideText.text = "������ óġ�ÿ� ���������� �����մϴ�. \n������ ������ �̱� �Ǵ� ��ȭ ���� �̱⿡ ��� �˴ϴ�. \n�ƹ��ų� �ϳ� �غ����� ���� �ִ��� ������!";
        ShowArrowAboveButton(itemButton);  // ȭ��ǥ �߰�

        itemButton.onClick.AddListener(() =>
        {
            isItem = true;
            HideArrow();  // ȭ��ǥ �����
            ShowNextStep();
            itemButton.onClick.RemoveListener(ShowNextStep);
            moneyButton.onClick.RemoveListener(ShowNextStep);
        });

        moneyButton.onClick.AddListener(() =>
        {
            isMoney = true;
            HideArrow();  // ȭ��ǥ �����
            ShowNextStep();
            moneyButton.onClick.RemoveListener(ShowNextStep);
            itemButton.onClick.RemoveListener(ShowNextStep);
        });
    }

    private void FinalBoss()
    {
        if (isItem)
        {
            defenseguideText.text = "�������� �����ϼ̱���. �������� ���� 3���� �������� �����մϴ�! \n ��ȭ�� 30~500������ Ȯ���� ���� ȹ�� �մϴ�! ���߿� ���� �غ�����!";
            ShowArrowAboveButton(defenseNextButton);  // ȭ��ǥ �߰�
        }
        else if (isMoney)
        {
            defenseguideText.text = "��ȭ�� �����ϼ̱���. ��ȭ�� 30~500������ Ȯ���� ���� ȹ�� �մϴ�!  \n �������� ���� 3���� �������� �����մϴ�! ������ ���� ������ �ּ���!";
            ShowArrowAboveButton(defenseNextButton);  // ȭ��ǥ �߰�
        }

        defenseNextButton.onClick.AddListener(() =>
        {
            HideArrow();  // ȭ��ǥ �����
            ShowNextStep();
        });
    }

    void FinalBoss2()
    {
        defenseguideText.text = "���� Ʃ�丮�� ������ ������ óġ�� ������!";
    }

    void UseMoney()
    {
        guideText.text = "����ϼ̽��ϴ�! ���潺�� ���̵��� 1~3�� �����ϸ� �� ���̵� Ŭ����� \n �ǹ��� ��������, �ǹ��� ���� ������ ���� �˴ϴ�. ������ ȹ���غ�����";
        guideTitle.text = "���� �ý���";

        for (int i = 0; i < taxButton.Length; i++)
        {
            int index = i; // Ŭ���� ������ �ذ��ϱ� ���� ���� ������ ���� ĸó
            if (taxButton[index] == null) // null üũ
            {
                Debug.LogError($"taxButton[{index}] is null");
                continue;
            }

            ShowArrowAboveButton(taxButton[index]);  // ȭ��ǥ �߰�

            taxButton[index].onClick.AddListener(() =>
            {
                HideArrow();  // ȭ��ǥ �����
                ShowNextStep();
                taxButton[index].onClick.RemoveListener(ShowNextStep); // �̺�Ʈ ������ ����
            });
        }
    }

    private void UseGrade()
    {
        guideText.text = "���ƿ�! ���ݰ� Ŭ���� ��ȭ�� ������� ����غ���. ��ȭ �ý����Դϴ� ��ȭ ��ư�� ����������.";
        guideTitle.text = "��ȭ �ý���";

        ShowArrowAboveButton(gradeButton);  // ȭ��ǥ �߰�

        gradeButton.onClick.AddListener(() =>
        {
            HideArrow();  // ȭ��ǥ �����
            ShowNextStep();
            gradeButton.onClick.RemoveListener(ShowNextStep);
        });
    }

    private void UseGrade2()
    {
        guideText.text = "���⿣ ���� �����ϴ� ��� �������� �����մϴ�. ������ ��� ���� �Ѹ��� ��ȭ�غ�����";
        guideTitle.text = "��ȭ �ý���";

        for (int i = 0; i < gradeButton2.Length; i++)
        {
            int index = i; // Ŭ���� ������ �ذ��ϱ� ���� ���� ������ ���� ĸó
            if (gradeButton2[index] == null) // null üũ
            {
                Debug.LogError($"taxButton[{index}] is null");
                continue;
            }

            ShowArrowAboveButton(gradeButton2[index]);  // ȭ��ǥ �߰�

            gradeButton2[index].onClick.AddListener(() =>
            {
                HideArrow();  // ȭ��ǥ �����
                ShowNextStep();
                gradeButton2[index].onClick.RemoveListener(ShowNextStep); // �̺�Ʈ ������ ����
            });
        }
    }

    private void EndGuide()
    {
        guideText.text = "����ϼ̽��ϴ�. ���潺 ���� �����ٸ� ������ ��ȭ�� �� �ٽ� �����غ�����! \n ���� ����� ���� �׽�Ʈ�Ϸ� ��� �غ�����";
        guideTitle.text = "���̵� ����";

        ShowArrowAboveButton(nextButton);  // ȭ��ǥ �߰�

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            //StopCoroutine(nextButtonHighlightCoroutine);
            nextButton.image.color = originalColor;  // ���� �������� ����
            HideArrow();  // ȭ��ǥ �����
            Destroy(guidePanel);
            isFirstTime = false;
        });
    }

    // ��ư�� ��¦�̰� ����� �Լ�
    //private IEnumerator HighlightNextButton()
    //{
    //    Color originalColor = nextButton.image.color; // ���� ��ư ��
    //    Color highlightColor = Color.yellow; // ������ ����

    //    while (true)
    //    {
    //        nextButton.image.color = highlightColor;
    //        yield return new WaitForSeconds(0.5f);
    //        nextButton.image.color = originalColor;
    //        yield return new WaitForSeconds(0.5f);
    //    }
    //}
}
