using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Skill
{
    public int ID; // ��ų�� ���� ID
    public string skillName; // ��ų�̸�
    public string skillDescription; // ��ų����
    public float skillDamage; // ��ų������
    public float skillRange; // ��ų����
    public float skillProbability; // ��ųȮ��
    public GameObject skillPrefab;
    public List<GameObject> skillArray = new List<GameObject>();
    public bool isSingtarget = false; // ���� Ÿ������isSingleTarget
    public bool hasSlowEffect; // ���ο� ��Ű���� ����
    public float slowAmount; // ���ο� ȿ���� ����
    public bool hasDefenseReduction; // ���� ���� ����
    public float defenseReductionAmount; // ���� ������ ����
    public bool isSpecialSkill; // ����� ��ų ���� (�߰�)
    public float duration; // ��ų ���� �ð� (�߰�)
    public float speed; // ��ų �ӵ� (�߰�)
    public float manaCost; // ��ų �ߵ��� �ʿ��� ����
    public float manaRegenRate; // �ð��� ���� ȸ����
    public float currentMana; // ���� ����
    public float maxMana; // �ִ� ����
    public float manaGainPerAttack; // �⺻ ���� �� ȹ���ϴ� ������
    public float cloneDuration; // �н� ���� �ð�
    public float clonePowerMultiplier; // �н��� �ɷ� ����
    public GameObject otherSkillPrefab;
    public Image mpBar; // ������ UI
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
        return targetMaxHealth * damage; // �⺻������ ���� �ִ� ü���� n% ������
    }
    public virtual float CalculateFinalDamage(float casterAttackPower, float skillDamageMultiplier)
    {
        return casterAttackPower * skillDamageMultiplier; // ���ݷ� * ��ų ����
    }
}