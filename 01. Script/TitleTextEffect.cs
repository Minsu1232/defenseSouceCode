using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleTextEffect : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public float scaleDuration = 1.0f; // 텍스트 커지는 시간
    public float bounceDuration = 0.3f; // 통통 튀는 시간
    public float tiltAngle = 15f; // 기울기 각도
    public float blinkDuration = 0.1f; // 깜빡이는 시간
    public int blinkCount = 2; // 깜빡이는 횟수
    public TextMeshProUGUI tapText;

    void Start()
    {
        // 시작 크기를 0으로 설정
        titleText.transform.localScale = Vector3.zero;

        // 텍스트에 글로우 효과 추가
        titleText.fontMaterial.EnableKeyword("GLOW_ON");
        titleText.fontMaterial.SetColor("_GlowColor", Color.cyan); // 글로우 색상 설정
        titleText.fontMaterial.SetFloat("_GlowPower", 0.5f); // 글로우 강도 설정

        // 텍스트 크기 애니메이션 시작 (0에서 200까지)
        StartScaleEffect();
    }

    void StartScaleEffect()
    {
        // 텍스트 크기를 0에서 200으로 변경하는 애니메이션
        titleText.transform.DOScale(Vector3.one * 200, scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // 통통 튀는 효과 추가
            StartBounceEffect();
        });
    }

    void StartBounceEffect()
    {
        // 통통 튀는 애니메이션 (위치 이동 없이, 크기 변화만 포함)
        Sequence bounceSequence = DOTween.Sequence();
        bounceSequence.Append(titleText.transform.DORotate(new Vector3(0, 0, -tiltAngle), bounceDuration / 2).SetEase(Ease.OutQuad));
        bounceSequence.Append(titleText.transform.DORotate(Vector3.zero, bounceDuration / 2).SetEase(Ease.InQuad));

        // 깜빡임 효과 추가
        bounceSequence.OnComplete(() =>
        {
            StartBlinkEffect();
        });
    }

    void StartBlinkEffect()
    {
        // 깜빡임 효과
        Sequence blinkSequence = DOTween.Sequence();
        Color originalColor = titleText.color;

        for (int i = 0; i < blinkCount; i++)
        {
            blinkSequence.Append(titleText.DOFade(0f, blinkDuration));
            blinkSequence.Append(titleText.DOFade(1f, blinkDuration));
        }

        // 깜빡임 효과 종료 후 "탭 하여 시작하기" 텍스트 표시
        blinkSequence.OnComplete(() =>
        {
            titleText.color = originalColor;
            tapText.text = "탭 하여 시작하기";
        });
    }
}
