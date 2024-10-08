using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimevalWarriorManaSkill : ManaSkill
{
    public GameObject sky; // 목표 지점 (sky)
    public GameObject meteorPrefab; // 운석 프리팹
    public Vector3 meteorDropPosition; // 운석이 떨어질 위치
    public PrimevalWarriorManaSkill(Skill data)
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
        if (skillPrefab == null || meteorPrefab == null || sky == null)
        {
            Debug.LogError("Prefab or target is not assigned!");
            return;
        }

        Debug.Log($"{caster.Name} used {skillName}");

        // 검기 생성 및 초기화
        GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
        SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
        if (skillBehavior != null)
        {
            Vector3 targetPosition = sky.transform.position;

            // 검기가 목표 지점에 도달하면 운석을 떨어뜨리는 콜백 연결
            skillBehavior.OnSwordDestroyed += DropMeteor;

            // 검기 초기화
            skillBehavior.Initialize(caster, finalDamage, skillRange, targetPosition, sky, isSingtarget, hasSlowEffect, slowAmount, hasDefenseReduction, defenseReductionAmount, isSpecialSkill, duration, speed);
        }
    }

    // 검기가 목표 지점에 도달했을 때 호출되는 메서드
    private void DropMeteor(Vector3 dropPosition)
    {
        Debug.Log("Meteor will fall!");

        // 운석을 설정된 위치에 생성
        GameObject meteorInstance = GameObject.Instantiate(meteorPrefab, meteorDropPosition, Quaternion.identity);

        // 운석에 추가적인 설정이 필요하면 여기에 추가 가능
        // 예: meteorInstance.GetComponent<...>().SomeFunction();
    }
}
