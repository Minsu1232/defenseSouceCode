using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MythWarrior : Warrior
{
    public GameObject swordSlash; // 스킬 프리팹
    public GameObject manaSkillPrefab;

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // 영웅이 활성화될 때 패시브 효과 적용

        // 스킬 초기화
        skills = new List<Skill>
        {
            new DeathScytheSkill
            {
                skillName = "데스사이드",
                skillDescription = "체력이 가장 많은 적에게 사신의 낫을 휘두릅니다.",
                skillDamage = 0.05f,
                skillRange = 5.0f, // 범위 설정 (예: 5.0f)
                skillProbability = 0.1f,
                skillPrefab = swordSlash, // 인스펙터에서 할당된 프리팹 사용
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.03f,
            },
            new ByenightManaSkill
            {
                skillName = "영혼흡수",
                skillDescription = "적을 때릴때 마다 일정 영혼을 획득합니다. 영혼이 가득차면 분신을 소환합니다 ",                         
                clonePrefab = manaSkillPrefab, // 인스펙터에서 할당된 프리팹 사용
                manaCost = 300f,
                manaRegenRate = 0f,
                maxMana = 300f,
                mpBar = mpBar // UI에서 할당된 MP 바 이미지
            }
            // 추가 스킬 초기화
        };
    }
}
