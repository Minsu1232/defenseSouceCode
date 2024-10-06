using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimevalWarriorManaSkill : ManaSkill
{
    public GameObject sky; // ��ǥ ���� (sky)
    public GameObject meteorPrefab; // � ������
    public Vector3 meteorDropPosition; // ��� ������ ��ġ
    public PrimevalWarriorManaSkill(Skill data)
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
        if (skillPrefab == null || meteorPrefab == null || sky == null)
        {
            Debug.LogError("Prefab or target is not assigned!");
            return;
        }

        Debug.Log($"{caster.Name} used {skillName}");

        // �˱� ���� �� �ʱ�ȭ
        GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
        SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
        if (skillBehavior != null)
        {
            Vector3 targetPosition = sky.transform.position;

            // �˱Ⱑ ��ǥ ������ �����ϸ� ��� ����߸��� �ݹ� ����
            skillBehavior.OnSwordDestroyed += DropMeteor;

            // �˱� �ʱ�ȭ
            skillBehavior.Initialize(caster, finalDamage, skillRange, targetPosition, sky, isSingtarget, hasSlowEffect, slowAmount, hasDefenseReduction, defenseReductionAmount, isSpecialSkill, duration, speed);
        }
    }

    // �˱Ⱑ ��ǥ ������ �������� �� ȣ��Ǵ� �޼���
    private void DropMeteor(Vector3 dropPosition)
    {
        Debug.Log("Meteor will fall!");

        // ��� ������ ��ġ�� ����
        GameObject meteorInstance = GameObject.Instantiate(meteorPrefab, meteorDropPosition, Quaternion.identity);

        // ��� �߰����� ������ �ʿ��ϸ� ���⿡ �߰� ����
        // ��: meteorInstance.GetComponent<...>().SomeFunction();
    }
}
