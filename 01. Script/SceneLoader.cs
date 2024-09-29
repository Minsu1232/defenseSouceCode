using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening; // DOTween 사용을 위한 네임스페이스
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; } // 싱글턴 인스턴스

    public GameObject Introtext;
    public GameObject Introtext2;
    [SerializeField] private CanvasGroup loadingPanelCanvasGroup; // CanvasGroup으로 투명도 조절
    public TextMeshProUGUI loadingText; // "로딩중" 텍스트 (TextMeshPro 사용 시)
    public string loadScene;
    public float fadeDuration = 4f; // 페이드 애니메이션 지속 시간
    private bool isAnimatingLoadingText = false; // 로딩 텍스트 애니메이션 상태

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 로드되어도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        if (Introtext != null && Introtext2 != null)
        {
            Destroy(Introtext);
            Destroy(Introtext2);
        }

        // 로딩 패널 활성화 및 페이드 인 시작
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    // 페이드 효과와 비동기 씬 로드를 처리하는 코루틴
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // 로딩 텍스트 애니메이션 시작
        if (!isAnimatingLoadingText)
        {
            StartCoroutine(AnimateLoadingText());
        }

        // 페이드 인 (로딩 화면 나타남)
        yield return FadeIn();

        // 비동기 씬 로드 시작
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // 씬이 로드된 후 바로 활성화되지 않도록 설정

        // 씬 로드가 완료될 때까지 대기
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f) // 씬이 거의 로드되었을 때 (씬 로딩 90%)
            {
                yield return new WaitForSeconds(0.8f); //  지연
                operation.allowSceneActivation = true; // 이제 씬 활성화
            }
            yield return null; // 한 프레임 대기
        }

        // 씬 로드 완료 후 잠깐 유지하고 페이드 아웃
        yield return new WaitForSeconds(1f); // 페이드 아웃 전 잠깐 대기
        yield return FadeOut();
    }

    // 페이드 인 효과 (투명도 0 -> 1)
    private IEnumerator FadeIn()
    {   
        loadingText.gameObject.SetActive(true);
        loadingPanelCanvasGroup.alpha = 1f; // 바로 알파 값을 1로 설정하여 즉시 어두워짐
        loadingPanelCanvasGroup.gameObject.SetActive(true); // 패널 활성화

        yield return null; // 즉시 완료되도록 한 프레임 대기
    }

    // 페이드 아웃 효과 (투명도 1 -> 0)
    private IEnumerator FadeOut()
    {   
        loadingText.gameObject.SetActive(false);
        yield return loadingPanelCanvasGroup.DOFade(0f, fadeDuration).WaitForCompletion(); // DOTween으로 페이드 아웃
        loadingPanelCanvasGroup.gameObject.SetActive(false); // 페이드 아웃 후 패널 비활성화

        // 로딩 텍스트 애니메이션 종료
        isAnimatingLoadingText = false;
    }

    // "로딩중", "로딩중.", "로딩중..", "로딩중..." 애니메이션 처리
    private IEnumerator AnimateLoadingText()
    {
        isAnimatingLoadingText = true;
        string baseText = "로딩중";
        int dotCount = 0;

        while (isAnimatingLoadingText)
        {
            // 점 개수에 따라 텍스트 설정
            loadingText.text = baseText + new string('.', dotCount);

            // 0.5초 대기
            yield return new WaitForSeconds(0.5f);

            // 점 개수 업데이트 (0, 1, 2, 3 -> 반복)
            dotCount = (dotCount + 1) % 4;
        }
    }
}
 