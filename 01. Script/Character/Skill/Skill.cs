using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillName; // 스킬이름
    public string skillDescription; // 스킬설명
    public float skillDamage; // 스킬데미지
    public float skillRange; // 스킬범위
    public float skillProbability; // 스킬확률
    public GameObject skillPrefab;
    public bool isSingtarget = false; // 단일 타겟인지
    public bool hasSlowEffect; // 슬로우 시키는지 여부
    public float slowAmount; // 슬로우 효과의 강도
    public bool hasDefenseReduction; // 방어력 감소 여부
    public float defenseReductionAmount; // 방어력 감소의 강도

    public virtual void ActivateSkill(CharacterInfo caster, GameObject target)
    {
        if (skillPrefab == null)
        {
            Debug.LogError("Skill prefab is not assigned!");
            return;
        }

        Debug.Log($"{caster.Name} used {skillName}");

        GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
        SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
        if (skillBehavior != null)
        {
            Vector3 targetPosition = target.transform.position;
            skillBehavior.Initialize(caster, skillDamage, skillRange, targetPosition, target, isSingtarget, hasSlowEffect, slowAmount, hasDefenseReduction, defenseReductionAmount,false,2f);

            caster.totalDamageDealt += skillDamage;
        }
    }
    public virtual void ActivateManaSkill(CharacterInfo caster, GameObject target)
    {
        if (skillPrefab == null)
        {
            Debug.LogError("Skill prefab is not assigned!");
            return;
        }

        Debug.Log($"{caster.Name} used {skillName}");

        GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
        SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
        if (skillBehavior != null)
        {
            Vector3 targetPosition = target.transform.position;
            skillBehavior.Initialize(caster, skillDamage, skillRange, targetPosition, target, false, hasSlowEffect, slowAmount, hasDefenseReduction, defenseReductionAmount, false, 10f);
            caster.totalDamageDealt += skillDamage;
        }
    }
    public virtual float CalculateDamage(float targetMaxHealth, float damage)
    {
        return targetMaxHealth * damage; // 기본적으로 몬스터 최대 체력의 n% 데미지
    }
}