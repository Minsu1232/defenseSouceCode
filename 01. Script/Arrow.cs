using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private GameObject target;
    private CharacterInfo caster;
    private float damage;
    private float speed = 10f;
    private Vector3 direction;

    // Initialize에서 화살의 방향과 회전을 설정
    public void Initialize(GameObject target, float damage, CharacterInfo caster)
    {
        this.target = target;
        this.damage = damage;
        this.caster = caster;

        // 타겟을 향한 방향을 계산
        UpdateDirectionAndRotation();
    }

    private void Update()
    {
        if (target != null)
        {
            // 매 프레임마다 타겟의 위치를 향해 방향과 회전 계산
            UpdateDirectionAndRotation();

            // 타겟을 향해 화살 이동
            transform.position += direction * speed * Time.deltaTime;

            // 화살이 타겟에 근접했는지 확인
            if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
            {
                HitTarget();
            }
        }
        else
        {
            Destroy(gameObject); // 타겟이 없으면 화살 파괴
        }
    }

    private void UpdateDirectionAndRotation()
    {
        // 타겟의 위치를 향해 방향 계산
        direction = (target.transform.position - transform.position).normalized;

        // 타겟을 향해 화살이 회전하도록 설정
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void HitTarget()
    {
        Monster enemy = target.GetComponent<Monster>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, CharacterData.AttackType.Archer); // 데미지 적용
            float finalDamage = damage * enemy.GetDamageMultiplier(enemy.defense);

            // 자신이 가한 데미지를 누적
            caster.totalDamageDealt += finalDamage;
        }
        Destroy(gameObject); // 화살 파괴
    }
}
