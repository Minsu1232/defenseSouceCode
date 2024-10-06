using System.Collections;
using System.Collections.Generic;
using System.Linq; // System.Linq ���ӽ����̽��� ����մϴ�.
using UnityEngine;

public class PrimevalWarriorSkill : Skill
{
    public PrimevalWarriorSkill(Skill data)
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
    public GameObject barrierPrefab; // ����� ������
    public GameObject swordCrack; // Į�� ������ �� ���� ���� ������
    public Transform[] dropPoints; // Į�� ������ �� �ִ� ������ ����Ʈ��
   

    private List<Transform> availableDropPoints; // ��� ������ ��� ����Ʈ ����Ʈ
    private List<Vector3> usedDropPointPositions = new List<Vector3>(); // ���� ��� ����Ʈ ��ġ ����Ʈ
    private int skillActivationCount = 0; // ��ų�� �ߵ��� Ƚ��

    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {

        base.ActivateSkill(caster, target);
        // ��ų�� �̹� 4�� �ߵ��Ǿ��ٸ� �� �̻� �ߵ����� ����
        if (skillActivationCount >= 4)
        {           
            Debug.Log("The skill has been activated 4 times and cannot be used anymore.");
            return;
        }

        // ��� ������ ��� ����Ʈ ����Ʈ�� �ʱ�ȭ���� �ʾҴٸ� �ʱ�ȭ�մϴ�.
        if (availableDropPoints == null || availableDropPoints.Count == 0)
        {
            availableDropPoints = dropPoints.ToList();
        }

        // ��� ������ ��� ����Ʈ���� �����ϰ� ����
        int randomIndex = Random.Range(0, availableDropPoints.Count);
        Transform chosenDropPoint = availableDropPoints[randomIndex];

        // ���� ��ġ�� ���� (y���� +3 �� ��ġ���� ����)
        Vector3 startPosition = new Vector3(chosenDropPoint.position.x, chosenDropPoint.position.y + 3, chosenDropPoint.position.z);

        // Į�� �����ϰ� ��ġ�� ����
        GameObject swordInstance = GameObject.Instantiate(skillPrefab, startPosition, Quaternion.identity);
        SkillBehavior skillBehavior = swordInstance.GetComponent<SkillBehavior>();
        if (skillBehavior != null)
        {
            // Ÿ���� ��ġ�� �ƴ� Į�� ������ ��ġ�� ����
            Vector3 targetPosition = chosenDropPoint.position;
            skillBehavior.Initialize(caster, finalDamage, skillRange, targetPosition, chosenDropPoint.gameObject, isSingtarget, hasSlowEffect, slowAmount, hasDefenseReduction, defenseReductionAmount, isSpecialSkill, duration, speed);

            // ���� �ı��� �� ȣ��� �ݹ� ����
            skillBehavior.OnSwordDestroyed = OnSwordDestroyed;
        }

        // ����� ��� ����Ʈ�� ��ġ�� ����Ʈ�� �߰��ϰ�, ��� ������ ����Ʈ���� ����
        usedDropPointPositions.Add(chosenDropPoint.position);
        availableDropPoints.RemoveAt(randomIndex);

        // ���� ���� ��ġ�� ���� (y���� -0.8�� �̵�)
        Vector3 crackPosition = new Vector3(chosenDropPoint.position.x, chosenDropPoint.position.y - 0.65f, chosenDropPoint.position.z);

        // ���� ����
        GameObject crackInstance = GameObject.Instantiate(swordCrack, crackPosition, Quaternion.identity);

        // ��ų �ߵ� Ƚ�� ����
        skillActivationCount++;
    }

    private void OnSwordDestroyed(Vector3 dropPointPosition)
    {
        // ���� ��� ����Ʈ�� �ٽ� ��� ������ ����Ʈ�� �߰�
        Transform dropPoint = dropPoints.FirstOrDefault(dp => dp.position == dropPointPosition);
        if (dropPoint != null)
        {
            availableDropPoints.Add(dropPoint);
            usedDropPointPositions.Remove(dropPointPosition);
        }

        // ��ų �ߵ� Ƚ�� ����
        skillActivationCount--;

        Debug.Log("Sword destroyed. Skill can be used again.");
    }
}
