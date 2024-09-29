using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkill : Skill
{ 
    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {
        if (skillPrefab == null)
        {
            Debug.LogError("Skill prefab is not assigned!");
            return;
        }

        Debug.Log($"{caster.Name} used {skillName}");

        // ��ų ���� ���� ��� ���� �����Ͽ� �迭�� ��ȯ
        Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, skillRange, caster.enemyLayer);

        Monster highestHealthEnemy = null;
        float highestHealth = 0;

        // ������ �� �߿��� ���� ü���� ���� ���� ã��
        foreach (var hit in hits)
        {
            Monster enemy = hit.GetComponent<Monster>();
            if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie)
            {
                if (enemy.currentHealth > highestHealth)
                {
                    highestHealth = enemy.currentHealth;
                    highestHealthEnemy = enemy;
                }
            }
        }

        // ���� ü���� ���� ���� ���� ��� ��ų ����
        if (highestHealthEnemy != null)
        {
            GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
            SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
            if (skillBehavior != null)
            {
                skillBehavior.Initialize(caster, skillDamage, 0, highestHealthEnemy.transform.position, highestHealthEnemy.gameObject, true,false,slowAmount,true,defenseReductionAmount);
            }
        }
    }
}
