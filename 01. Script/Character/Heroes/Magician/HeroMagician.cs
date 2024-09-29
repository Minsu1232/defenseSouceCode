using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static CharacterInfo;

public class HeroMagician : Magician
{
    public GameObject skeletonPrefab; // 콜드빔 프리팹

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // 영웅이 활성화될 때 패시브 효과 적용

        // 스킬 초기화
        skills = new List<Skill>
        {
            new Skill
            {
                skillName = "데스샷",
                skillDescription = "유령을 적에게 날립니다.",
                skillDamage = characterData.attackPower * 2f,
                skillRange = 2f,
                skillProbability = 0.5f,
                skillPrefab = skeletonPrefab, // 인스펙터에서 할당된 프리팹 사용
                hasSlowEffect = false,
                slowAmount = 0,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.01f,
            },
            // 추가 스킬 초기화
        };
    }
}
