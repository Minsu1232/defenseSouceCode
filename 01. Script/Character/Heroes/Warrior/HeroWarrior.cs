using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static CharacterInfo;

public class HeroWarrior : Warrior
{
    public GameObject[] chefPrefabs; // 스킬 프리팹

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // 영웅이 활성화될 때 패시브 효과 적용

        // 스킬 초기화
        skills = new List<Skill>
        {
            new ChefSkill
            {
                skillName = "오마카세",
                skillDescription = "체력이 가장 많은 적에게 랜덤한 음식을 날립니다.",
                skillDamage = characterData.attackPower * 2.5f,
                skillRange = 3.0f, // 범위 설정 (예: 5.0f)
                skillProbability = 0.4f,
                gameObjects = chefPrefabs, // 인스펙터에서 할당된 프리팹 사용
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.01f,
            },
            // 추가 스킬 초기화
        };
    }
}
