using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicianSkill : Skill
{
    public MagicianSkill(Skill data)
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

        GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
        SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
        if (skillBehavior != null)
        {
            Vector3 targetPosition = target.transform.position;
            skillBehavior.Initialize(caster, finalDamage, skillRange, targetPosition, target, isSingtarget, hasSlowEffect, slowAmount, hasDefenseReduction, defenseReductionAmount, false, 2f);

            caster.totalDamageDealt += skillDamage;
        }
    }
}

