using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillName; // ��ų�̸�
    public string skillDescription; // ��ų����
    public float skillDamage; // ��ų������
    public float skillRange; // ��ų����
    public float skillProbability; // ��ųȮ��
    public GameObject skillPrefab;
    public bool isSingtarget = false; // ���� Ÿ������
    public bool hasSlowEffect; // ���ο� ��Ű���� ����
    public float slowAmount; // ���ο� ȿ���� ����
    public bool hasDefenseReduction; // ���� ���� ����
    public float defenseReductionAmount; // ���� ������ ����

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
        return targetMaxHealth * damage; // �⺻������ ���� �ִ� ü���� n% ������
    }
}