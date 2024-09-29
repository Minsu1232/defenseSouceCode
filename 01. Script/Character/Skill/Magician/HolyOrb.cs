using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyOrb : MonoBehaviour
{
    public CharacterInfo targetHero;
    private float buffDuration;
    public float travelTime = 1f; // ������ ���ư��� �ð�
    public Animator orbAnimator; // �ִϸ��̼� ��Ʈ�ѷ�
    public GameObject buffPrefab;
    public float arcHeight = 2f; // ������ ���� ����

    public void Initialize(CharacterInfo targetHero, float buffDuration)
    {
        this.targetHero = targetHero;
        this.buffDuration = buffDuration;

        // �ִϸ��̼� �ӵ��� �̵� �ð��� �°� ����
        //AdjustAnimationSpeed();

        // ��ü�� Ÿ�� �������� �̵�
        StartCoroutine(MoveToTarget());
    }

    //private void AdjustAnimationSpeed()
    //{
    //    // �̵� �ð��� ���� �ִϸ��̼� �ӵ� ���� (�ִϸ��̼� Ŭ�� ���̿� �°� ����)
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
            // ��� �ð� ���� ���
            float t = elapsedTime / travelTime;

            // ���� �̵� ���
            Vector2 currentPosition = Vector2.Lerp(startPosition, endPosition, t);

            // ������ ȿ�� �߰� (Y�����θ� �̵�)
            float heightOffset = Mathf.Sin(Mathf.PI * t) * arcHeight; // ������ ���
            currentPosition.y += heightOffset;

            // ���� ��ġ ������Ʈ
            transform.position = currentPosition;

            // ��� �ð� ������Ʈ
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // ���� �� ������ ��ġ�� ���� ����
        Vector2 vector2 = endPosition;
        vector2.x += 0.5f;
        vector2.y += 0.5f; // �ǹ� ����
        Instantiate(buffPrefab, vector2, Quaternion.identity);

        // ���� ����
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
