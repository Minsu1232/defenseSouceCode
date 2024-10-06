using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScytheSkill : Skill
{
    public DeathScytheSkill(Skill data)
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
    public override float CalculateDamage(float targetMaxHealth, float damage)
    {

        float calculatedDamage = targetMaxHealth * damage; // �ִ� ü���� 5% ������
        return Mathf.Min(calculatedDamage, 3500f); // �ִ� 3500 �������� ����
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

        // ��ų ���� ���� ��� ���� �����Ͽ� �迭�� ��ȯ
        Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, skillRange, caster.enemyLayer);

        Monster highestHealthEnemy = null;
        float highestHealth = 0;

        // ������ �� �߿��� ���� ü���� ���� ���� ã��
        foreach (var hit in hits)
        {
            Monster enemy = hit.GetComponent<Monster>();
            if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie)
            {
                if (enemy.currentHealth > highestHealth)
                {
                    highestHealth = enemy.currentHealth;
                    highestHealthEnemy = enemy;
                }
            }
        }

        // ���� ü���� ���� ���� ���� ��� ��ų ����
        if (highestHealthEnemy != null)
        {
            GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
            SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
            if (skillBehavior != null)
            {
                skillBehavior.Initialize(caster, CalculateDamage(highestHealthEnemy.maxHealth, finalDamage), 0, highestHealthEnemy.transform.position, highestHealthEnemy.gameObject, isSingtarget, hasSlowEffect, slowAmount, hasDefenseReduction, defenseReductionAmount, isSpecialSkill, duration, speed);
            }
        }
    }
}
