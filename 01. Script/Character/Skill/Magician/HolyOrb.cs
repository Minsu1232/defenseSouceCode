using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyOrb : MonoBehaviour
{
    public CharacterInfo targetHero;
    private float buffDuration;
    public float travelTime = 1f; // 버프가 날아가는 시간
    public Animator orbAnimator; // 애니메이션 컨트롤러
    public GameObject buffPrefab;
    public float arcHeight = 2f; // 포물선 높이 조정

    public void Initialize(CharacterInfo targetHero, float buffDuration)
    {
        this.targetHero = targetHero;
        this.buffDuration = buffDuration;

        // 애니메이션 속도를 이동 시간에 맞게 조정
        //AdjustAnimationSpeed();

        // 구체를 타겟 영웅에게 이동
        StartCoroutine(MoveToTarget());
    }

    //private void AdjustAnimationSpeed()
    //{
    //    // 이동 시간에 맞춰 애니메이션 속도 조정 (애니메이션 클립 길이에 맞게 설정)
    //    if (orbAnimator != null)
    //    {
    //        orbAnimator.speed = orbAnimator.runtimeAnimatorController.animationClips[0].length / travelTime;
    //    }
    //}

    private IEnumerator MoveToTarget()
    {
        Vector2 startPosition = transform.position;
        Vector2 endPosition = targetHero.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < travelTime)
        {
            // 경과 시간 비율 계산
            float t = elapsedTime / travelTime;

            // 직선 이동 계산
            Vector2 currentPosition = Vector2.Lerp(startPosition, endPosition, t);

            // 포물선 효과 추가 (Y축으로만 이동)
            float heightOffset = Mathf.Sin(Mathf.PI * t) * arcHeight; // 포물선 계산
            currentPosition.y += heightOffset;

            // 현재 위치 업데이트
            transform.position = currentPosition;

            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 도착 시 보정된 위치에 버프 생성
        Vector2 vector2 = endPosition;
        vector2.x += 0.5f;
        vector2.y += 0.5f; // 피벗 보정
        Instantiate(buffPrefab, vector2, Quaternion.identity);

        // 버프 적용
        ApplyBuff();
    }

    private void ApplyBuff()
    {
        if (targetHero.GetComponent<CharacterInfo>() == null)
        {
            Debug.LogError("Target Hero does not have a CharacterInfo component!");
        }
        targetHero.ApplyBuff(buffDuration);
        Destroy(gameObject);
    }
}
