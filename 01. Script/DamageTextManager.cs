using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; }

    public TextPool textPool;
    public GameObject criticalImagePrefab;  // ũ��Ƽ�� �̹��� ������

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

    //public void ShowDamageText(Transform monsterTransform, int damageAmount)
    //{
    //    GameObject damageCanvasObj = textPool.GetDamageCanvas();
    //    damageCanvasObj.transform.SetParent(monsterTransform);
    //    damageCanvasObj.transform.localPosition = Vector3.zero;

    //    TextMeshProUGUI damageText = damageCanvasObj.GetComponentInChildren<TextMeshProUGUI>();
    //    damageText.text = damageAmount.ToString();

    //    StartCoroutine(FadeOutAndReturn(damageCanvasObj));
    //}

    public void ShowDamageTextColor(Transform monsterTransform, int damageAmount, Color textColor)
    {
        GameObject damageCanvasObj = textPool.GetDamageCanvas();
        damageCanvasObj.transform.SetParent(monsterTransform);
        damageCanvasObj.transform.localPosition = Vector3.zero;

        TextMeshProUGUI damageText = damageCanvasObj.GetComponentInChildren<TextMeshProUGUI>();
        damageText.text = damageAmount.ToString();
        damageText.color = textColor;  // �ؽ�Ʈ ���� ����

       

        StartCoroutine(FadeOutAndReturn(damageCanvasObj));
    }
    public void ShowGetMoneyText(Transform monsterTransform, int amount)
    {
        GameObject damageCanvasObj = textPool.GetDamageCanvas();
        damageCanvasObj.transform.SetParent(monsterTransform);
        Vector3 amountText = Vector3.zero;
        amountText.y -= 1.5f;
        damageCanvasObj.transform.localPosition = amountText;

        TextMeshProUGUI damageText = damageCanvasObj.GetComponentInChildren<TextMeshProUGUI>();
        damageText.text = $"+{amount}"; // �ݾ��� �ؽ�Ʈ�� ǥ��

        // ���� ǥ���� �� ������ Ȳ�ݻ����� ����
        damageText.color = Color.white;

        // �ؽ�Ʈ �ִϸ��̼� (���̵� �ƿ� �� �̵�)
        StartCoroutine(FadeOutAndReturn(damageCanvasObj));
    }
    private IEnumerator FadeOutAndReturn(GameObject obj)
    {
        // ������Ʈ�� ������Ʈ�� null���� Ȯ��
        if (obj == null)
        {
            yield break; // ������Ʈ�� null�̸� �ڷ�ƾ�� ����
        }

        TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (text == null)
        {
            yield break; // �ؽ�Ʈ ������Ʈ�� ������ �ڷ�ƾ�� ����
        }

        Vector3 initialPosition = obj.transform.localPosition;
        Vector3 targetPosition = initialPosition + new Vector3(0, 1f, 0); // ���� 1 ���� �̵�

        float duration = 1f; // �ִϸ��̼� ���� �ð�
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // ������Ʈ�� ������ ��ȿ���� Ȯ��
            if (obj == null || text == null)
            {
                yield break; // ������Ʈ �Ǵ� �ؽ�Ʈ ������Ʈ�� null�̸� �ڷ�ƾ�� ����
            }

            // ��ġ�� õõ�� ���� �̵�
            obj.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);

            // �ؽ�Ʈ ���� (����) ���������� ����
            //text.color = new Color(text.color.r, text.color.g, text.color.b, 1 - (elapsedTime / duration));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ������ ��ġ�� ���� ����
        if (obj != null && text != null)
        {
            //obj.transform.localPosition = targetPosition;
            //text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }

        // ������Ʈ�� ������ ��ȿ�ϴٸ� Ǯ�� ��ȯ
        if (obj != null)
        {
            obj.SetActive(false);
            textPool.ReturnDamageCanvas(obj); // ��� �� Ǯ�� ��ȯ
        }
    }
}
