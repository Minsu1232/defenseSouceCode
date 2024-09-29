using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ArrowRainSkill : Skill
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

        // ������ ���� �ϳ��� �ִ��� Ȯ��
        foreach (var hit in hits)
        {
            Monster enemy = hit.GetComponent<Monster>();
            if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie) // ���� �����ϰ� ����ִ��� Ȯ��
            {
                Vector2 vector2 = target.transform.position; // ������ �Ӹ������� Ȱ�� ������
                        vector2.y += 2.5f;
                GameObject skillInstance = GameObject.Instantiate(skillPrefab, vector2, Quaternion.identity);
                SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
                if (skillBehavior != null)
                {
                    skillBehavior.Initialize(caster, skillDamage, skillRange, enemy.transform.position, enemy.gameObject, true, false, 0, false, 0);
                }
            }
        }
    }
}
