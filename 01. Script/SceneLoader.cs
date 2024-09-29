using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening; // DOTween ����� ���� ���ӽ����̽�
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; } // �̱��� �ν��Ͻ�

    public GameObject Introtext;
    public GameObject Introtext2;
    [SerializeField] private CanvasGroup loadingPanelCanvasGroup; // CanvasGroup���� ���� ����
    public TextMeshProUGUI loadingText; // "�ε���" �ؽ�Ʈ (TextMeshPro ��� ��)
    public string loadScene;
    public float fadeDuration = 4f; // ���̵� �ִϸ��̼� ���� �ð�
    private bool isAnimatingLoadingText = false; // �ε� �ؽ�Ʈ �ִϸ��̼� ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ε�Ǿ �ı����� ����
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

        // �ε� �г� Ȱ��ȭ �� ���̵� �� ����
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    // ���̵� ȿ���� �񵿱� �� �ε带 ó���ϴ� �ڷ�ƾ
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // �ε� �ؽ�Ʈ �ִϸ��̼� ����
        if (!isAnimatingLoadingText)
        {
            StartCoroutine(AnimateLoadingText());
        }

        // ���̵� �� (�ε� ȭ�� ��Ÿ��)
        yield return FadeIn();

        // �񵿱� �� �ε� ����
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // ���� �ε�� �� �ٷ� Ȱ��ȭ���� �ʵ��� ����

        // �� �ε尡 �Ϸ�� ������ ���
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f) // ���� ���� �ε�Ǿ��� �� (�� �ε� 90%)
            {
                yield return new WaitForSeconds(0.8f); //  ����
                operation.allowSceneActivation = true; // ���� �� Ȱ��ȭ
            }
            yield return null; // �� ������ ���
        }

        // �� �ε� �Ϸ� �� ��� �����ϰ� ���̵� �ƿ�
        yield return new WaitForSeconds(1f); // ���̵� �ƿ� �� ��� ���
        yield return FadeOut();
    }

    // ���̵� �� ȿ�� (���� 0 -> 1)
    private IEnumerator FadeIn()
    {   
        loadingText.gameObject.SetActive(true);
        loadingPanelCanvasGroup.alpha = 1f; // �ٷ� ���� ���� 1�� �����Ͽ� ��� ��ο���
        loadingPanelCanvasGroup.gameObject.SetActive(true); // �г� Ȱ��ȭ

        yield return null; // ��� �Ϸ�ǵ��� �� ������ ���
    }

    // ���̵� �ƿ� ȿ�� (���� 1 -> 0)
    private IEnumerator FadeOut()
    {   
        loadingText.gameObject.SetActive(false);
        yield return loadingPanelCanvasGroup.DOFade(0f, fadeDuration).WaitForCompletion(); // DOTween���� ���̵� �ƿ�
        loadingPanelCanvasGroup.gameObject.SetActive(false); // ���̵� �ƿ� �� �г� ��Ȱ��ȭ

        // �ε� �ؽ�Ʈ �ִϸ��̼� ����
        isAnimatingLoadingText = false;
    }

    // "�ε���", "�ε���.", "�ε���..", "�ε���..." �ִϸ��̼� ó��
    private IEnumerator AnimateLoadingText()
    {
        isAnimatingLoadingText = true;
        string baseText = "�ε���";
        int dotCount = 0;

        while (isAnimatingLoadingText)
        {
            // �� ������ ���� �ؽ�Ʈ ����
            loadingText.text = baseText + new string('.', dotCount);

            // 0.5�� ���
            yield return new WaitForSeconds(0.5f);

            // �� ���� ������Ʈ (0, 1, 2, 3 -> �ݺ�)
            dotCount = (dotCount + 1) % 4;
        }
    }
}
 