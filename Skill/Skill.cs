using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Skill
{
    public int ID; // 스킬의 고유 ID
    public string skillName; // 스킬이름
    public string skillDescription; // 스킬설명
    public float skillDamage; // 스킬데미지
    public float skillRange; // 스킬범위
    public float skillProbability; // 스킬확률
    public GameObject skillPrefab;
    public List<GameObject> skillArray = new List<GameObject>();
    public bool isSingtarget = false; // 단일 타겟인지isSingleTarget
    public bool hasSlowEffect; // 슬로우 시키는지 여부
    public float slowAmount; // 슬로우 효과의 강도
    public bool hasDefenseReduction; // 방어력 감소 여부
    public float defenseReductionAmount; // 방어력 감소의 강도
    public bool isSpecialSkill; // 스페셜 스킬 여부 (추가)
    public float duration; // 스킬 지속 시간 (추가)
    public float speed; // 스킬 속도 (추가)
    public float manaCost; // 스킬 발동에 필요한 마나
    public float manaRegenRate; // 시간당 마나 회복량
    public float currentMana; // 현재 마나
    public float maxMana; // 최대 마나
    public float manaGainPerAttack; // 기본 공격 시 획득하는 마나량
    public float cloneDuration; // 분신 지속 시간
    public float clonePowerMultiplier; // 분신의 능력 배율
    public GameObject otherSkillPrefab;
    public Image mpBar; // 마나바 UI
    public float finalDamage;
    public virtual void ActivateSkill(CharacterInfo caster, GameObject target)
    {
        finalDamage = CalculateFinalDamage(caster.AttackPower, skillDamage);
    }
    public virtual void ActivateManaSkill(CharacterInfo caster, GameObject target)
    {
        finalDamage = CalculateFinalDamage(caster.AttackPower, skillDamage);
    }
    public virtual float CalculateDamage(float targetMaxHealth, float damage)
    {
        return targetMaxHealth * damage; // 기본적으로 몬스터 최대 체력의 n% 데미지
    }
    public virtual float CalculateFinalDamage(float casterAttackPower, float skillDamageMultiplier)
    {
        return casterAttackPower * skillDamageMultiplier; // 공격력 * 스킬 배율
    }
}