using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RareMagician : Magician
{
    public GameObject coldBeamPrefab; // 콜드빔 프리팹

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // 영웅이 활성화될 때 패시브 효과 적용

        // 스킬 초기화
        skills = new List<Skill>
        {
            new Skill
            {
                skillName = "콜드빔",
                skillDescription = "주의 적들에게 얼음조각을 날립니다.",
                skillDamage = characterData.attackPower * 1.5f,
                skillRange = 2f,
                skillProbability = 0.5f,
                skillPrefab = coldBeamPrefab, // 인스펙터에서 할당된 프리팹 사용
                hasSlowEffect = true,
                slowAmount = 3f,
                hasDefenseReduction = false,
                defenseReductionAmount = 0,
            },
            // 추가 스킬 초기화
        };
    }
}
