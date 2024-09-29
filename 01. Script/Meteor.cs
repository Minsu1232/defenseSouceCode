using UnityEngine;
using DG.Tweening; // DoTween 네임스페이스 추가

public class Meteor : MonoBehaviour
{
    public Vector3 startPosition;  // 시작 위치 (카메라 영역 밖)
    public Vector3 targetPosition; // 목표 위치 (운석이 떨어질 위치)
    public float travelTime = 2f;  // 이동 시간 (몇 초 동안 이동할지)
    public float damage = 100000f; // 운석이 몬스터에게 줄 피해량
    public float impactRadius = 15f; // 충돌 시 몬스터에게 피해를 줄 범위
    public LayerMask monsterLayer; // 몬스터 레이어 마스크
    public GameObject explosionEffect; // 충돌시 폭발 이펙트
    public GameObject hitParticle; // 데미지 이펙트
    

    void Start()
    {
        // 운석의 초기 위치를 설정
        transform.position = startPosition;

        // 운석의 회전값 설정 (z축으로 45도 회전)
        Vector3 euler = transform.eulerAngles;
        euler.z += 45f;
        transform.eulerAngles = euler;

        // DoTween을 사용하여 운석을 목표 위치로 이동시킴
        transform.DOMove(targetPosition, travelTime).OnComplete(OnReachTarget);
    }

    // 목표 위치에 도달했을 때 호출되는 메서드
    private void OnReachTarget()
    {
        Debug.Log("Meteor has reached the target position!");

        // 범위 내의 몬스터를 모두 탐색
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, impactRadius, monsterLayer);

        foreach (var hit in hits)
        {
            // 몬스터 객체를 찾음
            Monster enemy = hit.GetComponent<Monster>();
            if (enemy != null)
            {
                // 몬스터에게 피해를 입힘
                enemy.TakeDamage(damage,CharacterData.AttackType.Warrior);
                Vector2 vector = hit.transform.position; //히트파티클 위치조정
                //vector.x += 3f;
                Instantiate(hitParticle,vector,Quaternion.identity);
                Vector2 vector2 = targetPosition;
                vector2.y += 0.9f;
               Instantiate(explosionEffect,vector2,Quaternion.identity);
                Debug.Log($"Meteor hit {enemy.name} and dealt {damage} damage.");
            }
        }

        // 필요 시, 추가적인 처리 (폭발 효과, 파괴 등)
        Destroy(gameObject); // 운석을 파괴
    }

    // 범위를 시각적으로 표시하기 위한 Gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
