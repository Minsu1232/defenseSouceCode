using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArrowShotSkill : Skill
{
    public ArrowShotSkill(Skill data)
    {
        // SkillData���� ���� ������ �Ҵ�
        ID = data.ID;
        skillName = data.skillName;
        skillDescription = data.skillDescription;
        skillDamage = data.skillDamage;
        skillRange = data.skillRange;
        skillProbability = data.skillProbability;
        skillPrefab = data.skillPrefab;
        isSingtarget = data.isSingtarget;
        hasSlowEffect = data.hasSlowEffect;
        slowAmount = data.slowAmount;
        hasDefenseReduction = data.hasDefenseReduction;
        defenseReductionAmount = data.defenseReductionAmount;
    }

    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {   base.ActivateSkill(caster, target);
        if (skillPrefab == null)
        {
            Debug.LogError("Skill prefab is not assigned!");
            return;
        }

        // ��ų ���� ���� ��� ���� �����Ͽ� �迭�� ��ȯ
        Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, skillRange, caster.enemyLayer);

        foreach (var hit in hits)
        {
            Monster enemy = hit.GetComponent<Monster>();
            if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie)
            {
                GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
                SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
                if (skillBehavior != null)
                {
                    skillBehavior.Initialize(caster, finalDamage, skillRange, enemy.transform.position, enemy.gameObject, isSingtarget, hasSlowEffect, slowAmount, hasDefenseReduction, defenseReductionAmount, isSpecialSkill, duration, speed);
                }
            }
        }
    }
}
