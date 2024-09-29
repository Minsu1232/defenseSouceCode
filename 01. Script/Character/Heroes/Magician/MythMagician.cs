using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MythMagician : Magician
{
    public GameObject skillPrefab; // 스킬 프리팹
    public GameObject manaSkillPrefab;

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // 영웅이 활성화될 때 패시브 효과 적용

        // 스킬 초기화
        skills = new List<Skill>
        {
            new AreaDevelopmentSkill
            {
                skillName = "영역 전개",
                skillDescription = " 영역을 펼쳐 적에게 저주를 겁니다.",
                skillDamage = characterData.attackPower * 0.6f,
                skillRange = 1.5f,
                skillProbability = 0.1f,
                skillPrefab = skillPrefab, // 인스펙터에서 할당된 프리팹 사용
                hasSlowEffect = true,
                slowAmount = 1f,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.005f,
            },
             new MythMagicianBuff
            {
                skillName = "메모라이즈",
                skillDescription = "리치가 과거의 기억이 스칩니다",                
                skillPrefab = manaSkillPrefab,   // 마나 스킬 프리팹 사용
                manaCost = 300f,
                manaRegenRate = 10f,
                maxMana = 300f,              
                mpBar = mpBar // UI에서 할당된 MP 바 이미지
            }
            // 추가 스킬 초기화
        };
    }
}
