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
        // Ÿ���� ��ġ�� ���� ���� ���
        direction = (target.transform.position - transform.position).normalized;

        // Ÿ���� ���� ȭ���� ȸ���ϵ��� ����
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

            // �ڽ��� ���� �������� ����
            caster.totalDamageDealt += finalDamage;
        }
        Destroy(gameObject);
    }
}
