using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    private Animator animator;

    public float duration;
    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // 애니메이터가 존재할 때: 애니메이션이 끝난 후 파티클 삭제
            float animationLength = GetAnimationLength(animator);
            StartCoroutine(DestroyAfterAnimation(animationLength + 0.1f)); // 애니메이션 길이 +0.1초 뒤 삭제
        }
        else
        {
            // 애니메이터가 없을 때: 인스펙터에서 설정한 듀레이션 시간 이후 삭제
            StartCoroutine(DestroyAfterAnimation(duration));
        }
    }

    // 애니메이션 길이를 구하는 함수
    private float GetAnimationLength(Animator animator)
    {
        if (animator.runtimeAnimatorController != null)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                return clips[0].length; // 첫 번째 애니메이션 클립의 길이를 반환
            }
        }
        return 0f;
    }

    // 일정 시간 후에 파티클 오브젝트 삭제
    private IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject); // 파티클 오브젝트 삭제
    }
}
