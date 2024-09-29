using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimevalMagician : Magician
{
    public GameObject skillPrefab;
    public GameObject buffPrefab;
    public GameObject manaSkillPrefab;
    public GameObject debuffPrefab;
    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // 영웅이 활성화될 때 패시브 효과 적용

        // 스킬 초기화
        skills = new List<Skill>
        {
            new PrimevalMagicianSkill
            {
                skillName = "갓블레스",
                skillDescription = "갓블레스",                
                skillRange = 5f,
                skillProbability = 0.3f,
                skillPrefab = skillPrefab, // 인스펙터에서 할당된 프리팹 사용
             
            },
             new PrimevalMagicianManaSkill
            {
                skillName = "빛의 심판",
                skillDescription = "빛의 심판",
                skillDamage = characterData.attackPower * 10f,
                skillRange = 3f,
                
                skillPrefab = manaSkillPrefab ,   // 마나 스킬 프리팹 사용
                debuffPrefab = debuffPrefab ,
                manaCost = 100f,
                manaRegenRate = 10f,
                maxMana = 100f,
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = false,
                defenseReductionAmount = 0f,
                mpBar = mpBar // UI에서 할당된 MP 바 이미지
            }
            // 추가 스킬 초기화
        };
    }
}
