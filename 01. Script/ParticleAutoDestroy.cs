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
            // �ִϸ����Ͱ� ������ ��: �ִϸ��̼��� ���� �� ��ƼŬ ����
            float animationLength = GetAnimationLength(animator);
            StartCoroutine(DestroyAfterAnimation(animationLength + 0.1f)); // �ִϸ��̼� ���� +0.1�� �� ����
        }
        else
        {
            // �ִϸ����Ͱ� ���� ��: �ν����Ϳ��� ������ �෹�̼� �ð� ���� ����
            StartCoroutine(DestroyAfterAnimation(duration));
        }
    }

    // �ִϸ��̼� ���̸� ���ϴ� �Լ�
    private float GetAnimationLength(Animator animator)
    {
        if (animator.runtimeAnimatorController != null)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                return clips[0].length; // ù ��° �ִϸ��̼� Ŭ���� ���̸� ��ȯ
            }
        }
        return 0f;
    }

    // ���� �ð� �Ŀ� ��ƼŬ ������Ʈ ����
    private IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject); // ��ƼŬ ������Ʈ ����
    }
}
