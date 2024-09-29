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
        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����

        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            new PrimevalMagicianSkill
            {
                skillName = "������",
                skillDescription = "������",                
                skillRange = 5f,
                skillProbability = 0.3f,
                skillPrefab = skillPrefab, // �ν����Ϳ��� �Ҵ�� ������ ���
             
            },
             new PrimevalMagicianManaSkill
            {
                skillName = "���� ����",
                skillDescription = "���� ����",
                skillDamage = characterData.attackPower * 10f,
                skillRange = 3f,
                
                skillPrefab = manaSkillPrefab ,   // ���� ��ų ������ ���
                debuffPrefab = debuffPrefab ,
                manaCost = 100f,
                manaRegenRate = 10f,
                maxMana = 100f,
                hasSlowEffect = false,
                slowAmount = 0f,
                hasDefenseReduction = false,
                defenseReductionAmount = 0f,
                mpBar = mpBar // UI���� �Ҵ�� MP �� �̹���
            }
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
