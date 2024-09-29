using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MythArcher : Archer
{
    public GameObject skillArrow; // 스킬 화살 프리팹
    public GameObject manaSkillPrefab;   // 마나 스킬 프리팹
    
    protected override void Start()
    {
        base.Start();
       

        // 스킬 초기화
        skills = new List<Skill>
        {
            // 기본 스킬 추가
            new BaoPuSkill
            {
                skillName = "죽창",
                skillDescription = "적을 공격시 죽창도 함께 공격 합니다.",
                skillDamage = characterData.attackPower * 1.5f,
                skillRange = 5.5f,
                skillProbability = 1f,
                skillPrefab = skillArrow, // 인스펙터에서 할당된 프리팹 사용
                isSingtarget = true,
                hasSlowEffect = false,
                slowAmount = 0,
                hasDefenseReduction = false,
                defenseReductionAmount = 0,
            },

            // 마나 스킬 추가
            new BaoPuManaSkill
            {
                skillName = "죽림",
                skillDescription = "하늘에서 대나무가 쏟아 집니다.",
                skillDamage = characterData.attackPower * 4f,
                skillRange = 4f,
                
                skillPrefab = manaSkillPrefab,   // 마나 스킬 프리팹 사용
                manaCost = 100f,
                manaRegenRate = 10f,
                maxMana = 100f,
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = false,
                defenseReductionAmount = 0f,
                mpBar = mpBar // UI에서 할당된 MP 바 이미지
            }
        };
    }
}
    
  


