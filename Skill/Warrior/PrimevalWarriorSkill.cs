using System.Collections;
using System.Collections.Generic;
using System.Linq; // System.Linq 네임스페이스를 사용합니다.
using UnityEngine;

public class PrimevalWarriorSkill : Skill
{
    public PrimevalWarriorSkill(Skill data)
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
    public GameObject barrierPrefab; // 결계의 프리팹
    public GameObject swordCrack; // 칼이 떨어진 후 생길 금의 프리팹
    public Transform[] dropPoints; // 칼이 떨어질 수 있는 지정된 포인트들
   

    private List<Transform> availableDropPoints; // 사용 가능한 드롭 포인트 리스트
    private List<Vector3> usedDropPointPositions = new List<Vector3>(); // 사용된 드롭 포인트 위치 리스트
    private int skillActivationCount = 0; // 스킬이 발동된 횟수

    public override void ActivateSkill(CharacterInfo caster, GameObject target)
    {

        base.ActivateSkill(caster, target);
        // 스킬이 이미 4번 발동되었다면 더 이상 발동하지 않음
        if (skillActivationCount >= 4)
        {           
            Debug.Log("The skill has been activated 4 times and cannot be used anymore.");
            return;
        }

        // 사용 가능한 드롭 포인트 리스트가 초기화되지 않았다면 초기화합니다.
        if (availableDropPoints == null || availableDropPoints.Count == 0)
        {
            availableDropPoints = dropPoints.ToList();
        }

        // 사용 가능한 드롭 포인트에서 랜덤하게 선택
        int randomIndex = Random.Range(0, availableDropPoints.Count);
        Transform chosenDropPoint = availableDropPoints[randomIndex];

        // 시작 위치를 설정 (y값을 +3 한 위치에서 시작)
        Vector3 startPosition = new Vector3(chosenDropPoint.position.x, chosenDropPoint.position.y + 3, chosenDropPoint.position.z);

        // 칼을 생성하고 위치를 설정
        GameObject swordInstance = GameObject.Instantiate(skillPrefab, startPosition, Quaternion.identity);
        SkillBehavior skillBehavior = swordInstance.GetComponent<SkillBehavior>();
        if (skillBehavior != null)
        {
            // 타겟의 위치가 아닌 칼이 떨어질 위치를 지정
            Vector3 targetPosition = chosenDropPoint.position;
            skillBehavior.Initialize(caster, finalDamage, skillRange, targetPosition, chosenDropPoint.gameObject, isSingtarget, hasSlowEffect, slowAmount, hasDefenseReduction, defenseReductionAmount, isSpecialSkill, duration, speed);

            // 검이 파괴될 때 호출될 콜백 연결
            skillBehavior.OnSwordDestroyed = OnSwordDestroyed;
        }

        // 사용한 드롭 포인트의 위치를 리스트에 추가하고, 사용 가능한 리스트에서 제거
        usedDropPointPositions.Add(chosenDropPoint.position);
        availableDropPoints.RemoveAt(randomIndex);

        // 금이 생길 위치를 설정 (y값을 -0.8로 이동)
        Vector3 crackPosition = new Vector3(chosenDropPoint.position.x, chosenDropPoint.position.y - 0.65f, chosenDropPoint.position.z);

        // 금을 생성
        GameObject crackInstance = GameObject.Instantiate(swordCrack, crackPosition, Quaternion.identity);

        // 스킬 발동 횟수 증가
        skillActivationCount++;
    }

    private void OnSwordDestroyed(Vector3 dropPointPosition)
    {
        // 사용된 드롭 포인트를 다시 사용 가능한 리스트에 추가
        Transform dropPoint = dropPoints.FirstOrDefault(dp => dp.position == dropPointPosition);
        if (dropPoint != null)
        {
            availableDropPoints.Add(dropPoint);
            usedDropPointPositions.Remove(dropPointPosition);
        }

        // 스킬 발동 횟수 감소
        skillActivationCount--;

        Debug.Log("Sword destroyed. Skill can be used again.");
    }
}
