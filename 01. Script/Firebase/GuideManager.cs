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

    // 가이드 관련 UI
    [SerializeField] private GameObject guidePanel;
    public GameObject defenseGuidePanel;
    public TextMeshProUGUI guideTitle;
    public TextMeshProUGUI guideText;
    public Button nextButton;
    public Button defenseNextButton;
    public TextMeshProUGUI defenseguideTitle;
    public TextMeshProUGUI defenseguideText;
    // 이벤트, 뒤로가기, 카드 뽑기 관련 UI
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
    public bool isFirstTime = false; // 최초 접속 여부 플래그
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private FirebaseUser user;

    //private Coroutine nextButtonHighlightCoroutine;
    //private Coroutine defenseNextButtonHighlightCoroutine;
    private Color originalColor;

    // 화살표 관련 설정
    public RectTransform arrowTransform;  // 화살표 이미지의 RectTransform
    [SerializeField] private float arrowMoveDistance = 50f;  // 화살표 이동 거리
    [SerializeField] private float arrowMoveDuration = 0.5f;  // 화살표 애니메이션 지속 시간
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
        // Firebase 초기화 완료 대기
        while (!FireBaseManager.Instance.IsInitialized)
        {
            yield return null; // 한 프레임 대기
        }

        // Firebase 참조 설정
        databaseReference = FireBaseManager.Instance.GetDatabaseReference();

        if (databaseReference != null)
        {
            Debug.Log("Firebase Initialized in GuideManager");
            LoadFirstTimeStatus(); // 최초 접속 여부 로드
        }
        else
        {
            Debug.LogError("Failed to get Firebase Database Reference in GuideManager.");
        }
    }

    // 최초 접속 여부를 저장하는 메서드
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

    // 최초 접속 여부를 로드하는 메서드
    public void LoadFirstTimeStatus()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // 최초 접속 여부 확인
        databaseReference.Child("users").Child(userId).Child("firstTime").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to check first time access from Firebase.");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // 만약 firstTime 데이터가 없으면, 최초 접속으로 간주
                if (!snapshot.Exists || snapshot.Value.ToString() == "false")
                {
                    Debug.Log("First time access detected. Showing guide.");
                    StartGuide(); // 최초 접속이면 가이드 실행
                    SaveFirstTimeStatus(true); // 최초 접속 상태 저장
                    isFirstTime = true;
                }
                else
                {
                    Debug.Log("User has accessed before. Skipping guide.");
                    isFirstTime = false;
                    // 가이드 스킵 후 일반 게임 시작 로직 추가 가능
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

    // 화살표를 특정 버튼 위에 표시하는 함수
    public void ShowArrowAboveButton(Button targetButton)
    {
        // 화살표를 버튼의 자식으로 설정
        arrowTransform.SetParent(targetButton.transform, false);

        // 화살표를 버튼의 위쪽으로 이동
        arrowTransform.anchoredPosition = new Vector2(0, arrowMoveDistance);  // 부모(Button)의 중심 기준으로 위로 이동

        // 화살표를 활성화
        arrowTransform.gameObject.SetActive(true);

        // 위아래로 반복해서 움직이는 애니메이션
        arrowTransform.DOAnchorPosY(arrowTransform.anchoredPosition.y + arrowMoveDistance, arrowMoveDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);  // 무한 반복
    }

    // 화살표를 숨기는 함수
    public void HideArrow()
    {
        arrowTransform.gameObject.SetActive(false);  // 화살표 비활성화
        DOTween.Kill(arrowTransform);  // DOTween 애니메이션 중단
    }

    private void ShowEventGuide()
    {
        guideText.text = "처음 오셨군요? 게임에 대해 알려 드릴께요. 다음 버튼을 눌러 진행해 주세요";
        guideTitle.text = "환영합니다!";

        //nextButtonHighlightCoroutine = StartCoroutine(HighlightNextButton());
        ShowArrowAboveButton(nextButton);  // 화살표 추가

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            //StopCoroutine(nextButtonHighlightCoroutine);
            nextButton.image.color = originalColor;  // 원래 색상으로 복구
            HideArrow();  // 화살표 숨기기
            ShowNextStep();  // 다음 단계로 이동
            nextButton.onClick.RemoveAllListeners();
        });
    }

    private void ShowBackButtonGuide()
    {
        guideText.text = "우선 뒤로가기 버튼을 눌러서 창을 닫아주세요.";
        guideTitle.text = "뒤로가기";
        backButton.gameObject.SetActive(true);

        ShowArrowAboveButton(backButton);  // 화살표 추가

        backButton.onClick.AddListener(() =>
        {
            HideArrow();  // 화살표 숨기기
            ShowNextStep();
            backButton.onClick.RemoveListener(HideArrow);
            backButton.onClick.RemoveListener(ShowNextStep);
        });
    }

    private void ShowCardDrawGuide()
    {
        guideText.text = "카드를 뽑아보세요!";
        guideTitle.text = "게임 진행";

        ShowArrowAboveButton(cardButton);  // 화살표 추가

        cardButton.onClick.AddListener(() =>
        {
            HideArrow();  // 화살표 숨기기
            ShowNextStep();
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(CardDrawer.Instance.OnDrawButtonClick);
        });
    }

    private void ShowWaypointGuide()
    {
        guideText.text = "한번 막아보세요!";
        guideTitle.text = "게임 진행";

        ShowArrowAboveButton(challengeButton);  // 화살표 추가

        challengeButton.onClick.AddListener(() =>
        {
            HideArrow();  // 화살표 숨기기
            ShowNextStep();
        });
    }

    private void DefenseGuide()
    {
        // 여기에 디펜스 관련 안내를 추가할 수 있습니다.
    }

    private void DefenseBoss()
    {
        defenseguideText.text = "잘하셨습니다! \n매 10라운드마다 보스가 존재 합니다. 디펜스는 총 20라운드 이지만.. 이번만 특별히 바로 보스를 내주죠 물리쳐 보세요!";
        defenseguideTitle.text = "게임 진행";
    }

    private void BossCoinActive()
    {
        defenseguideText.text = "보스를 처치시엔 보스코인을 지급합니다. \n코인은 아이템 뽑기 또는 재화 랜덤 뽑기에 사용 됩니다. \n아무거나 하나 해보시죠 운이 있는지 없는지!";
        ShowArrowAboveButton(itemButton);  // 화살표 추가

        itemButton.onClick.AddListener(() =>
        {
            isItem = true;
            HideArrow();  // 화살표 숨기기
            ShowNextStep();
            itemButton.onClick.RemoveListener(ShowNextStep);
            moneyButton.onClick.RemoveListener(ShowNextStep);
        });

        moneyButton.onClick.AddListener(() =>
        {
            isMoney = true;
            HideArrow();  // 화살표 숨기기
            ShowNextStep();
            moneyButton.onClick.RemoveListener(ShowNextStep);
            itemButton.onClick.RemoveListener(ShowNextStep);
        });
    }

    private void FinalBoss()
    {
        if (isItem)
        {
            defenseguideText.text = "아이템을 선택하셨군요. 아이템은 현재 3개의 아이템이 존재합니다! \n 재화는 30~500골드까지 확률에 따라 획득 합니다! 나중에 한입 해보세요!";
            ShowArrowAboveButton(defenseNextButton);  // 화살표 추가
        }
        else if (isMoney)
        {
            defenseguideText.text = "재화를 선택하셨군요. 재화는 30~500골드까지 확률에 따라 획득 합니다!  \n 아이템은 현재 3개의 아이템이 존재합니다! 다음을 눌러 진행해 주세요!";
            ShowArrowAboveButton(defenseNextButton);  // 화살표 추가
        }

        defenseNextButton.onClick.AddListener(() =>
        {
            HideArrow();  // 화살표 숨기기
            ShowNextStep();
        });
    }

    void FinalBoss2()
    {
        defenseguideText.text = "이제 튜토리얼 마지막 보스를 처치해 보세요!";
    }

    void UseMoney()
    {
        guideText.text = "고생하셨습니다! 디펜스의 난이도엔 1~3이 존재하며 각 난이도 클리어마다 \n 건물이 지어지고, 건물에 따라 세금이 생산 됩니다. 눌러서 획득해보세요";
        guideTitle.text = "세금 시스템";

        for (int i = 0; i < taxButton.Length; i++)
        {
            int index = i; // 클로저 문제를 해결하기 위해 지역 변수로 값을 캡처
            if (taxButton[index] == null) // null 체크
            {
                Debug.LogError($"taxButton[{index}] is null");
                continue;
            }

            ShowArrowAboveButton(taxButton[index]);  // 화살표 추가

            taxButton[index].onClick.AddListener(() =>
            {
                HideArrow();  // 화살표 숨기기
                ShowNextStep();
                taxButton[index].onClick.RemoveListener(ShowNextStep); // 이벤트 리스너 제거
            });
        }
    }

    private void UseGrade()
    {
        guideText.text = "좋아요! 세금과 클리어 재화를 얻었으니 사용해보죠. 강화 시스템입니다 강화 버튼을 눌러보세요.";
        guideTitle.text = "강화 시스템";

        ShowArrowAboveButton(gradeButton);  // 화살표 추가

        gradeButton.onClick.AddListener(() =>
        {
            HideArrow();  // 화살표 숨기기
            ShowNextStep();
            gradeButton.onClick.RemoveListener(ShowNextStep);
        });
    }

    private void UseGrade2()
    {
        guideText.text = "여기엔 현재 존재하는 모든 영웅들이 존재합니다. 마음에 드는 영웅 한명을 강화해보세요";
        guideTitle.text = "강화 시스템";

        for (int i = 0; i < gradeButton2.Length; i++)
        {
            int index = i; // 클로저 문제를 해결하기 위해 지역 변수로 값을 캡처
            if (gradeButton2[index] == null) // null 체크
            {
                Debug.LogError($"taxButton[{index}] is null");
                continue;
            }

            ShowArrowAboveButton(gradeButton2[index]);  // 화살표 추가

            gradeButton2[index].onClick.AddListener(() =>
            {
                HideArrow();  // 화살표 숨기기
                ShowNextStep();
                gradeButton2[index].onClick.RemoveListener(ShowNextStep); // 이벤트 리스너 제거
            });
        }
    }

    private void EndGuide()
    {
        guideText.text = "고생하셨습니다. 디펜스 도중 막힌다면 꾸준히 강화를 해 다시 도전해보세요! \n 이제 당신의 운을 테스트하러 출발 해보세요";
        guideTitle.text = "가이드 종료";

        ShowArrowAboveButton(nextButton);  // 화살표 추가

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            //StopCoroutine(nextButtonHighlightCoroutine);
            nextButton.image.color = originalColor;  // 원래 색상으로 복구
            HideArrow();  // 화살표 숨기기
            Destroy(guidePanel);
            isFirstTime = false;
        });
    }

    // 버튼을 반짝이게 만드는 함수
    //private IEnumerator HighlightNextButton()
    //{
    //    Color originalColor = nextButton.image.color; // 원래 버튼 색
    //    Color highlightColor = Color.yellow; // 강조할 색상

    //    while (true)
    //    {
    //        nextButton.image.color = highlightColor;
    //        yield return new WaitForSeconds(0.5f);
    //        nextButton.image.color = originalColor;
    //        yield return new WaitForSeconds(0.5f);
    //    }
    //}
}
