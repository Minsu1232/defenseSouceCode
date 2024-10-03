using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByenightManaSkill : ManaSkill
{
    public ByenightManaSkill(Skill data)
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
        // ĳ���Ϳ��� �н� ������ ��û
        caster.SpawnClone(skillPrefab, cloneDuration, clonePowerMultiplier, target);

        // ���� �ʱ�ȭ
        currentMana = 0;
        UpdateManaBar(); // ������ ������Ʈ
    }

    public void GainManaOnAttack()
    {
        currentMana += manaGainPerAttack;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        UpdateManaBar(); // ������ ������Ʈ
    }
}
