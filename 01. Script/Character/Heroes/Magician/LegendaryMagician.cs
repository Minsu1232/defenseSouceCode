using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegendaryMagician : Magician
{
    public GameObject sunOve; // 스킬 프리팹

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // 영웅이 활성화될 때 패시브 효과 적용

        // 스킬 초기화
        skills = new List<Skill>
        {
            new SunOveSkill
            {
                skillName = "라의 힘",
                skillDescription = "라의 힘을 빌려 태양 오브를 소환합니다.",
                skillDamage = characterData.attackPower * 0.75f,
                skillRange = 1f,
                skillProbability = 0.2f,
                skillPrefab = sunOve, // 인스펙터에서 할당된 프리팹 사용
                hasSlowEffect = false,
                slowAmount = 0,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.015f,
            },
            // 추가 스킬 초기화
        };
    }
}
