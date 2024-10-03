using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MythMagicianBuff : ManaSkill
{
    public MythMagicianBuff(Skill data)
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
        caster.StartCoroutine(Buff(caster,10));
        skillPrefab.SetActive(true);
    }
    IEnumerator Buff(CharacterInfo caster, int duration)
    {
        float attack = caster.baseAttackPower * 0.15f;
        float speed = caster.baseAttackSpeed * 0.15f;
        float critical = caster.baseAttackCritical * 0.1f;
        caster.IncreaseStats(attack, speed, critical); // 스탯 증가
        skillPrefab.transform.SetParent(caster.transform, false);
        skillPrefab.SetActive(true);
        yield return new WaitForSeconds(duration); // 버프 지속 시간
        skillPrefab.SetActive(false);
        caster.IncreaseStats(attack,speed, critical); // 스탯 복구
    }
}
