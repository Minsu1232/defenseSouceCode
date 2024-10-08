using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ArrowRainSkill : Skill
{
    public ArrowRainSkill(Skill data)
    {
        // SkillData에서 공통 데이터 할당
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
    {   
        base.ActivateSkill(caster, target);
        if (skillPrefab == null)
        {
            Debug.LogError("Skill prefab is not assigned!");
            return;
        }

        Debug.Log($"{caster.Name} used {skillName}");

        // 스킬 범위 내의 모든 적을 감지하여 배열로 반환
        Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, skillRange, caster.enemyLayer);

        // 감지된 적이 하나라도 있는지 확인
        foreach (var hit in hits)
        {
            Monster enemy = hit.GetComponent<Monster>();
            if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie) // 적이 존재하고 살아있는지 확인
            {
                Vector2 vector2 = target.transform.position; // 몬스터의 머리위에서 활이 떨어짐
                        vector2.y += 2.5f;
                GameObject skillInstance = GameObject.Instantiate(skillPrefab, vector2, Quaternion.identity);
                SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
                if (skillBehavior != null)
                {
                    skillBehavior.Initialize(caster, finalDamage, skillRange, enemy.transform.position, enemy.gameObject,isSingtarget,hasSlowEffect, slowAmount, hasDefenseReduction, defenseReductionAmount,isSpecialSkill,duration,speed);
                }
            }
        }
    }
}
