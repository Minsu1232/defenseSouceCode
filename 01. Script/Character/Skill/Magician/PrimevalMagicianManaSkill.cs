using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimevalMagicianManaSkill : ManaSkill
{

    public float damageIncreasePercentage = 20f; // ������ ���� ����
    public float damageIncreaseDuration = 10f; // ������ ���� ���� �ð�
    public GameObject debuffPrefab;
    public override void ActivateManaSkill(CharacterInfo caster, GameObject target)
    {


        // ��ų ���� ���� ��� ���� �����Ͽ� �迭�� ��ȯ
        Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, skillRange, caster.enemyLayer);

        // ������ ���� �ϳ��� �ִ��� Ȯ��
        foreach (var hit in hits)
        {
            Monster enemy = hit.GetComponent<Monster>();
            if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie) // ���� �����ϰ� ����ִ��� Ȯ��
            {
                Vector2 vector2 = enemy.transform.position; // ������ �Ӹ������� ������
                vector2.y += 7f;
                vector2.x += 1f;
                GameObject skillInstance = GameObject.Instantiate(skillPrefab, vector2, Quaternion.identity);
                SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
                if (skillBehavior != null)
                {
                    skillBehavior.Initialize(caster, skillDamage, skillRange, enemy.transform.position, enemy.gameObject, true, false, 0, false, 0, false, 15f);


                }
                enemy.ApplyDamageIncrease(damageIncreasePercentage, damageIncreaseDuration);
                // ���� Transform�� ������
                Transform enemyTransform = hit.transform;

                // ����� �������� ���� �ڽ����� ����
                GameObject debuff = GameObject.Instantiate(debuffPrefab, enemyTransform.position, Quaternion.identity);

                // ������ ����� ������Ʈ�� ���� �ڽ����� ����
                debuff.transform.SetParent(enemyTransform);
            }

        }
    }
}
