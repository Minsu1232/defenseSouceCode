using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBolt : MonoBehaviour
{
    private GameObject target;
    private float damage;
    private CharacterInfo caster;
    private float speed = 12f;
    private Vector3 direction;

    public void Initialize(GameObject target, float damage, CharacterInfo caster)
    {
        this.target = target;
        this.damage = damage;
        this.caster = caster;
    }

    private void Update()
    {
        if (target != null)
        {
            //UpdateDirectionAndRotation();

            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
            {
                HitTarget();
            }
        }
        else
        {
            Destroy(gameObject);
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
            enemy.TakeDamage(damage, CharacterData.AttackType.Magic);
            float finalDamage = damage * enemy.GetDamageMultiplier(enemy.defense);

            // 자신이 가한 데미지를 누적
            caster.totalDamageDealt += finalDamage;
        }
        Destroy(gameObject);
    }
}
