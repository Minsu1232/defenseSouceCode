using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleTextEffect : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public float scaleDuration = 1.0f; // �ؽ�Ʈ Ŀ���� �ð�
    public float bounceDuration = 0.3f; // ���� Ƣ�� �ð�
    public float tiltAngle = 15f; // ���� ����
    public float blinkDuration = 0.1f; // �����̴� �ð�
    public int blinkCount = 2; // �����̴� Ƚ��
    public TextMeshProUGUI tapText;

    void Start()
    {
        // ���� ũ�⸦ 0���� ����
        titleText.transform.localScale = Vector3.zero;

        // �ؽ�Ʈ�� �۷ο� ȿ�� �߰�
        titleText.fontMaterial.EnableKeyword("GLOW_ON");
        titleText.fontMaterial.SetColor("_GlowColor", Color.cyan); // �۷ο� ���� ����
        titleText.fontMaterial.SetFloat("_GlowPower", 0.5f); // �۷ο� ���� ����

        // �ؽ�Ʈ ũ�� �ִϸ��̼� ���� (0���� 200����)
        StartScaleEffect();
    }

    void StartScaleEffect()
    {
        // �ؽ�Ʈ ũ�⸦ 0���� 200���� �����ϴ� �ִϸ��̼�
        titleText.transform.DOScale(Vector3.one * 200, scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            // ���� Ƣ�� ȿ�� �߰�
            StartBounceEffect();
        });
    }

    void StartBounceEffect()
    {
        // ���� Ƣ�� �ִϸ��̼� (��ġ �̵� ����, ũ�� ��ȭ�� ����)
        Sequence bounceSequence = DOTween.Sequence();
        bounceSequence.Append(titleText.transform.DORotate(new Vector3(0, 0, -tiltAngle), bounceDuration / 2).SetEase(Ease.OutQuad));
        bounceSequence.Append(titleText.transform.DORotate(Vector3.zero, bounceDuration / 2).SetEase(Ease.InQuad));

        // ������ ȿ�� �߰�
        bounceSequence.OnComplete(() =>
        {
            StartBlinkEffect();
        });
    }

    void StartBlinkEffect()
    {
        // ������ ȿ��
        Sequence blinkSequence = DOTween.Sequence();
        Color originalColor = titleText.color;

        for (int i = 0; i < blinkCount; i++)
        {
            blinkSequence.Append(titleText.DOFade(0f, blinkDuration));
            blinkSequence.Append(titleText.DOFade(1f, blinkDuration));
        }

        // ������ ȿ�� ���� �� "�� �Ͽ� �����ϱ�" �ؽ�Ʈ ǥ��
        blinkSequence.OnComplete(() =>
        {
            titleText.color = originalColor;
            tapText.text = "�� �Ͽ� �����ϱ�";
        });
    }
}
