using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunOveSkill : Skill
{
    public SunOveSkill(Skill data)
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

        //// ��ų ���� ���� ��� ���� �����Ͽ� �迭�� ��ȯ
        //Collider2D[] hits = Physics2D.OverlapCircleAll(caster.transform.position, skillRange, caster.enemyLayer);

        //// ������ ���� �ϳ��� �ִ��� Ȯ��
        //foreach (var hit in hits)
        //{
        //    Monster enemy = hit.GetComponent<Monster>();
        //    if (enemy != null && enemy.currentHealth > 0 && !enemy.isDie) // ���� �����ϰ� ����ִ��� Ȯ��
        //    {                                
        GameObject skillInstance = GameObject.Instantiate(skillPrefab, caster.transform.position, Quaternion.identity);
        SkillBehavior skillBehavior = skillInstance.GetComponent<SkillBehavior>();
        if (skillBehavior != null)
        {
            Vector3 targetPosition = target.transform.position;
            skillBehavior.Initialize(caster, finalDamage, skillRange, targetPosition, target, false, false, 0, true, defenseReductionAmount, isSpecialSkill,duration,speed);

        }
    }
}




