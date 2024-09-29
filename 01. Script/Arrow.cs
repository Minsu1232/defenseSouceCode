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

    // Initialize���� ȭ���� ����� ȸ���� ����
    public void Initialize(GameObject target, float damage, CharacterInfo caster)
    {
        this.target = target;
        this.damage = damage;
        this.caster = caster;

        // Ÿ���� ���� ������ ���
        UpdateDirectionAndRotation();
    }

    private void Update()
    {
        if (target != null)
        {
            // �� �����Ӹ��� Ÿ���� ��ġ�� ���� ����� ȸ�� ���
            UpdateDirectionAndRotation();

            // Ÿ���� ���� ȭ�� �̵�
            transform.position += direction * speed * Time.deltaTime;

            // ȭ���� Ÿ�ٿ� �����ߴ��� Ȯ��
            if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
            {
                HitTarget();
            }
        }
        else
        {
            Destroy(gameObject); // Ÿ���� ������ ȭ�� �ı�
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
            enemy.TakeDamage(damage, CharacterData.AttackType.Archer); // ������ ����
            float finalDamage = damage * enemy.GetDamageMultiplier(enemy.defense);

            // �ڽ��� ���� �������� ����
            caster.totalDamageDealt += finalDamage;
        }
        Destroy(gameObject); // ȭ�� �ı�
    }
}
