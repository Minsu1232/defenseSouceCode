using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaoPuSkill : Skill
{
    public BaoPuSkill(Skill data)
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
        isSpecialSkill = data.isSpecialSkill;
        duration = data.duration;
        speed = data.speed;
        manaCost = data.manaCost;
        manaRegenRate = data.manaRegenRate;
        maxMana = data.maxMana;
        manaGainPerAttack = data.manaGainPerAttack;
        cloneDuration = data.cloneDuration;
        clonePowerMultiplier = data.clonePowerMultiplier;
    }
    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {   
        base.ActivateSkill(caster, target);
        caster.IncreaseStats(0.5f, 0.001f, 0.001f);

        if (target != null)
        {
            Vector2 vector2 = target.transform.position; // 몬스터의 머리위에서 활이 떨어짐
            vector2.y += 2.5f;
            GameObject skillInstance = GameObject.Instantiate(skillPrefab, vector2, Quaternion.identity);
            SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
            if (skillBehavior != null)
            {
                skillBehavior.Initialize(caster, finalDamage, skillRange, target.transform.position, target.gameObject, isSingtarget, false, 0, false, 0, false,12f);                
            }
        }
    }
   
}
