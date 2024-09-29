using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RareArcher : Archer
{
    public GameObject skillArrow; // 스킬 화살 프리팹
    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // 영웅이 활성화될 때 패시브 효과 적용

        // 스킬 초기화
        skills = new List<Skill>
        {
            new ArrowShotSkill
            {
                skillName = "애로우샷",
                skillDescription = "주위의 적들에게 화살을 쏩니다.",
                skillDamage = characterData.attackPower * 1.3f,
                skillRange = 3.5f,
                skillProbability = 0.3f,
                skillPrefab = skillArrow, // 인스펙터에서 할당된 프리팹 사용
                hasSlowEffect = false,
                slowAmount = 0,
                hasDefenseReduction = false,
                defenseReductionAmount = 0,
            },
            // 추가 스킬 초기화
        };
    }
}
