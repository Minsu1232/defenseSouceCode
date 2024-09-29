using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; }

    public TextPool textPool;
    public GameObject criticalImagePrefab;  // 크리티컬 이미지 프리팹

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
        damageText.color = textColor;  // 텍스트 색상 설정

       

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
        damageText.text = $"+{amount}"; // 금액을 텍스트로 표시

        // 돈을 표시할 때 색상을 황금색으로 설정
        damageText.color = Color.white;

        // 텍스트 애니메이션 (페이드 아웃 및 이동)
        StartCoroutine(FadeOutAndReturn(damageCanvasObj));
    }
    private IEnumerator FadeOutAndReturn(GameObject obj)
    {
        // 오브젝트와 컴포넌트가 null인지 확인
        if (obj == null)
        {
            yield break; // 오브젝트가 null이면 코루틴을 종료
        }

        TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (text == null)
        {
            yield break; // 텍스트 컴포넌트가 없으면 코루틴을 종료
        }

        Vector3 initialPosition = obj.transform.localPosition;
        Vector3 targetPosition = initialPosition + new Vector3(0, 1f, 0); // 위로 1 단위 이동

        float duration = 1f; // 애니메이션 지속 시간
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 오브젝트가 여전히 유효한지 확인
            if (obj == null || text == null)
            {
                yield break; // 오브젝트 또는 텍스트 컴포넌트가 null이면 코루틴을 종료
            }

            // 위치를 천천히 위로 이동
            obj.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);

            // 텍스트 색상 (투명도) 점진적으로 변경
            //text.color = new Color(text.color.r, text.color.g, text.color.b, 1 - (elapsedTime / duration));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 마지막 위치와 투명도 설정
        if (obj != null && text != null)
        {
            //obj.transform.localPosition = targetPosition;
            //text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }

        // 오브젝트가 여전히 유효하다면 풀로 반환
        if (obj != null)
        {
            obj.SetActive(false);
            textPool.ReturnDamageCanvas(obj); // 사용 후 풀로 반환
        }
    }
}
