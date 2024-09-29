using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegendaryMagician : Magician
{
    public GameObject sunOve; // ��ų ������

    protected override void Start()
    {
        base.Start();
        PassiveEffect(); // ������ Ȱ��ȭ�� �� �нú� ȿ�� ����

        // ��ų �ʱ�ȭ
        skills = new List<Skill>
        {
            new SunOveSkill
            {
                skillName = "���� ��",
                skillDescription = "���� ���� ���� �¾� ���긦 ��ȯ�մϴ�.",
                skillDamage = characterData.attackPower * 0.75f,
                skillRange = 1f,
                skillProbability = 0.2f,
                skillPrefab = sunOve, // �ν����Ϳ��� �Ҵ�� ������ ���
                hasSlowEffect = false,
                slowAmount = 0,
                hasDefenseReduction = true,
                defenseReductionAmount = 0.015f,
            },
            // �߰� ��ų �ʱ�ȭ
        };
    }
}
