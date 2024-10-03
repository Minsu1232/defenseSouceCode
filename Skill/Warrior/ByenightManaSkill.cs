using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByenightManaSkill : ManaSkill
{
    public ByenightManaSkill(Skill data)
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
   

    public override void ActivateManaSkill(CharacterInfo caster, GameObject target)
    {
        base.ActivateManaSkill(caster, target);
        // 캐릭터에게 분신 생성을 요청
        caster.SpawnClone(skillPrefab, cloneDuration, clonePowerMultiplier, target);

        // 마나 초기화
        currentMana = 0;
        UpdateManaBar(); // 마나바 업데이트
    }

    public void GainManaOnAttack()
    {
        currentMana += manaGainPerAttack;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        UpdateManaBar(); // 마나바 업데이트
    }
}
