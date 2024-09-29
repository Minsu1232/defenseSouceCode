using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MythMagician : Magician
{
    public GameObject skillPrefab; // ��ų ������
    public GameObject manaSkillPrefab;

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����

        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            new AreaDevelopmentSkill
            {
                skillName = "���� ����",
                skillDescription = " ������ ���� ������ ���ָ� �̴ϴ�.",
                skillDamage = characterData.attackPower * 0.6f,
                skillRange = 1.5f,
                skillProbability = 0.1f,
                skillPrefab = skillPrefab, // �ν����Ϳ��� �Ҵ�� ������ ���
                hasSlowEffect = true,
                slowAmount = 1f,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.005f,
            },
             new MythMagicianBuff
            {
                skillName = "�޸������",
                skillDescription = "��ġ�� ������ ����� ��Ĩ�ϴ�",                
                skillPrefab = manaSkillPrefab,   // ���� ��ų ������ ���
                manaCost = 300f,
                manaRegenRate = 10f,
                maxMana = 300f,              
                mpBar = mpBar // UI���� �Ҵ�� MP �� �̹���
            }
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
