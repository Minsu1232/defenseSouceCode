using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static CharacterInfo;

public class HeroArcher : Archer
{
    public GameObject skillArrow; // 스킬 화살 프리팹
    protected override void Start()
    {
        base.Start();       
        // 스킬 초기화
        skills = new List<Skill>
        {
            new ArrowShotSkill
            {
                skillName = "데빌애로우샷",
                skillDescription = "다크엘프의 힘이 담긴 화살을 쏩니다.",
                skillDamage = characterData.attackPower * 1.5f,
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
   

