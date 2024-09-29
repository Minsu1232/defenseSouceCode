using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class LegendaryArcher : Archer
{  
    public GameObject skillArrow; // ��ų ȭ�� ������
    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����

        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            new ArrowRainSkill
            {
                skillName = "��ȭ���",
                skillDescription = "������ ���鿡�� ��ȭ��� �����ϴ�.",
                skillDamage = characterData.attackPower * 3f,
                skillRange = 3.5f,
                skillProbability = 0.5f,
                skillPrefab = skillArrow, // �ν����Ϳ��� �Ҵ�� ������ ���
                hasSlowEffect = false,
                slowAmount = 0,
                hasDefenseReduction = false,
                defenseReductionAmount = 0,
            },
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
